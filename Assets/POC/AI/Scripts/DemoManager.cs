using System.Collections;
using System.Collections.Generic;
using UglyToad.PdfPig.Graphics.Operations.TextState;
using UnityEngine;

public class DemoManager : MonoBehaviour
{

    public enum DemoState
    {
        Splash=0,     //スプラッシュ画面
        Top,        //ホーム
        Recording,  //録音中
        Wait,       //待機中
        Reply,      //返答　まだ会話を行う物
        Answer,     //会話を終了し動作を行う物
        Video,

    }
    DemoState state = DemoState.Top;

    [SerializeField]
    GameObject splashObj, topObj, recordingObj, waitObj, replyObj,answerObj,videoObj;

    [SerializeField]
    ShowTextWindow recordingTextWindow, replyTextWindow,answerTextWindw;

    [SerializeField]
    bool windowTest = false;

    // Start is called before the first frame update
    void Start()
    {
        SetState(DemoState.Splash);
    }

    private void Update()
    {

        if (windowTest)
        {
            windowTest = false;
            switch (state)
            {
                case DemoState.Splash:
                    SetState(DemoState.Top);
                    break;
                case DemoState.Top:
                    SetState(DemoState.Recording);
                    SetText("録音中デモ");
                    break;
                case DemoState.Recording:
                    SetState(DemoState.Wait);
                    break;
                case DemoState.Wait:
                    SetState(DemoState.Reply);
                    //SetText("返答デモ 文字数テスト文字数テスト\n文字数テスト文字数テスト");
                    break;
                case DemoState.Reply:
                    SetState(DemoState.Answer);
                    SetText("回答デモ");
                    break;
                case DemoState.Answer:
                    SetState(DemoState.Video);
                    break;
                case DemoState.Video:
                    SetState(DemoState.Splash);
                    break;
            }
        }
    }

    public void SetState(DemoState state)
    {
        this.state = state;

        splashObj.SetActive(false);
        topObj.SetActive(false);
        recordingObj.SetActive(false);
        waitObj.SetActive(false);
        replyObj.SetActive(false);
        answerObj.SetActive(false);
        //videoObj.SetActive(false);

        switch (state)
        {
            case DemoState.Splash:
                splashObj.SetActive(true);
                break;
            case DemoState.Top:
                topObj.SetActive(true);
                break;
            case DemoState.Recording:
                recordingObj.SetActive(true);
                break;
            case DemoState.Wait:
                waitObj.SetActive(true);
                break;
            case DemoState.Reply:
                replyObj.SetActive(true);
                break;
            case DemoState.Answer:
                answerObj.SetActive(true);
                break;
            case DemoState.Video:
                topObj.SetActive(true);
                videoObj.SetActive(true);
                break;
        }
    }

    public void SetText(string text)
    {
        switch(state)
        {
            case DemoState.Recording:
                recordingTextWindow.SetText(text);
                recordingTextWindow.Open();
                break;
            case DemoState.Reply:
                replyTextWindow.SetText(text);
                replyTextWindow.Open();
                break;
            case DemoState.Answer:
                answerTextWindw.SetText(text);
                answerTextWindw.Open();
                break;
        }
    }

    //ボタンから呼ぶよう
    public void OnButtonSetState(int state)
    {
        SetState((DemoState)state);
    }

}
