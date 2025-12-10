using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Microsoft.CognitiveServices.Speech;
using System.Threading.Tasks;
using System;

public class AzureSpeechClient : MonoBehaviour
{
    private string speechKey = "";
    private string speechRegion = "japanwest";
    private string apiUrl = "";//"https://dev-aiasst-app01-g0g3ckcnfncpbuf0.japanwest-01.azurewebsites.net/question";

    public async void StartRecognition()
    {
        ////XR実機向け
        //if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        //{
        //    Permission.RequestUserPermission(Permission.Microphone);
        //}

        var config = SpeechConfig.FromSubscription(speechKey, speechRegion);
        using var recognizer = new SpeechRecognizer(config);

        Debug.Log("マイク入力から音声認識を開始します...");
        // 5秒待つ（Time.timeScaleの影響なし）




        var result = await recognizer.RecognizeOnceAsync();

        if (result.Reason == ResultReason.RecognizedSpeech)
        {
            string recognizedText = result.Text;
            Debug.Log("認識結果: " + recognizedText);

            StartCoroutine(PostToApi(recognizedText));
        }
        else
        {
            Debug.LogError("音声認識失敗: " + result.Reason);

            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                string recognizedText = result.Text;
                Debug.Log("認識結果: " + recognizedText);
                StartCoroutine(PostToApi(recognizedText));
            }
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(result);
                Debug.LogError($"音声認識がキャンセルされました: {cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Debug.LogError($"エラーコード: {cancellation.ErrorCode}");
                    Debug.LogError($"詳細: {cancellation.ErrorDetails}");
                    //Debug.LogError($"ヒント: Azureキーやリージョン、マイクのアクセス許可を確認してください。");
                }
            }
            else
            {
                Debug.LogError("音声認識失敗: " + result.Reason);
            }

        }
    }

    IEnumerator PostToApi(string recognizedText)
    {
        var payload = new QuestionPayload { file = recognizedText, input = "text" };
        string json = JsonUtility.ToJson(payload);
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("API応答: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("APIエラー: " + request.responseCode + " - " + request.error);
            Debug.LogError("応答内容: " + request.downloadHandler.text);
        }
    }

    [System.Serializable]
    public class QuestionPayload
    {
        public string file;
        public string input;
    }
}
