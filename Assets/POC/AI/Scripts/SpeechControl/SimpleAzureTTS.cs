using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using System.Threading.Tasks;

public class SimpleAzureTTS : MonoBehaviour
{
    [Header("Azure Speech 設定")]
    public string azureKey = "15d2a377321445018ec8b47605ad2ef2";
    public string azureRegion = "japaneast";
    public string voiceName = "ja-JP-NanamiNeural";  // 日本語の自然音声

    //todo: 後で削除
    private string text = "サンプルメッセージ";


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
            Debug.Log($"音声読み上げ、失敗：  { result.Reason}");
        }
    }
}
