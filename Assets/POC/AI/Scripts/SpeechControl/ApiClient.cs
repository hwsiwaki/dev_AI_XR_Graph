using System.Text;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;


[System.Serializable]
public class QuestionPayload
{
    public string file;   // ← APIが期待するキー名
    public string input = "text";  // デフォルトで"text"
}



//public class ApiClient : MonoBehaviour
//{
//    string apiBaseUrl = "https://dev-aiasst-app01-g0g3ckcnfncpbuf0.japanwest-01.azurewebsites.net";

//    void Start()
//    {
//        StartCoroutine(CallRootEndpoint());
//    }

//    IEnumerator CallRootEndpoint()
//    {
//        UnityWebRequest request = UnityWebRequest.Get(apiBaseUrl + "/Root");
//        yield return request.SendWebRequest();

//        if (request.result == UnityWebRequest.Result.Success)
//        {
//            Debug.Log("GET /Root response: " + request.downloadHandler.text);
//        }
//        else
//        {
//            Debug.LogError("GET /Root error: " + request.error);
//        }
//    }
//}


public class ApiClient : MonoBehaviour
{
    string apiBaseUrl = "https://dev-aiasst-app01-g0g3ckcnfncpbuf0.japanwest-01.azurewebsites.net";

    void Start()
    {
        StartCoroutine(PostQuestion("こんにちは、今日の天気は？"));
    }

    IEnumerator PostQuestion(string userInput)
    {
        QuestionPayload payload = new QuestionPayload
        {
            file = userInput,
            input = "text"
        };

        string jsonData = JsonUtility.ToJson(payload);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(apiBaseUrl + "/question", "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("POST /question response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("POST /question error: " + request.responseCode + " - " + request.error);
            Debug.LogError("Response body: " + request.downloadHandler.text);
        }
    }
}
