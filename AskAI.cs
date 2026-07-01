using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

[System.Serializable]
public class GeminiPart { public string text; }
[System.Serializable]
public class GeminiContent { public GeminiPart[] parts; }
[System.Serializable]
public class GeminiRequest { public GeminiContent[] contents; }
[System.Serializable]
public class GeminiResponsePart { public string text; }
[System.Serializable]
public class GeminiResponseContent { public GeminiResponsePart[] parts; }
[System.Serializable]
public class GeminiCandidate { public GeminiResponseContent content; }
[System.Serializable]
public class GeminiResponse { public GeminiCandidate[] candidates; }

public class AskAI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject aiPanel;
    [SerializeField] private GameObject bbcPanel;
    [SerializeField] private GameObject spatialDisplayCanvas;

    [Header("Tab Buttons")]
    [SerializeField] private Button aiTabButton;
    [SerializeField] private Button bbcTabButton;

    [Header("AI UI")]
    [SerializeField] private TMP_InputField questionInput;
    [SerializeField] private TextMeshProUGUI responseText;

    [Header("BBC UI")]
    [SerializeField] private TextMeshProUGUI bbcHeadlineText;

    private string apiKey = "AIzaSyDG7JnfXlvW_AYRUG95yGTY9J3p-Hy31Mw";
    private string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent";

    void Start()
    {
        // Start with AI tab open by default
        ShowAITab();
        if (spatialDisplayCanvas != null)
            spatialDisplayCanvas.SetActive(false);
    }

    // Called by WristUI BBCButton or AskAiButton
    public void OpenSpatialDisplay()
    {
        if (spatialDisplayCanvas != null)
            spatialDisplayCanvas.SetActive(true);
        ShowAITab();
    }

    public void CloseSpatialDisplay()
    {
        if (spatialDisplayCanvas != null)
            spatialDisplayCanvas.SetActive(false);
    }

    public void ShowAITab()
    {
        aiPanel.SetActive(true);
        bbcPanel.SetActive(false);
    }

    public void ShowBBCTab()
    {
        aiPanel.SetActive(false);
        bbcPanel.SetActive(true);
        LoadBBCHeadlines();
    }

    public void OnAskButton()
    {
        if (string.IsNullOrEmpty(questionInput.text)) return;
        StartCoroutine(SendQuestion(questionInput.text));
    }

    private IEnumerator SendQuestion(string question)
    {
        responseText.text = "Thinking...";

        GeminiRequest request = new GeminiRequest
        {
            contents = new GeminiContent[]
            {
                new GeminiContent
                {
                    parts = new GeminiPart[]
                    {
                        new GeminiPart
                        {
                            text = "You are a science teacher assistant. Answer this science question simply and clearly for a student: " + question
                        }
                    }
                }
            }
        };

        string jsonBody = JsonUtility.ToJson(request);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        string fullUrl = apiUrl + "?key=" + apiKey;

        UnityWebRequest www = new UnityWebRequest(fullUrl, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            GeminiResponse response = JsonUtility.FromJson<GeminiResponse>(www.downloadHandler.text);
            if (response.candidates != null && response.candidates.Length > 0)
                responseText.text = response.candidates[0].content.parts[0].text;
            else
                responseText.text = "No response received.";
        }
        else
        {
            responseText.text = "Error: " + www.error;
        }
    }

    private void LoadBBCHeadlines()
    {
        // Placeholder BBC content - displays static science news
        if (bbcHeadlineText != null)
        {
            bbcHeadlineText.text =
                "BBC Science News\n\n" +
                "- Scientists discover new species in deep ocean\n\n" +
                "- James Webb telescope captures earliest galaxy\n\n" +
                "- Breakthrough in cancer research announced\n\n" +
                "- Climate change affects Arctic ice levels\n\n" +
                "- New AI model predicts protein structures";
        }
    }
}