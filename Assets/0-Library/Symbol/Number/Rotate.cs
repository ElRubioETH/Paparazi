using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DigitController : MonoBehaviour
{
    public TextMeshProUGUI numberText;
    public Button increaseButton;
    public Button decreaseButton;

    [HideInInspector] public int currentValue = 1;

    void Start()
    {
        UpdateDisplay();
        increaseButton.onClick.AddListener(Increase);
        decreaseButton.onClick.AddListener(Decrease);
    }

    void Increase()
    {
        currentValue = (currentValue + 1) % 10;
        if (currentValue == 0) currentValue = 1;
        UpdateDisplay();
    }

    void Decrease()
    {
        currentValue--;
        if (currentValue < 1) currentValue = 9;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        numberText.text = currentValue.ToString();
    }

    public int GetValue()
    {
        return currentValue;
    }
}
