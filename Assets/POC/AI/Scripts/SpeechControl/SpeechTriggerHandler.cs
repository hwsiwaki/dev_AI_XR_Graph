using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SpeechTriggerHandler : MonoBehaviour
{
    private SpeechRecognizer recognizer;
    private bool isRecognizing = false;

    // Azure Speech API設定
    [SerializeField] private string azureApiKey = "15d2a377321445018ec8b47605ad2ef2";
    [SerializeField] private string azureRegion = "japaneast"; // 例: "japanwest"
    //[SerializeField] private static readonly String AzureAISpeechKey = "15d2a377321445018ec8b47605ad2ef2";
    //[SerializeField] private static readonly String AzureAISpeechRegion = "japaneast";

    // FastAPIエンドポイント
    private string apiEndpoint = "https://dev-aiasst-app01-g0g3ckcnfncpbuf0.japanwest-01.azurewebsites.net/question";

    //既存処理連携
    public UIController uiController;
    public UnityEngine.UI.Button uiControllerButton;

    async void Start()
    {
        await StartRecognition();
    }

    private async Task StartRecognition()
    {
        var config = SpeechConfig.FromSubscription(azureApiKey, azureRegion);
        config.SpeechRecognitionLanguage = "ja-JP";

        recognizer = new SpeechRecognizer(config);

        recognizer.Recognized += async (s, e) =>
        {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                string recognizedText = e.Result.Text;
                Debug.Log($"認識結果: {recognizedText}");

                if (recognizedText.Contains("ヤッテミー") 
                || recognizedText.ToLower().Contains("ya temi") 
                || recognizedText.Contains("やあ。テミー。") 
                || recognizedText.Contains("いや、テミー。")
                || recognizedText.Contains("やってみ")
                || recognizedText.Contains("ティミ")
                || recognizedText.Contains("やあ。テミン。"))
                {
                    Debug.Log("キーワード検出：「ヤッテミー」または「Ya, Temi」");
                    await StopRecognition(); // 認識停止
                    //await SendToApiServer(recognizedText); // API通信
                    AIVoiceConnect();
                }
            }
        };

        await recognizer.StartContinuousRecognitionAsync();
        isRecognizing = true;
        Debug.Log("常時音声認識を開始しました");
    }

    private void AIVoiceConnect()
    {
        
        try
        {
            Debug.Log("AIVoiceConnect");
            //uiController.OnStartButton();
            uiControllerButton.onClick.Invoke();
        }
        catch
        {
            Debug.Log("AIVoiceConnect error");
            Debug.Assert(false);
        }
    }

    private async Task StopRecognition()
    {
        if (isRecognizing && recognizer != null)
        {
            await recognizer.StopContinuousRecognitionAsync();
            isRecognizing = false;
            Debug.Log("音声認識を停止しました");
        }
    }

    private async Task SendToApiServer(string recognizedText)
    {
        try
        {
            var payload = new { question = recognizedText };
            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = new HttpClient();
            var response = await client.PostAsync(apiEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                ParseAndHandleResponse(responseBody);
            }
            else
            {
                Debug.LogWarning($"APIエラー: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"API通信中にエラー: {ex.Message}");
        }
    }

    private void ParseAndHandleResponse(string json)
    {
        try
        {
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            string message = root.GetProperty("message").GetString();
            int orderType = root.GetProperty("order_type").GetInt32();
            string functionTool = root.GetProperty("functiontool_name").GetString();
            string toolResponse = root.GetProperty("functiontool_response").GetString();
            string intent = root.GetProperty("intent").GetString();

            var entities = new List<string>();
            foreach (var e in root.GetProperty("entities").EnumerateArray())
            {
                entities.Add(e.GetString());
            }

            Debug.Log($"API応答：message={message}");
            Debug.Log($"intent={intent}, entities=[{string.Join(", ", entities)}]");
            Debug.Log($"tool={functionTool}, tool_response={toolResponse}");

            // 🎯 任意のアクション実装場所（UI更新、TTS再生など）
            // ShowUI(message);
            // PlayAudio(intent);
        }
        catch (Exception ex)
        {
            Debug.LogError($"JSON解析失敗: {ex.Message}");
        }
    }

    private async void OnDestroy()
    {
        await StopRecognition();
        recognizer?.Dispose();
    }
}
