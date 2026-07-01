using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Toggle musicToggle;

    [Header("Lights")]
    [SerializeField] private Light biologyLight;
    [SerializeField] private Light chemistryLight;
    [SerializeField] private Light physicsLight;
    [SerializeField] private Light analyticsLight;
    [SerializeField] private Light directionalLight;

    private void Start()
    {
        // Hide settings panel on start
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // Set default slider values
        if (volumeSlider != null)
        {
            volumeSlider.value = 1f;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (brightnessSlider != null)
        {
            brightnessSlider.value = 1f;
            brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
        }

        if (musicToggle != null)
        {
            musicToggle.isOn = true;
            musicToggle.onValueChanged.AddListener(OnMusicToggled);
        }
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
        Debug.Log("[GameSettings] Settings opened.");
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        Debug.Log("[GameSettings] Settings closed.");
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        Debug.Log("[GameSettings] Volume set to " + value);
    }

    public void OnBrightnessChanged(float value)
    {
        RenderSettings.ambientIntensity = value;

        if (directionalLight != null)
            directionalLight.intensity = value;
        if (biologyLight != null)
            biologyLight.intensity = value * 3f;
        if (chemistryLight != null)
            chemistryLight.intensity = value * 3f;
        if (physicsLight != null)
            physicsLight.intensity = value * 3f;
        if (analyticsLight != null)
            analyticsLight.intensity = value * 3f;

        Debug.Log("[GameSettings] Brightness set to " + value);
    }

    public void OnMusicToggled(bool value)
    {
        if (volumeSlider != null)
            AudioListener.volume = value ? volumeSlider.value : 0f;
        Debug.Log("[GameSettings] Music " + (value ? "ON" : "OFF"));
    }

    public void OnResetProgress()
    {
        // Reset sliders
        if (volumeSlider != null) volumeSlider.value = 1f;
        if (brightnessSlider != null) brightnessSlider.value = 1f;
        if (musicToggle != null) musicToggle.isOn = true;

        // Reset audio
        AudioListener.volume = 1f;

        // Reset analytics scores
        if (AnalyticsDashboard.Instance != null)
        {
            AnalyticsDashboard.BiologyScore = 0f;
            AnalyticsDashboard.ChemistryScore = 0f;
            AnalyticsDashboard.PhysicsScore = 0f;
            AnalyticsDashboard.Instance.UpdateScores("Biology", 0f);
            AnalyticsDashboard.Instance.UpdateScores("Chemistry", 0f);
            AnalyticsDashboard.Instance.UpdateScores("Physics", 0f);
        }

        Debug.Log("[GameSettings] All settings reset.");
    }
}