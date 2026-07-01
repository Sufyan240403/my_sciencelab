using UnityEngine;

public class WristUI : MonoBehaviour
{
    [Header("Sub-panels")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Spatial Display Canvas (BBC + AI combined)")]
    [SerializeField] private AskAI spatialDisplay;

    [Header("Quiz Canvases")]
    [SerializeField] private GameObject biologyQuizCanvas;
    [SerializeField] private GameObject chemistryQuizCanvas;
    [SerializeField] private GameObject physicsQuizCanvas;
    [SerializeField] private GameObject analyticsCanvas;

    private void Start()
    {
        SafeSetActive(settingsPanel, false);
    }

    public void OnSettingsButtonClicked()
    {
        if (settingsPanel == null)
        {
            Debug.LogError("[WristUI] SettingsPanel not assigned!");
            return;
        }
        bool isOpen = settingsPanel.activeSelf;
        settingsPanel.SetActive(!isOpen);
    }

    public void OnSettings()
    {
        OnSettingsButtonClicked();
    }

    public void OnAskAIButtonClicked()
    {
        if (spatialDisplay == null)
        {
            Debug.LogError("[WristUI] SpatialDisplay not assigned!");
            return;
        }
        spatialDisplay.OpenSpatialDisplay();
        spatialDisplay.ShowAITab();
    }

    public void OnBBCButtonClicked()
    {
        if (spatialDisplay == null)
        {
            Debug.LogError("[WristUI] SpatialDisplay not assigned!");
            return;
        }
        spatialDisplay.OpenSpatialDisplay();
        spatialDisplay.ShowBBCTab();
    }

    public void OnBiologyButtonClicked()
    {
        ShowQuiz(biologyQuizCanvas, "Biology");
    }

    public void OnChemistryButtonClicked()
    {
        ShowQuiz(chemistryQuizCanvas, "Chemistry");
    }

    public void OnPhysicsButtonClicked()
    {
        ShowQuiz(physicsQuizCanvas, "Physics");
    }

    public void OnAnalyticsButtonClicked()
    {
        ShowQuiz(analyticsCanvas, "Analytics");
    }

    public void OnAgentIntroButtonClicked()
    {
        Debug.Log("[WristUI] Agent Intro button pressed.");
    }

    public void OnExitButtonClicked()
    {
        Debug.Log("[WristUI] Exit button pressed.");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Home_Screen");
    }

    private void ShowQuiz(GameObject canvas, string label)
    {
        if (canvas == null)
        {
            Debug.LogWarning("[WristUI] " + label + " canvas not assigned!");
            return;
        }
        canvas.SetActive(true);
    }

    private static void SafeSetActive(GameObject go, bool state)
    {
        if (go != null) go.SetActive(state);
    }
}