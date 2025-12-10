using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using UnityEngine.Networking;

public class VoiceRecorder : MonoBehaviour
{
    public int maxRecordingTime = 5; // 最大録音時間（秒）
    private AudioClip recordedClip;
    private string filePath;

    void Start()
    {
        filePath = Application.streamingAssetsPath + "/recorded.wav";
    }

    public void StartRecording()
    {
        recordedClip = Microphone.Start(null, false, maxRecordingTime, 44100);
        Debug.Log("録音開始");
    }

    public void StopRecording()
    {
        //if (Microphone.IsRecording(null))
        //{
            Microphone.End(null);
            Debug.Log("録音終了");

            // AudioClip → WAV ファイルに変換
            byte[] wavData = WavUtility.FromAudioClip(recordedClip);
            File.WriteAllBytes(filePath, wavData);

            Debug.Log("WAVファイル保存完了: " + filePath);

            // API送信処理呼び出し
            StartCoroutine(SendToApi(filePath));
        //}
    }

    IEnumerator SendToApi(string wavPath)
    {
        byte[] audioData = File.ReadAllBytes(wavPath);

        WWWForm form = new WWWForm();
        form.AddBinaryData("file", audioData, "recorded.wav", "audio/wav");
        form.AddField("input_speech_type", "text");

        UnityWebRequest request = UnityWebRequest.Post("https://dev-aiasst-app01-g0g3ckcnfncpbuf0.japanwest-01.azurewebsites.net/question", form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("API応答: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("APIエラー: " + request.error);
        }
    }
}
