using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    //public VoiceRecorder recorder;
    public SpeechControl speechControl;
    private float wait_time = 8.5f;

    //State管理
    [SerializeField]
    private DemoManager demoManager;

    public void OnStartButton()
    {
        Debug.Log("UIController OnStartButton");
        StateControlRecording();

        //demoManager.SetState(DemoManager.DemoState.Recording);
        demoManager.SetText("ご用件をお伺いいたします");

        Debug.Log("voice rec start");
        //recorder.StartRecording();

        //音声をSTTで送信
        //speechControl.StartRecordingSTT();

        //元々の設定 音声をwavファイルで送信
        speechControl.StartRecording();

        //(音声取得終了処理)
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

    public void StateControlRecording()
    {
        demoManager.SetState(DemoManager.DemoState.Recording);
        //continuousSpeechRecognizer.YaTemiVoiceControl();
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
