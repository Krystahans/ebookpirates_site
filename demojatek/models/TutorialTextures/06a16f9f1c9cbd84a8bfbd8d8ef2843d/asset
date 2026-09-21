using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour
{
    public static QuizManager Instance { get; private set; }

    [Header("UI Elements")]
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public GameObject quizPanel;
    public bool IsQuizActive = false;
    public bool showQuiz;
    private TopBarManager topBarManager => TopBarManager.Instance;
    private List<QuizDBManager.QuizQuestion> availableQuestions;
    private QuizDBManager.QuizQuestion currentQuestion;
    private System.Action onQuizCloseCallback;

    void Start()
    {
        Debug.Log("QuizManager Start: Initializing QuizManager.");
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        if (QuizDBManager.Instance != null && QuizDBManager.Instance.Questions.Count > 0)
        {
            // Load all questions into available list
            availableQuestions = new List<QuizDBManager.QuizQuestion>(QuizDBManager.Instance.Questions);
        }
        else
        {
            availableQuestions = new List<QuizDBManager.QuizQuestion>();
        }
        
        if(quizPanel != null) quizPanel.SetActive(false);
    }

    public void QuizPanelEnabled()
    {
        // Calculate chance based on PickupCount
        int pickupIndex = 0;
        if (topBarManager != null)
        {
            pickupIndex = topBarManager.PickupCount + 1; // We are checking for the upcoming pickup action
        }

        float chance;

        if (pickupIndex < 6)
        {
            chance = 0.0f;
        }
        else if (pickupIndex == 6)
        {
            chance = 1.0f; // Guaranteed quiz on 6th pickup action
        }
        else
        {
            int currentItems = topBarManager != null ? topBarManager.Stored + topBarManager.Carried : 0;
            chance = (float)availableQuestions.Count / (30 - currentItems);
            Debug.Log($"availableQuestions: {availableQuestions.Count}, currentItems: {currentItems}, pickupIndex: {pickupIndex}, Chance: {chance}");
        }

        showQuiz = false;
        if (availableQuestions.Count > 0)
        {
            if (chance >= 1f) showQuiz = true;
            else if (chance <= 0f) showQuiz = false;
            else showQuiz = Random.value < chance;
        }

        if (showQuiz)
        {
            ActivateQuiz();
        }
        else
        {
            // No quiz, just give the item
            if (topBarManager != null)
            {
                topBarManager.AddCarried();
            }
            // Ensure panel is closed
            if(quizPanel.activeSelf) CancelQuiz();
        }
    }

    public void ActivateQuiz()
    {
        if (MonologManager.Instance != null && MonologManager.Instance.IsMonologOn)
        {
            // A monolog/popup is active. Queue this quiz to show after.
            MonologManager.Instance.AddOnCloseCallback(ActivateQuiz);
            return;
        }

        IsQuizActive = true;
        quizPanel.SetActive(true);
        Time.timeScale = 0f;
        if (CameraMove.Instance != null) CameraMove.Instance.lockState(true); 
        
        LoadRandomQuestion();
    }

    private void LoadRandomQuestion()
    {
        if (availableQuestions == null || availableQuestions.Count == 0)
        {
            CancelQuiz();
            return;
        }

        int randomIndex = Random.Range(0, availableQuestions.Count);
        currentQuestion = availableQuestions[randomIndex];
        
        DisplayQuestion(currentQuestion);
    }

    void DisplayQuestion(QuizDBManager.QuizQuestion question)
    {
        if (questionText != null)
            questionText.text = question.QuestionText;

        // Randomize answer order
        List<int> indices = new List<int>();
        for (int i = 0; i < question.Answers.Length; i++) indices.Add(i);

        // Fisher-Yates shuffle
        for (int i = 0; i < indices.Count; i++)
        {
            int temp = indices[i];
            int rand = Random.Range(i, indices.Count);
            indices[i] = indices[rand];
            indices[rand] = temp;
        }

        // Assign answers to buttons
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < indices.Count)
            {
                answerButtons[i].gameObject.SetActive(true);
                int answerIndex = indices[i];
                
                // Set text
                TextMeshProUGUI btnText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                    btnText.text = question.Answers[answerIndex];

                // Mark correct answer
                bool isButtonCorrect = answerIndex == question.CorrectAnswerIndex;
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(isButtonCorrect));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
        Debug.Log($"AwailableQuestions: {availableQuestions.Count}");
    }

    void OnAnswerSelected(bool isCorrect)
    {
        if (isCorrect)
        {
            Debug.Log("Correct Answer!");
            MonologManager.Instance.RightAnswer();
            if (topBarManager != null) topBarManager.AddCarried();
            if (availableQuestions != null && currentQuestion != null)
            {
                availableQuestions.Remove(currentQuestion);
            }
        }
        else
        {
            Debug.Log("Wrong Answer!");
            MonologManager.Instance.WrongAnswer();
        }
        CancelQuiz();
    }

    public void AddOnCloseCallback(System.Action callback)
    {
        if (callback == null) return;
        if (onQuizCloseCallback != null)
        {
            onQuizCloseCallback += callback;
        }
        else
        {
            onQuizCloseCallback = callback;
        }
    }

    public void CancelQuiz()
    {
        IsQuizActive = false;
        quizPanel.SetActive(false);
        if (CameraMove.Instance != null) CameraMove.Instance.lockState(false);

        if (onQuizCloseCallback != null)
        {
            System.Action callback = onQuizCloseCallback;
            onQuizCloseCallback = null;
            callback();
        }
    }
}
