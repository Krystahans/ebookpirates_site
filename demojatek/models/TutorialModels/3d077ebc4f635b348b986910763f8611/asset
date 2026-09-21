using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Needed for Skip()

public class QuizDBManager : MonoBehaviour
{
    // Singleton instance to allow easy access from other scripts
    public static QuizDBManager Instance { get; private set; }

    /// <summary>
    /// Represents a single question from the quiz CSV.
    /// </summary>
    public class QuizQuestion
    {
        public string ID;
        public string QuestionText;
        public string[] Answers;
        public int CorrectAnswerIndex;
    }

    [Tooltip("Assign the 'Quiz Kerdesek_valaszok.csv' file here.")]
    public TextAsset quizFile;

    /// <summary>
    /// The list of all questions loaded from the CSV file.
    /// </summary>
    public List<QuizQuestion> Questions { get; private set; } = new List<QuizQuestion>();

    void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        LoadQuizData();
    }

    /// <summary>
    /// Loads and parses the quiz data from the assigned CSV file.
    /// </summary>
    private void LoadQuizData()
    {
        if (quizFile == null)
        {
            Debug.LogError("Quiz CSV file is not assigned in the QuizDBManager inspector!", this);
            return;
        }

        // Split the file into lines. This handles both Windows and Unix line endings.
        string[] lines = quizFile.text.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length <= 1)
        {
            Debug.LogWarning("Quiz CSV file is empty or has only a header line.", this);
            return;
        }

        // Use LINQ's Skip(1) to ignore the header row.
        foreach (string line in lines)
        {
            // CSV format: ID;Question;CorrectAnswer;WrongAnswer1;WrongAnswer2;WrongAnswer3
            string[] fields = line.Split(';');

            if (fields.Length < 6)
            {
                Debug.LogWarning($"Skipping malformed line in CSV: \"{line}\"", this);
                continue;
            }

            try
            {
                var question = new QuizQuestion
                {
                    ID = fields[0].Trim(),
                    QuestionText = fields[1].Trim(),
                    Answers = new string[]
                    {
                        fields[2].Trim(), // Correct Answer
                        fields[3].Trim(), // Wrong Answer 1
                        fields[4].Trim(), // Wrong Answer 2
                        fields[5].Trim()  // Wrong Answer 3
                    },
                    CorrectAnswerIndex = 0 // Correct answer is always the first one in this format
                };
                //Debug.Log($"Loaded Question ID {question.ID}: {question.QuestionText}", this);

                Questions.Add(question);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Error parsing line: \"{line}\". Error: {ex.Message}", this);
            }
        }

        Debug.Log($"Successfully loaded {Questions.Count} questions from {quizFile.name}.", this);
    }
}
