using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using static FileTypeIdentifier;

public class FileSearchControllerAI : MonoBehaviour
{
    [Header("")]
    [SerializeField, Tooltip("")]
    private PoCPDFPanel pdfComponentPrefab;

    [SerializeField, Tooltip("")]
    private PoCImagePanel imageComponentPrefab;

    [SerializeField, Tooltip("")]
    private PoCVideoPanel videoComponentPrefab;

    [Header("")]
    [SerializeField, Tooltip("")]
    private FileSearchPresenter fileSearchPresenter;

    //
    PDFWindowHandler pdfWindowHandler;
    ImageWindowHandler imageWindowHandler;
    VideoWindowHandler videoWindowHandler;

    private void Awake()
    {
        Initialized().Forget();
    }

    public async UniTask Initialized()
    {
        ValidateComponents();

        TTT();

        SetEvent();
        SetAction();
        Bind();

        PresenterInitialized().Forget();
    }

    public async UniTask PresenterInitialized()
    {
        fileSearchPresenter.Initialized().Forget();
    }

    private void OnDestroy()
    {
        RemoveEvent();
        RemoveAction();
    }

    private void ValidateComponents()
    {
        if (fileSearchPresenter == null) Debug.LogError($"Missing {nameof(fileSearchPresenter)} on {gameObject.name}");

        if (pdfComponentPrefab == null) Debug.LogError($"Missing {nameof(pdfComponentPrefab)} on {gameObject.name}");
        if (imageComponentPrefab == null) Debug.LogError($"Missing {nameof(imageComponentPrefab)} on {gameObject.name}");
        if (videoComponentPrefab == null) Debug.LogError($"Missing {nameof(videoComponentPrefab)} on {gameObject.name}");
    }

    private void TTT()
    {
        pdfWindowHandler = new PDFWindowHandler(pdfComponentPrefab);
        imageWindowHandler = new ImageWindowHandler(imageComponentPrefab);
        videoWindowHandler = new VideoWindowHandler(videoComponentPrefab);
    }

    private void SetEvent()
    {

    }

    private void RemoveEvent()
    {

    }

    private void SetAction()
    {
        fileSearchPresenter.OnOpenViewer += OpenViewer;
    }

    private void RemoveAction()
    {
        fileSearchPresenter.OnOpenViewer -= OpenViewer;
    }

    private void Bind()
    {
        
    }

    //private void OpenViewer(string _filePath)
    public void OpenViewer(string _filePath)
    {
        IDisplayWindowHandler handler;
        FileType fileType = IdentifyFileType(_filePath);

        switch (fileType)
        {
            case FileType.PDF:
                handler = this.pdfWindowHandler.Copy();
                break;
            case FileType.Video:
                handler = this.videoWindowHandler.Copy();
                break;
            case FileType.Image:
                handler = this.imageWindowHandler.Copy();
                break;
            default:
                handler = null;
                break;
        }

        if (handler != null)
        {
            handler.FilePath = _filePath;
            handler.OpenAsync().Forget();
        }
    }
}
