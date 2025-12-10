using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class AudioUploader : MonoBehaviour
{
    string apiUrl = "https://dev-aiasst-app01-g0g3ckcnfncpbuf0.japanwest-01.azurewebsites.net/question"; // 正確なエンドポイントに修正

    void Start()
    {
        string filePath = Application.streamingAssetsPath + "/sample_question_hws.wav";
        StartCoroutine(UploadAudio(filePath));
    }

    IEnumerator UploadAudio(string filePath)
    {
        // ファイルが存在するか確認
        if (!File.Exists(filePath))
        {
            Debug.LogError("音声ファイルが見つかりません: " + filePath);
            yield break;
        }

        byte[] audioData = File.ReadAllBytes(filePath);

        // フォームデータを構築
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", audioData, "sample_question_hws.wav", "audio/wav");
        form.AddField("input_speech_type", "text");

        UnityWebRequest request = UnityWebRequest.Post(apiUrl, form);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("レスポンス: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("エラー: " + request.responseCode + " - " + request.error);
            Debug.LogError("レスポンス本文: " + request.downloadHandler.text);
        }
    }
}
