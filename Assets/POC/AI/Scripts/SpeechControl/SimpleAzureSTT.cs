using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using System.Threading.Tasks;
using System;
using UnityEngine.Networking;

using System.Collections;


public class SimpleAzureSTT : MonoBehaviour
{
    [Header("Azure Speech 設定")]
    public string azureKey = "15d2a377321445018ec8b47605ad2ef2";
    public string azureRegion = "japaneast";
    public string voiceName = "ja-JP-NanamiNeural";  // 日本語の自然音声

    //todo: 後で削除
    private string text = "サンプルメッセージ";

    [SerializeField] private static readonly String AzureAISpeechKey = "15d2a377321445018ec8b47605ad2ef2";
    [SerializeField] private static readonly String AzureAISpeechRegion = "japaneast";
    private string apiUrl = "https://dev-aiasst-app01-g0g3ckcnfncpbuf0.japanwest-01.azurewebsites.net/question";


    // 音声合成を開始する（音声は自動再生される）
    public async void SpeakText()//(string text)
    {

        var config = SpeechConfig.FromSubscription(azureKey, azureRegion);
        config.SpeechSynthesisVoiceName = voiceName;

        using var synthesizer = new SpeechSynthesizer(config);  // OSの再生デバイスで再生
        var result = await synthesizer.SpeakTextAsync(text);

        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
        {
            Debug.Log("音声読み上げ成功");
        }
        else
        {
            //Debug.LogError($"音声合成エラー: {result.Reason}\n{result.ErrorDetails}");
            Debug.Log($"音声読み上げ、失敗：  {result.Reason}");
        }
    }

    //sample 250703 検証用

    private void Start()
    {
        VoiceControlSTT();
    }
    public async void VoiceControlSTT()
    {
        Debug.Log("VoiceControlSTT");

        //var config = SpeechConfig.FromSubscription("15d2a377321445018ec8b47605ad2ef2", "japaneast");
        var config = SpeechConfig.FromSubscription(AzureAISpeechKey, AzureAISpeechRegion);
        config.SpeechRecognitionLanguage = "ja-JP";

        //org記載事項(必要ならコメント外す)
        using var recognizer = new Microsoft.CognitiveServices.Speech.SpeechRecognizer(config);
        //検証用追記
        //recognizer = new SpeechRecognizer(config);

        Debug.Log("音声入力を開始してください...");
        await SpeakText("音声入力を開始してください");
        var result = await recognizer.RecognizeOnceAsync();

        if (result.Reason == ResultReason.RecognizedSpeech)
        {
            Debug.Log($"認識結果: {result.Text}");
            //Todo: 250703 apiサーバーにTTSで接続
            //StartCoroutine(PostToApi(recognizedText));
            StartCoroutine(PostToApi(result.Text));
        }
        else
        {
            Debug.LogWarning($"認識失敗: {result.Reason}");
            Microphone.End(null);
        }
    }

    //Azure speech TTS
    public async Task SpeakText(string speech_message)
    {

        var config = SpeechConfig.FromSubscription(AzureAISpeechKey, AzureAISpeechRegion);
        config.SpeechSynthesisVoiceName = voiceName;

        using var synthesizer = new SpeechSynthesizer(config);  // OSの再生デバイスで再生
        var result = await synthesizer.SpeakTextAsync(speech_message);

        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
        {
            Debug.Log("音声読み上げ成功");
            
        }
        else
        {
            //Debug.LogError($"音声合成エラー: {result.Reason}\n{result.ErrorDetails}");
            Debug.Log($"音声読み上げ、失敗：  {result.Reason}");
        }
    }

    IEnumerator PostToApi(string recognizedText)
    {
        Debug.Log($"PostToApi recognizedText: {recognizedText}");
        //var payload = new QuestionPayload { file = recognizedText, input = "text" };
        var payload = new QuestionPayload { file = "今日の天気を教えて", input = "text" };
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
