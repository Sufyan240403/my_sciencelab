using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeScreenManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject homePanel;
    [SerializeField] private GameObject loadingPanel;

    [Header("Loading UI")]
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TextMeshProUGUI loadingText;

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button logOffButton;
    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonPressed);
        logOffButton.onClick.AddListener(OnLogOffButtonPressed);

        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }
    public void OnStartButtonPressed()
    {
        startButton.interactable = false;
        logOffButton.interactable = false;
        StartCoroutine(LoadLabScene());
    }
    public void OnLogOffButtonPressed()
    {
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
    private IEnumerator LoadLabScene()
    {
        homePanel.SetActive(false);
        loadingPanel.SetActive(true);
        AsyncOperation op =SceneManager.LoadSceneAsync("Science_Lab");
        op.allowSceneActivation = false;
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            if (loadingBar != null)
                loadingBar.value = progress;
            if (loadingText != null)
            {
                if (progress < 0.4f)
                    loadingText.text = "Loading assets...";
                else if (progress < 0.7f)
                    loadingText.text = "Setting up environment...";
                else
                    loadingText.text = "Finalizing...";
            }
            if (op.progress >= 0.9f)
            {

                yield return new WaitForSeconds(1.5f);
                op.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
