using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System;
using System.Linq;
//AIFourdry
using System.Collections.Generic;
using System.Net.Http.Headers;

//voice recorder
using System.Collections;
using System.IO;
using System.Net;
using UnityEngine.Networking;

//APIサーバーからのjson parse
//using System.Collections.Generic;
using Newtonsoft.Json;
//using static Microsoft.MixedReality.Toolkit.Experimental.UI.KeyboardKeyFunc;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEditor;
//using GameDevWare.Serialization;
using UnityEngine.UIElements;

//XR実機向け
using UnityEngine.Android;
using System.Linq;
using Unity.VideoHelper;

//prefabをアドレス指定で読み込み
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


//AIFourdy
[Serializable]
public class ChatMessageFoundty
{
    public string role;
    public string content;
}

public class ChatAIFoundryFunctions
{
    public string name;
    public string description;
}

[Serializable]
public class ChatRequestFoundty
{
    public List<ChatMessageFoundty> messages;
    public int max_tokens = 6000;
    public float temperature = 0f;
    public string model;
}

[Serializable]
public class ChatChoiceFoundty
{
    public ChatMessageFoundty message;
}

[Serializable]
public class ChatResponseFoundty
{
    public List<ChatChoiceFoundty> choices;
}

//APIサーバーからのjsonパース
//using Newtonsoft.Json;
public class MovieName
{
    [JsonProperty("value")]
    public string Value { get; set; }

    [JsonProperty("trust_level")]
    public string TrustLevel { get; set; }
}

public class PDFName
{
    [JsonProperty("value")]
    public string Value { get; set; }

    [JsonProperty("trust_level")]
    public string TrustLevel { get; set; }
}

public class Members
{
    [JsonProperty("value")]
    public List<string> Value { get; set; }

    [JsonProperty("trust_level")]
    public string TrustLevel { get; set; }
}

public class ModelName
{
    [JsonProperty("value")]
    public string Value { get; set; }

    [JsonProperty("trust_level")]
    public string TrustLevel { get; set; }
}

public class StartCall
{
    [JsonProperty("value")]
    public string Value { get; set; }

    [JsonProperty("trust_level")]
    public string TrustLevel { get; set; }
}

public class ShowPDF
{
    [JsonProperty("value")]
    public string Value { get; set; }

    [JsonProperty("trust_level")]
    public string TrustLevel { get; set; }
}

public class FunctionToolResponse
{
    [JsonProperty("movie_name")]
    //public string MovieName { get; set; }
    public MovieName MovieName { get; set; }

    [JsonProperty("question_prompt")]
    public string QuestionPrompt { get; set; }

    [JsonProperty("model_name")]
    //public string ModelName { get; set; }
    public ModelName ModelName { get; set; }

    [JsonProperty("start_call")]
    //public string StartCall { get; set; }
    public StartCall StartCall { get; set; }

    [JsonProperty("show_pdf")]
    //public string ShowPDF { get; set; }
    public ShowPDF ShowPDF { get; set; }

    [JsonProperty("members")]
    //public string StartCall { get; set; }
    public Members Members { get; set; }

    [JsonProperty("pdf_name")]
    //public string StartCall { get; set; }
    public PDFName PDFName { get; set; }
}

public class Function
{
    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("order_type")]
    public int OrderType { get; set; }

    [JsonProperty("functiontool_name")]
    public string FunctionToolName { get; set; }

    [JsonProperty("functiontool_response")]
    public FunctionToolResponse FunctionToolResponse { get; set; }
}

public class Root
{
    [JsonProperty("functions")]
    public List<Function> Functions { get; set; }
}

public class SpeechControl : MonoBehaviour
{
    [Header("Settings Azure Speech ")]
    private SpeechRecognizer recognizer;
    [SerializeField] private static readonly String AzureAISpeechKey = "15d2a377321445018ec8b47605ad2ef2";
    [SerializeField] private static readonly String AzureAISpeechRegion = "japaneast";
    [SerializeField] private static readonly String voiceName = "ja-JP-NanamiNeural";  // Azure TTS, 日本語の自然音声
    [SerializeField] private static readonly String GPTModel = "Phi-4-multimodal";//モデルはAI Foundryに配置のPhi-4-multimodalを利用
    [SerializeField] private static readonly String GPTKey = "";
    [SerializeField] private static readonly String GPTEndpoint = "https://phi-4-mm.services.ai.azure.com/";//"https://rg-azureopenai-s0-je.openai.azure.com/";
    //[SerializeField] private String GPTrole = "何でも知っている丁寧な人";
    //[SerializeField] private String BasePrompt = "次の日本語から意図を抽出し、JSON形式で返してください:";
    //[SerializeField] private String ResponseJsonFormat = "{{\r\n//\"\"intent\"\": \"\"意図の名前\"\",\r\n//\"\"entities\"\": [\"\"関連するキーワード1\"\", \"\"関連するキーワード2\"\"]\r\n//}}";
    //private String ResponseJsonFormat = "{{\r\n    //      \"\"intent\"\": \"\"意図の名前\"\",\r\n    //      \"\"entities\"\": [\"\"関連するキーワード1\"\", \"\"関連するキーワード2\"\"]\r\n    //    }}";
    private bool isRecognizing = false;

    //[SerializeField] private int useAIType = 1; //1:Azure AI Foundry, 2: 通常のChatGPT

    //API Server利用
    [SerializeField] private static readonly string azureAPIServerURL = "https://aigraphxr-app01-gkf5cze8c7g6e9g6.japaneast-01.azurewebsites.net/question"; //"https://dev-aiasst-app01-g0g3ckcnfncpbuf0.japanwest-01.azurewebsites.net/question";
    string apiUrl = "https://aigraphxr-app01-gkf5cze8c7g6e9g6.japaneast-01.azurewebsites.net/question";//"https://dev-aiasst-app01-g0g3ckcnfncpbuf0.japanwest-01.azurewebsites.net/question";

    //Todo: 検証用に仮置き(後で削除)
    //[SerializeField] private GameObject[] PDFWindowObj;
    [SerializeField] private GameObject[] XRModelObj;
    [SerializeField] private GameObject[] MovieObj;

    //prefabをアドレス指定で読み込み
    [SerializeField] private String[] modelPrefabAddress;

    //対象とするキーワードが含まれるときに、特定の操作を行う
    [SerializeField] private string[] targetWord;
    //[SerializeField] private string[] targetWordPDF;
    //[SerializeField] private string[] targetWordXRObj;
    [SerializeField] private string[] functionCalling;
    //private string targetWordEnd = "アクションの実行";
    private bool targetXRObjRotateFlag = false;

    //voice recorder
    public int maxRecordingTime = 5;//5; // 最大録音時間（秒）
    private AudioClip recordedClip;
    private string filePath;// = Application.streamingAssetsPath + "/recorded.wav";

    //音声書き込み(質疑応答時)
    [SerializeField] private TextMeshPro resoponseAITextDialog;
    //[SerializeField] private TextMeshProUGUI resoponseAITextDialog;
    //Debug用
    [SerializeField] private TextMeshPro resoponseAITextDialogDebug;

    //XR control
    public ShowTextWindow showTextWindow;

    //State管理
    [SerializeField]
    private DemoManager demoManager;

    ////文言表示演出
    //[SerializeField]
    //private ShowTextWindow showTextWindowDialog; //showTextWindow

    //効果音の再生
    [SerializeField] private AudioClip[] audioClip;
    [SerializeField] private AudioSource audioSource;

    public UIController uiController;

    //STT
    private string speechKey = "15d2a377321445018ec8b47605ad2ef2";
    private string speechRegion = "japanwest";

    //グラスデバイス上のFile検索
    [SerializeField]
    private FileSearchControllerAI fileSearchControllerAI;

    //リソースファイルのパス取得
    private string fileResourcePath;
    //private string fileResourcePathMeta;
    private string videoFileResourcePath;
    private string pdfFileResourcePath;
    //動画
    [SerializeField]
    private VideoController videoController;

    void Start()
    {
        //XR実機向け
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }


        DisappearAllObj();
        //filePath = Application.streamingAssetsPath + "/recorded.wav";
        filePath = Application.persistentDataPath + "/recorded.wav";
        //digilens
        //fileResourcePath = Application.persistentDataPath + "/Files/";
        //meta quest
        fileResourcePath = Application.persistentDataPath;
        //unity editor環境またはwindows環境
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        fileResourcePath = Application.persistentDataPath + "/Files/";
#endif
        //ParseAPIServerResponseData();

        //STTでのAPIサーバーへのリクエスト
        //AzureSpeachRequestParamSTT();
        //StartRecordingSTT();

    }


    private void StateControlRecording()
    {
        demoManager.SetState(DemoManager.DemoState.Recording);
    }

    private void DisappearAllObj()
    {
        Debug.Log("DisappearAllObj");
        //Microphone.End(null);
        //AssetDatabase.DeleteAsset(filePath);

        //for (int i = 0; i < PDFWindowObj.Length; i++)
        //{
        //    PDFWindowObj[i].SetActive(false);
        //}

        for (int i = 0; i < XRModelObj.Length; i++)
        {
            XRModelObj[i].SetActive(false);
        }
        Addressables.LoadAssetAsync<GameObject>(modelPrefabAddress[0]).Completed += OnPrefabLoadedDelete;


        for (int i = 0; i < MovieObj.Length; i++)
        {
            MovieObj[i].SetActive(false);
        }

        //responseAIText.text = null;
        resoponseAITextDialog.text = null;

        //Todo: XR側でのTextwindow操作
        showTextWindow.Close();

    }

    public async void VoiceControlSTT()
    {
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
        }
        else
        {
            Debug.LogWarning($"認識失敗: {result.Reason}");
            Microphone.End(null);
        }
    }

    private void XRModelControl(string messageWord, string modelName, string trustLevel)
    {
        Debug.Log("XRModelControl");
        Debug.Log($"XRModelControl messageWord: {messageWord} modelname: {modelName} trustLevel: {trustLevel}");

        //prefabのアドレス指定での名前検索
        switch (modelName)
        {
            case "EX5600":
                //Todo: model追記(denso用に一時的に追加、prefab addressでの生成をコメントアウト)
                //Addressables.InstantiateAsync(modelPrefabAddress[0], Vector3.zero, Quaternion.identity).Completed += OnPrefabInstantiated;
                XRModelObj[0].SetActive(true);
                break;
            case "EH4000AC-3":
                //Todo: model追記(denso用に一時的に追加、prefab addressでの生成をコメントアウト)
                //Addressables.InstantiateAsync(modelPrefabAddress[1], Vector3.zero, Quaternion.identity).Completed += OnPrefabInstantiated;
                XRModelObj[0].SetActive(true);
                break;
            default:
                //Todo: model追記(denso用に一時的に追加、prefab addressでの生成をコメントアウト)
                //Addressables.InstantiateAsync(modelPrefabAddress[0], Vector3.zero, Quaternion.identity).Completed += OnPrefabInstantiated;
                XRModelObj[0].SetActive(true);
                break;
        }

        //Addressables.LoadAssetAsync<GameObject>(modelPrefabAddress[0]).Completed += OnLoaded;

        //Todo: prefabのアドレスまたは名前をAIサーバー側からの返却値と合わせるようにする。
        //合わせた形で名前またはアドレス検索して、一致したprefabをInstantiateするようにする。
        //Addressables.InstantiateAsync(modelPrefabAddress[0], Vector3.zero, Quaternion.identity).Completed += OnPrefabInstantiated;
        //Addressables.InstantiateAsync(modelName, Vector3.zero, Quaternion.identity).Completed += OnPrefabInstantiated;



        ////model nameで検索して、参照(Linq)
        //var target = XRModelObj.FirstOrDefault(x => x.name == modelName);
        ////Debug.Log($"target: {target}");
        ////Debug.Log($"target name: {target.name}");
        //if (target != null)
        //{
        //    Debug.Log($"target name: {target.name}");
        //    DisappearAllObj();
        //    //target.SetActive(true);
        //    //Instantiate(target, Vector3.zero, Quaternion.identity);
        //    Addressables.InstantiateAsync(modelPrefabAddress[0], Vector3.zero, Quaternion.identity).Completed += OnPrefabInstantiated;
        //    //Addressables.InstantiateAsync(modelName, Vector3.zero, Quaternion.identity).Completed += OnPrefabInstantiated;
        //}
        //else
        //{
        //    Debug.Log("該当の名前のmodelは存在しない");
        //    Debug.Log("model nameが該当しないため、デフォルトのmodelを表示");
        //    //Todo: Instantiateに変更する
        //    //XRModelObj[1].SetActive(true);
        //    //Instantiate(XRModelObj[1], Vector3.zero, Quaternion.identity);
        //    Addressables.InstantiateAsync(modelPrefabAddress[0], Vector3.zero, Quaternion.identity).Completed += OnPrefabInstantiated;
        //}
    }

    private void XRModelDelete()
    {
        Debug.Log("XRModelDelete");
        Addressables.InstantiateAsync(modelPrefabAddress[0], Vector3.zero, Quaternion.identity).Completed += OnPrefabInstantiatedDelete;
    }

    private void OnPrefabLoadedDelete(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject prefabAsset = handle.Result;

            //// シーン上にある全オブジェクトを検索
            //GameObject[] allObjects = FindObjectsOfType<GameObject>();

            //foreach (GameObject obj in allObjects)
            //{
            //    // Prefabの元アセットと比較
            //    if (PrefabUtility.GetCorrespondingObjectFromSource(obj) == prefabAsset)
            //    {
            //        Destroy(obj); // インスタンス削除
            //        Debug.Log($"{modelPrefabAddress[0]} のインスタンスを削除しました: {obj.name}");
            //    }
            //}
        }
        else
        {
            Debug.LogError("指定のAddressable prefabがロードできませんでした: " + modelPrefabAddress[0]);
        }
    }

    //private string OnLoaded(AsyncOperationHandle<GameObject> obj)
    private void OnLoaded(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject loadedPrefab = obj.Result;
            Debug.Log("Model prefab loaded successfully: " + loadedPrefab.name);
            // ここで読み込まれたプレハブを使用
            //return loadedPrefab.name.ToString();
        }
        else
        {
            Debug.LogError("Failed to load model prefab.");
            //return null;
        }
    }

    private void OnPrefabInstantiated(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject instantiatedObject = obj.Result;
            // ここでインスタンス化されたオブジェクトを操作
            instantiatedObject.SetActive(true);
            Debug.Log("model name instantiated successfully: " + instantiatedObject.name);
        }
        else
        {
            Debug.LogError("Failed to instantiate model prefab.");
        }
    }

    private void OnPrefabInstantiatedDelete(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject instantiatedObject = obj.Result;
            // ここでインスタンス化されたオブジェクトを操作
            //instantiatedObject.SetActive(false);
            Destroy(instantiatedObject);
            Debug.Log("model name instantiated successfully: " + instantiatedObject.name);
        }
        else
        {
            Debug.LogError("Failed to instantiate model prefab.");
        }
    }

    private void SelectMovie(string message, string movieName, string trustLevel)
    {

        Debug.Log($"SelectMovie message: {message}, entity: {movieName}, trustLevel: {trustLevel}");
        Debug.Log($"SelectMovie Contains: {message.Contains("HMS")}");
        //debug用
        //string debugText = fileResourcePath + "/" + movieName;
        //resoponseAITextDialogDebug.text = debugText;

        //リソースファイル下のmovie名称から、対象movieを選択
        if (movieName != null && movieName != "movie_name_is_null")
        {

            //XRデバイス環境のパス
            videoFileResourcePath = fileResourcePath + "/" + movieName;
            //unity editor、windows環境のパス
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            videoFileResourcePath = fileResourcePath + movieName;
#endif

            Debug.Log($"SelectMovie fileResourcePath: {fileResourcePath} movieName: {movieName} videoFileResourcePath: {videoFileResourcePath}");
            videoController.PrepareForUrl(videoFileResourcePath);
            MovieObj[0].SetActive(true);
            videoController.Play();
        }
    }

    private void StartCall(string message, string entity, List<string> eachMenberValue, string trustLevel)
    {
        Debug.Log($"StartCall function calling message: {message}, entity: {entity}, eachMenberValue: {eachMenberValue}, trustLevel: {trustLevel}");
        //ACS(遠隔通話)の処理を記載
    }

    private void ShowPDF(string message, string pdfName, string trustLevel)
    {
        Debug.Log($"ShowPDF function calling message: {message}, pdfName: {pdfName}, trustLevel: {trustLevel}");
        //PDFの処理を記載
        //pdfFileResourcePath = fileResourcePath + pdfName;
        //XRデバイス環境のパス
        pdfFileResourcePath = fileResourcePath + "/" + pdfName;
        //debug用
        //resoponseAITextDialogDebug.text = pdfFileResourcePath;
        //unity editor、windows環境のパス
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        pdfFileResourcePath = fileResourcePath + pdfName;
#endif

        Debug.Log($"SelectPDF fileResourcePath: {fileResourcePath} pdfName: {pdfName} pdfFileResourcePath: {pdfFileResourcePath}");
        //fileSearchController.OpenViewer(pdfFileResourcePath);
        fileSearchControllerAI.OpenViewer(pdfFileResourcePath);
        //movie nameで検索して、参照(Linq)
        //debug用
        //var target = PDFWindowObj.FirstOrDefault(x => x.name == pdfName);
        //if (target != null)
        //{
        //    Debug.Log($"pdf name: {target.name}");
        //    //ShowTargetPDF(target);
        //    target.SetActive(true);
        //}
        //else
        //{
        //    Debug.Log("該当の名前のpdfは存在しないため、default設定のpdfを表示");
        //    PDFWindowObj[1].SetActive(true);
        //}
    }

    //Azure AIFoundry：
    //public async Task<string> AnalyzeIntent(string userPrompt)
    //{
    //    var client = new HttpClient();
    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    //    //Foundryにアクセスする際に、headerにmodel名が必要 //deploymentName
    //    //client.DefaultRequestHeaders.Add("x-ms-model-mesh-model-name", "Phi-4-multimodal-instruct");
    //    client.DefaultRequestHeaders.Add("x-ms-model-mesh-model-name", deploymentName);

    //    string uri = $"{endpoint}openai/deployments/{deploymentName}/chat/completions?api-version=2024-02-15-preview";

    //    var messages = new List<ChatMessageFoundty>
    //    {
    //        new ChatMessageFoundty { role = "system", content = $"ユーザーの発言から意図を抽出して、JSON形式で返してください。JSON形式は以下のフォーマットにしてください。{ResponseJsonFormat}" },
    //        new ChatMessageFoundty { role = "user", content = userPrompt }
    //    };

    //    var requestData = new ChatRequestFoundty
    //    {
    //        messages = messages,
    //        max_tokens = 6000,
    //        temperature = 0f,
    //        model = GPTModel
    //    };

    //    string json = JsonUtility.ToJson(requestData);
    //    var content = new StringContent(json, Encoding.UTF8, "application/json");

    //    //レスポンスが受け取れなかった場合の処理の追加
    //    try
    //    {
    //        var response = await client.PostAsync(uri, content);
    //        string result = await response.Content.ReadAsStringAsync();

    //        Debug.Log("Raw response: " + result);

    //        ChatResponseFoundty chatResponse = JsonUtility.FromJson<ChatResponseFoundty>(result);

    //        if (chatResponse.choices != null && chatResponse.choices.Count > 0)
    //        {
    //            return chatResponse.choices[0].message.content;
    //        }
    //        else
    //        {
    //            Debug.LogError("AI response has no choices.");
    //            return "エラー: AIの応答が空です。";
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError("Error calling Azure AI: " + e.Message);
    //        return "エラー: Azure AIとの通信中に例外が発生しました。";
    //    }
    //}

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

    //voice recorder
    public async void StartRecording()
    {
        DisappearAllObj();
        await SpeakText("ご用件をお伺いいたします。");
        Debug.Log("録音開始");
        SoundEffect(audioClip[0]);
        recordedClip = Microphone.Start(null, false, maxRecordingTime, 44100);

    }

    public void StopRecording()
    {
        //if (Microphone.IsRecording(null))
        Microphone.End(null);
        Debug.Log("録音終了");
        SoundEffect(audioClip[1]);

        // AudioClip → WAV ファイルに変換
        byte[] wavData = WavUtility.FromAudioClip(recordedClip);
        File.WriteAllBytes(filePath, wavData);

        Debug.Log("WAVファイル保存完了: " + filePath);

        // API送信処理呼び出し
        StartCoroutine(UploadAudio(filePath));

    }

    private void StopRecordingUploadAudio()
    {
        StartCoroutine(UploadAudio(filePath));
    }

    //add 250623 APIサーバーへのリクエストパラメーターにAzure speechから返却されたSTTデータを挿入
    private async void AzureSpeachRequestParamSTT()
    {
        var config = SpeechConfig.FromSubscription(AzureAISpeechKey, AzureAISpeechRegion);
        config.SpeechRecognitionLanguage = "ja-JP";

        //org記載事項(必要ならコメント外す)
        //using var recognizer = new Microsoft.CognitiveServices.Speech.SpeechRecognizer(config);
        //検証用追記
        recognizer = new SpeechRecognizer(config);

        Debug.Log("音声入力を開始してください...");
        //await SpeakText("音声入力を開始してください");
        var result = await recognizer.RecognizeOnceAsync();

        Debug.Log($"AzureSpeachRequestParamSTT: {result}");
        Debug.Log($"AzureSpeachRequestParamSTT 認識結果: {result.Text}");

    }

    //音声をwavファイルにてAPIサーバーに送信
    IEnumerator UploadAudio(string filePath)
    {
        Debug.Log("UploadAudio");
        // ファイルが存在するか確認
        if (!File.Exists(filePath))
        {
            Debug.LogError("音声ファイルが見つかりません: " + filePath);
            yield break;
        }

        byte[] audioData = File.ReadAllBytes(filePath);

        // フォームデータを構築
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", audioData, "recorded.wav", "audio/wav");
        form.AddField("input_speech_type", "text");

        UnityWebRequest request = UnityWebRequest.Post(apiUrl, form);

        yield return request.SendWebRequest();

        Debug.Log($"UploadAudio request result: {request.result}");
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("レスポンス: " + request.downloadHandler.text);
            ParseAPIServerResponseData(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("エラー: " + request.responseCode + " - " + request.error);
            Debug.LogError("レスポンス本文: " + request.downloadHandler.text);
            //レスポンスにAPIサーバー側からの応答内容に関するエラーが含まれていた場合、hangしてしまうため、元の状態に戻す。
            demoManager.SetState(DemoManager.DemoState.Top);
        }
    }

    //音声をSTTにてAPIサーバーに送信
    IEnumerator UploadAudioSTT(string filePath, string sendMessage)
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
        //form.AddBinaryData("file", audioData, "recorded.wav", "audio/wav");

        form.AddField("input_speech_type", "text");

        UnityWebRequest request = UnityWebRequest.Post(apiUrl, form);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("レスポンス: " + request.downloadHandler.text);
            ParseAPIServerResponseData(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("エラー: " + request.responseCode + " - " + request.error);
            Debug.LogError("レスポンス本文: " + request.downloadHandler.text);
            //レスポンスにAPIサーバー側からの応答内容に関するエラーが含まれていた場合、hangしてしまうため、元の状態に戻す。
            demoManager.SetState(DemoManager.DemoState.Top);
        }
    }

    //APIサーバーからresponseされるjsonデータのparse (防御的に再実装)
    private async void ParseAPIServerResponseData(string json)
    {
        Debug.Log($"[API Raw JSON length={json?.Length}]");
        if (string.IsNullOrWhiteSpace(json))
        {
            Debug.LogWarning("APIレスポンスが空です。");
            await SpeakText("応答が取得できませんでした。もう一度お願いします。");
            demoManager.SetState(DemoManager.DemoState.Top);
            return;
        }

        if (!json.TrimStart().StartsWith("{"))
        {
            Debug.LogError("JSON形式でないレスポンス: head=" + json.Substring(0, Mathf.Min(120, json.Length)));
            await SpeakText("形式不正な応答です。もう一度お願いします。");
            demoManager.SetState(DemoManager.DemoState.Top);
            return;
        }

        JObject root;
        try { root = JObject.Parse(json); }
        catch (JsonException ex)
        {
            Debug.LogError($"JSONパース失敗: {ex.Message}");
            await SpeakText("解析に失敗しました。もう一度お願いします。");
            demoManager.SetState(DemoManager.DemoState.Top);
            return;
        }

        var functionsToken = root["functions"];
        if (functionsToken == null)
        {
            Debug.LogWarning("functions キーがありません。");
            await SpeakText("応答が不完全です。もう一度お願いします。");
            demoManager.SetState(DemoManager.DemoState.Top);
            return;
        }

        var functionsArray = functionsToken as JArray ?? new JArray(functionsToken);
        if (functionsArray.Count == 0)
        {
            Debug.LogWarning("functions が空です。");
            await SpeakText("応答が空でした。もう一度お願いします。");
            demoManager.SetState(DemoManager.DemoState.Top);
            return;
        }

        foreach (var f in functionsArray)
        {
            if (f.Type != JTokenType.Object) continue;
            var funcObj = (JObject)f;

            string message   = funcObj.Value<string>("message");
            int    orderType = funcObj.Value<int?>("order_type") ?? -1;
            string toolName  = funcObj.Value<string>("functiontool_name");
            var responseToken = funcObj["functiontool_response"];

            // JObject でない場合（文字列 / 数値 / null）を検出
            if (responseToken is not JObject respObj)
            {
                Debug.LogWarning($"functiontool_response が期待形式(JObject)でない: type={responseToken?.Type}");
                // この場合キー付きの詳細は取得不能なのでスキップ継続
                if (orderType == 0 && !string.IsNullOrEmpty(message))
                {
                    uiController.StateControlReply();
                    showTextWindow.Open();
                    showTextWindow.SetText(message);
                    await SpeakText(message);
                }
                continue;
            }

            // 抽出
            string movieNameValue      = ExtractValue(respObj, "movie_name", out string movieTrust);
            string modelNameValue      = ExtractValue(respObj, "model_name", out string modelTrust);
            string pdfNameValue        = ExtractValue(respObj, "pdf_name", out string pdfTrust);
            string startCallValue      = ExtractValue(respObj, "start_call", out string startCallTrust);
            string showPdfValue        = ExtractValue(respObj, "show_pdf", out string showPdfTrust);
            string questionPromptValue = ExtractDirectString(respObj, "question_prompt");
            List<string> membersList   = ExtractMembers(respObj, out string membersTrust);

            Debug.Log($"[Function] tool={toolName} orderType={orderType} message='{message}'");

            if (orderType == 0 && !string.IsNullOrEmpty(message))
            {
                uiController.StateControlReply();
                showTextWindow.Open();
                showTextWindow.SetText(message);
                await SpeakText(message);
            }

            if (orderType == 1)
            {
                switch (toolName)
                {
                    case "show_3d_model":
                        uiController.StateControlTop();
                        await SpeakText(message ?? "モデルを表示します。");
                        XRModelControl(message, modelNameValue ?? "model_name_is_null", modelTrust ?? "trust_level_is_null");
                        break;
                    case "show_movie":
                        uiController.StateControlTop();
                        await SpeakText(message ?? "動画を表示します。");
                        SelectMovie(message, string.IsNullOrEmpty(movieNameValue) ? "movie_name_is_null" : movieNameValue, movieTrust ?? "trust_level_is_null");
                        break;
                    case "show_pdf":
                        uiController.StateControlTop();
                        await SpeakText(message ?? "PDFを表示します。");
                        if (!string.IsNullOrEmpty(pdfNameValue))
                            ShowPDF(message, pdfNameValue, pdfTrust ?? "trust_level_is_null");
                        break;
                    case "start_call":
                        uiController.StateControlTop();
                        await SpeakText(message ?? "通話を開始します。");
                        StartCall(message, membersList?.FirstOrDefault(), membersList ?? new List<string>(), membersTrust ?? "trust_level_is_null");
                        break;
                    case "question_detail":
                        uiController.StateControlTop();
                        await SpeakText(message ?? questionPromptValue ?? "詳細をお伝えします。");
                        break;
                    default:
                        Debug.Log($"未対応 toolName={toolName}");
                        break;
                }
            }
        }
    }

    // JToken は必ず JObject 前提に変更（呼び出し側で保証）
    private string ExtractValue(JToken responseObj, string key, out string trustLevel)
    {
        trustLevel = null;
        if (responseObj is not JObject obj) return null;

        var token = obj[key];
        if (token == null) return null;

        if (token.Type == JTokenType.String)
        {
            return token.Value<string>();
        }
        if (token.Type == JTokenType.Object)
        {
            var inner = (JObject)token;
            trustLevel = inner.Value<string>("trust_level");
            var v = inner["value"];
            if (v == null) return null;
            return v.Type == JTokenType.String ? v.Value<string>() : v.ToString();
        }
        return token.ToString();
    }

    private string ExtractDirectString(JToken responseObj, string key)
    {
        if (responseObj is not JObject obj) return null;
        var t = obj[key];
        return t?.Type == JTokenType.String ? t.Value<string>() : null;
    }

    private List<string> ExtractMembers(JToken responseObj, out string trustLevel)
    {
        trustLevel = null;
        if (responseObj is not JObject obj) return null;
        var m = obj["members"];
        if (m == null) return null;

        if (m.Type == JTokenType.Object)
        {
            var inner = (JObject)m;
            trustLevel = inner.Value<string>("trust_level");
            var valueToken = inner["value"];
            if (valueToken is JArray arr)
            {
                return arr.Values<string>().Where(s => !string.IsNullOrEmpty(s)).ToList();
            }
            if (valueToken?.Type == JTokenType.String)
            {
                return new List<string> { valueToken.Value<string>() };
            }
            return null;
        }
        if (m is JArray directArr)
        {
            return directArr.Values<string>().Where(s => !string.IsNullOrEmpty(s)).ToList();
        }
        if (m.Type == JTokenType.String)
        {
            return new List<string> { m.Value<string>() };
        }
        return null;
    }

    // 効果音再生（存在チェック付き）
    private void SoundEffect(AudioClip clip)
    {
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource が設定されていません。効果音を再生できません。");
            return;
        }
        if (clip == null)
        {
            Debug.LogWarning("AudioClip が null です。効果音を再生できません。");
            return;
        }
        audioSource.clip = clip;
        audioSource.Play();
    }
}
