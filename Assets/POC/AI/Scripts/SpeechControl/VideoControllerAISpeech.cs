using UnityEngine;
using UnityEngine.Video;

public class VideoControllerAISpeech : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    [SerializeField]
    private GameObject videoRawImg;

    [SerializeField]
    private UIController uiController;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        // 動画の再生完了時に呼ばれるイベントを登録
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // 動画を最初のフレームに戻す
        vp.Stop();
        vp.frame = 0;

        // GameObjectを非表示にする
        gameObject.SetActive(false);
        videoRawImg.SetActive(false);

        //stateを初期状態に戻す
        //uiController.StateControlTop();
    }
}
