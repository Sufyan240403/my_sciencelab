using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MCQManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Questions and Answers")]
    [SerializeField] private string[] questions;
    [SerializeField] private string[] optionA;
    [SerializeField] private string[] optionB;
    [SerializeField] private string[] optionC;
    [SerializeField] private string[] optionD;
    [SerializeField] private int[] correctAnswer;

    [Header("Analytics")]
    [SerializeField] private AnalyticsDashboard analyticsDashboard;
    [SerializeField] private string subjectName;

    private int currentQuestion = 0;
    private int score = 0;
    private bool quizFinished = false;

    private void Start()
    {
        if (quizPanel != null)
            quizPanel.SetActive(true);
        LoadQuestion();
    }

    private void LoadQuestion()
    {
        if (questions == null || questions.Length == 0) return;
        if (currentQuestion >= questions.Length) return;
        if (optionA == null || optionA.Length <= currentQuestion) return;
        if (optionB == null || optionB.Length <= currentQuestion) return;
        if (optionC == null || optionC.Length <= currentQuestion) return;
        if (optionD == null || optionD.Length <= currentQuestion) return;
        if (correctAnswer == null || correctAnswer.Length <= currentQuestion) return;

        quizFinished = false;

        if (questionText != null)
            questionText.text = "Q" + (currentQuestion + 1) + ": " + questions[currentQuestion];

        if (feedbackText != null)
            feedbackText.text = "";

        string[] options = new string[]
        {
            optionA[currentQuestion],
            optionB[currentQuestion],
            optionC[currentQuestion],
            optionD[currentQuestion]
        };

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] != null)
            {
                answerButtons[i].interactable = true;

                TextMeshProUGUI btnText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                    btnText.text = options[i];

                int index = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => OnAnswerClicked(index));
            }
        }
    }

    private void OnAnswerClicked(int index)
    {
        if (quizFinished) return;

        foreach (Button btn in answerButtons)
            if (btn != null) btn.interactable = false;

        string correctAnswerText = GetAnswerText(correctAnswer[currentQuestion], currentQuestion);

        if (index == correctAnswer[currentQuestion])
        {
            score++;
            if (feedbackText != null)
                feedbackText.text = "Correct! The answer is: " + correctAnswerText;
        }
        else
        {
            if (feedbackText != null)
                feedbackText.text = "Incorrect! The correct answer is: " + correctAnswerText;
        }

        currentQuestion++;

        if (currentQuestion < questions.Length)
            Invoke("LoadQuestion", 2f);
        else
            Invoke("FinishQuiz", 2f);
    }

    private void FinishQuiz()
    {
        quizFinished = true;

        if (analyticsDashboard != null)
            analyticsDashboard.AddScore(subjectName, score, questions.Length);

        if (feedbackText != null)
            feedbackText.text = "Quiz complete! You scored " + score + "/" + questions.Length + ". Go to Analytics to check your score!";

        if (questionText != null)
            questionText.text = "Q1: " + questions[0];

        if (optionA == null || optionA.Length == 0) return;

        string[] options = new string[]
        {
            optionA[0],
            optionB[0],
            optionC[0],
            optionD[0]
        };

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] != null)
            {
                TextMeshProUGUI btnText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                    btnText.text = options[i];

                answerButtons[i].interactable = true;

                int index = i;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => OnAnswerClicked(index));
            }
        }

        currentQuestion = 0;
        score = 0;
    }

    private string GetAnswerText(int answerIndex, int questionIndex)
    {
        switch (answerIndex)
        {
            case 0: return optionA[questionIndex];
            case 1: return optionB[questionIndex];
            case 2: return optionC[questionIndex];
            case 3: return optionD[questionIndex];
            default: return "";
        }
    }

    public void OpenQuiz()
    {
        if (quizPanel != null)
            quizPanel.SetActive(true);
        currentQuestion = 0;
        score = 0;
        quizFinished = false;
        LoadQuestion();
    }

    public void CloseQuiz()
    {
        if (quizPanel != null)
            quizPanel.SetActive(false);
    }
}