using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnalyticsDashboard : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private TextMeshProUGUI biologyScore;
    [SerializeField] private TextMeshProUGUI chemistryScore;
    [SerializeField] private TextMeshProUGUI physicsScore;
    [SerializeField] private TextMeshProUGUI totalScore;

    [Header("Progress")]
    [SerializeField] private Slider biologyBar;
    [SerializeField] private Slider chemistryBar;
    [SerializeField] private Slider physicsBar;
    [SerializeField] private Slider totalBar;

    [Header("Summary")]
    [SerializeField] private TextMeshProUGUI strengthLabel;
    [SerializeField] private TextMeshProUGUI weaknessLabel;
    [SerializeField] private TextMeshProUGUI tipLabel;
    [SerializeField] private TextMeshProUGUI overallGradeLabel;

    [Header("Agent")]
    [SerializeField] private ScienceAgent agent;

    public static float BiologyScore { get; set; }
    public static float ChemistryScore { get; set; }
    public static float PhysicsScore { get; set; }
    public static AnalyticsDashboard Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateDashboard(0, 0, 0);
    }

    public void AddScore(string subject, int score, int total)
    {
        if (total == 0) return;
        float percentage = ((float)score / (float)total) * 100f;
        switch (subject)
        {
            case "Biology": BiologyScore = percentage; break;
            case "Chemistry": ChemistryScore = percentage; break;
            case "Physics": PhysicsScore = percentage; break;
        }
        UpdateDashboard(BiologyScore, ChemistryScore, PhysicsScore);
    }

    public void UpdateScores(string subject, float score)
    {
        switch (subject)
        {
            case "Biology": BiologyScore = score; break;
            case "Chemistry": ChemistryScore = score; break;
            case "Physics": PhysicsScore = score; break;
        }
        UpdateDashboard(BiologyScore, ChemistryScore, PhysicsScore);
    }

    private void UpdateDashboard(float bio, float chem, float phys)
    {
        if (biologyScore) biologyScore.text = "Biology: " + bio.ToString("F0") + "%";
        if (chemistryScore) chemistryScore.text = "Chemistry: " + chem.ToString("F0") + "%";
        if (physicsScore) physicsScore.text = "Physics: " + phys.ToString("F0") + "%";

        if (biologyBar) biologyBar.value = bio / 100f;
        if (chemistryBar) chemistryBar.value = chem / 100f;
        if (physicsBar) physicsBar.value = phys / 100f;

        float overall = (bio + chem + phys) / 3f;

        if (totalScore) totalScore.text = "Overall: " + overall.ToString("F0") + "%";
        if (totalBar) totalBar.value = overall / 100f;

        if (overallGradeLabel)
        {
            if (overall >= 90) overallGradeLabel.text = "Overall Grade: A";
            else if (overall >= 80) overallGradeLabel.text = "Overall Grade: B";
            else if (overall >= 70) overallGradeLabel.text = "Overall Grade: C";
            else if (overall >= 60) overallGradeLabel.text = "Overall Grade: D";
            else overallGradeLabel.text = "Overall Grade: F";
        }

        float[] scores = new float[] { bio, chem, phys };
        string[] subjects = new string[] { "Biology", "Chemistry", "Physics" };
        int bestIndex = 0;
        int worstIndex = 0;

        for (int i = 1; i < scores.Length; i++)
        {
            if (scores[i] > scores[bestIndex]) bestIndex = i;
            if (scores[i] < scores[worstIndex]) worstIndex = i;
        }

        if (strengthLabel) strengthLabel.text = "Strength: " + subjects[bestIndex];
        if (weaknessLabel) weaknessLabel.text = "Weakness: " + subjects[worstIndex];
        if (tipLabel) tipLabel.text = "Tip: Focus on improving " + subjects[worstIndex] + " to boost your overall performance!";

        if (bio > 0 && chem > 0 && phys > 0 && agent != null)
            agent.WalkToStation("Analytics");
    }
}