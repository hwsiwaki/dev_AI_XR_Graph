using Microsoft.CognitiveServices.Speech;
using System;
using UnityEngine;

public class ContinuousSpeechRecognizer : MonoBehaviour
{
    private SpeechRecognizer recognizer;

    [SerializeField] private static readonly String AzureAISpeechKey = "15d2a377321445018ec8b47605ad2ef2";
    [SerializeField] private static readonly String AzureAISpeechRegion = "japaneast";

    //State管理
    [SerializeField]
    private DemoManager demoManager;

    [SerializeField]
    private GameObject nearMenuTop;

    public UIController uiController;
    public SpeechControl speechControl;
    private float wait_time = 8.5f;

    //void Start()
    //{
        //var config = SpeechConfig.FromSubscription(AzureAISpeechKey, AzureAISpeechRegion);
        ////var config = SpeechConfig.FromSubscription("YOUR_API_KEY", "YOUR_REGION");
        //config.SpeechRecognitionLanguage = "ja-JP";

        //recognizer = new SpeechRecognizer(config);

        //recognizer.Recognizing += (s, e) =>
        //{
        //    Debug.Log($"ya temi 中間認識: {e.Result.Text}");
        //};

        //recognizer.Recognized += async (s, e) =>
        //{
        //    if (e.Result.Reason == ResultReason.RecognizedSpeech)
        //    {
        //        string text = e.Result.Text;
        //        Debug.Log($"ya temi 中間認識認識結果: {text}");

        //        if (text.Contains("ヤッテミー") || text.ToLower().Contains("ya temi") || text.Contains("やあテミー") || text.Contains("やってみ") || text.Contains("やあ。テミー。") || text.Contains("year to me"))
        //        {
        //            Debug.Log("キーワード検出！ → 処理実行");
        //            //await recognizer.StopContinuousRecognitionAsync();

        //            ExecuteTriggeredAction();
        //        }
        //    }
        //};

        //await recognizer.StartContinuousRecognitionAsync();
        //Debug.Log("常時音声認識を開始しました。");
    //}


    public async void YaTemiVoiceControl()
    {
        Debug.Log("YaTemiVoiceControl");

        var config = SpeechConfig.FromSubscription(AzureAISpeechKey, AzureAISpeechRegion);
        //var config = SpeechConfig.FromSubscription("YOUR_API_KEY", "YOUR_REGION");
        config.SpeechRecognitionLanguage = "ja-JP";

        recognizer = new SpeechRecognizer(config);

        recognizer.Recognizing += (s, e) =>
        {
            Debug.Log($"ya temi 中間認識: {e.Result.Text}");
        };

        recognizer.Recognized += async (s, e) =>
        {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                string text = e.Result.Text;
                Debug.Log($"ya temi 中間認識認識結果: {text}");

                if (text.Contains("ヤッテミー") || text.ToLower().Contains("ya temi") || text.Contains("やあテミー") || text.Contains("やってみ") || text.Contains("やあ。テミー。") || text.Contains("year to me"))
                {
                    Debug.Log("キーワード検出！ → 処理実行");
                    //await recognizer.StopContinuousRecognitionAsync();

                    ExecuteTriggeredAction();
                }
            }
        };

        await recognizer.StartContinuousRecognitionAsync();
        Debug.Log("常時音声認識を開始しました。");
    }

    private void ExecuteTriggeredAction()
    {
        // ここに任意の処理を書く（例：UI表示、モデル起動、API通信など）
        Debug.Log("ヤッテミー処理を実行中...");
        //demoManager.SetState(DemoManager.DemoState.Recording);
        uiController.OnStartButton();
    }

    private async void OnDestroy()
    {
        if (recognizer != null)
        {
            await recognizer.StopContinuousRecognitionAsync();
            recognizer.Dispose();
        }
    }

    public void OnStartButton()
    {
        Debug.Log("UIController OnStartButton");
        StateControlRecording();

        //demoManager.SetState(DemoManager.DemoState.Recording);
        demoManager.SetText("録音を開始します");

        Debug.Log("voice rec start");
        //recorder.StartRecording();
        speechControl.StartRecording();

        StateControlRecording();

        //Todo: 元々の設定(音声取得終了処理)
        Invoke(nameof(OnStopButton), wait_time);
    }

    public void OnStopButton()
    {
        Debug.Log("voice rec stop");
        StateControlWait();
        //recorder.StopRecording();
        speechControl.StopRecording();

        //StateControlReply();
    }

    //private async Task StartRecognition()
    //{
    //    if (!isRecognizing)
    //    {
    //        await recognizer.StartContinuousRecognitionAsync();
    //        isRecognizing = true;
    //        Debug.Log("音声認識を開始しました。");
    //    }
    //}

    //private async Task StopRecognition()
    //{
    //    if (isRecognizing)
    //    {
    //        await recognizer.StopContinuousRecognitionAsync();
    //        isRecognizing = false;
    //        Debug.Log("音声認識を停止しました。");
    //    }
    //}

    private void OnKeywordDetected()
    {
        // ↓ここに任意の処理を書く（例：API通信、TTS再生、UI表示）
        Debug.Log("★★『ヤッテミー』が検出されたので処理を実行中... ★★");
    }

    public void StateControlRecording()
    {
        Debug.Log("UIController OnStartButton Start Control Recording");
        //demoManager = new DemoManager();
        demoManager.SetState(DemoManager.DemoState.Recording);
    }

    public void StateControlReply()
    {
        demoManager.SetState(DemoManager.DemoState.Reply);
    }

    public void StateControlTop()
    {
        demoManager.SetState(DemoManager.DemoState.Top);
        //continuousSpeechRecognizer.SetActive(true);
    }

    public void StateControlWait()
    {
        demoManager.SetState(DemoManager.DemoState.Wait);
    }

    public void StateControlAnswer()
    {
        demoManager.SetState(DemoManager.DemoState.Answer);
    }

    public void StateControlSplash()
    {
        demoManager.SetState(DemoManager.DemoState.Splash);
    }
}
