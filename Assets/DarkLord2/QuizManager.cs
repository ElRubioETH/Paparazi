using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class QuizManager : MonoBehaviour
{
    public Question[] questions; // M?ng câu h?i t? Inspector
    public Canvas quizCanvas; // Tham chi?u ??n QuizCanvas
    public GameObject successPanel; // Màn hình thành công
    public GameObject failPanel; // Màn hình th?t b?i
    public GameObject door; // Tham chi?u ??n c?a

    public TextMeshProUGUI[] questionTexts = new TextMeshProUGUI[5]; // 5 TextMeshProUGUI cho câu h?i
    public Toggle[] optionToggles = new Toggle[20]; // M?ng 1D: 20 Toggle (5 panel x 4)

    private int correctAnswers = 0;

    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] options; // 4 ?áp án
        public int correctAnswerIndex; // Ch? s? ?áp án ?úng (0-3)
    }

    void Start()
    {
        if (quizCanvas == null || successPanel == null || failPanel == null)
        {
            Debug.LogError("M?t ho?c nhi?u tham chi?u ch?a ???c gán!");
            return;
        }
        quizCanvas.gameObject.SetActive(false);
        successPanel.SetActive(false);
        failPanel.SetActive(false);
        SetupQuizUI();
    }

    void SetupQuizUI()
    {
        for (int i = 0; i < 5 && i < questionTexts.Length && i < questions.Length; i++)
        {
            if (questionTexts[i] != null)
            {
                questionTexts[i].text = questions[i].questionText;
                Debug.Log($"Question {i} text set: {questionTexts[i].text}");
            }
            else
            {
                Debug.LogError($"QuestionText[{i}] ch?a ???c gán!");
            }
            for (int j = 0; j < 4; j++)
            {
                int index = i * 4 + j; // Tính ch? s? trong m?ng 1D (0-19)
                if (index < optionToggles.Length && optionToggles[index] != null)
                {
                    TextMeshProUGUI toggleText = optionToggles[index].GetComponentInChildren<TextMeshProUGUI>();
                    if (toggleText != null)
                    {
                        toggleText.text = questions[i].options[j];
                        Debug.Log($"Toggle {index} text set: {toggleText.text}");
                    }
                    else
                    {
                        Debug.LogError($"Toggle[{index}] thi?u TextMeshProUGUI!");
                    }
                    optionToggles[index].group = optionToggles[index].GetComponentInParent<ToggleGroup>();
                }
                else
                {
                    Debug.LogError($"OptionToggles[{index}] ch?a ???c gán!");
                }
            }
        }
    }

    public void ShowQuiz()
    {
        if (quizCanvas == null)
        {
            Debug.LogError("Quiz Canvas ch?a ???c gán!");
            return;
        }
        quizCanvas.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Cursor State: " + Cursor.lockState + ", Visible: " + Cursor.visible);
        ResetToggles();
    }

    public void SubmitQuiz()
    {
        if (quizCanvas == null)
        {
            Debug.LogError("Quiz Canvas ch?a ???c gán!");
            return;
        }
        correctAnswers = 0;
        for (int i = 0; i < 5 && i < questions.Length; i++)
        {
            bool foundSelection = false;
            for (int j = 0; j < 4; j++)
            {
                int index = i * 4 + j;
                if (index < optionToggles.Length && optionToggles[index] != null)
                {
                    Debug.Log($"Checking Panel {i}, Option {j}, Toggle Index: {index}, isOn: {optionToggles[index].isOn}");
                    if (optionToggles[index].isOn)
                    {
                        foundSelection = true;
                        Debug.Log($"Panel {i}, Option {j} is selected, Correct Index: {questions[i].correctAnswerIndex}");
                        if (j == questions[i].correctAnswerIndex)
                        {
                            correctAnswers++;
                            Debug.Log($"Correct answer detected for Panel {i}");
                        }
                        break;
                    }
                }
            }
            if (!foundSelection)
                Debug.LogWarning($"No selection found for Panel {i}");
        }

        quizCanvas.gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log($"Correct Answers: {correctAnswers}/5");
        if (correctAnswers >= 3)
        {
            successPanel.SetActive(true);
            if (door != null)
                Destroy(door);
        }
        else
        {
            failPanel.SetActive(true);
        }
    }

    public void CloseFailPanel()
    {
        if (failPanel != null)
            failPanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void ResetToggles()
    {
        if (quizCanvas == null) return;
        foreach (Toggle toggle in optionToggles)
        {
            if (toggle != null)
            {
                bool initialState = toggle.isOn;
                toggle.isOn = false;
                toggle.onValueChanged.AddListener((isOn) =>
                    Debug.Log($"Toggle at index {Array.IndexOf(optionToggles, toggle)} isOn: {isOn}, Panel: {Array.IndexOf(optionToggles, toggle) / 4}, Option: {Array.IndexOf(optionToggles, toggle) % 4}"));
            }
        }
    }
}