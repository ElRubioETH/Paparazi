using TMPro;
using UnityEngine;

public class Active : MonoBehaviour
{
    public TextMeshProUGUI textOB;
    public GameObject QuestPan;
    public string dialogue = "Dialogue";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        QuestPan.gameObject.SetActive(false);   
    }

    // Update is called once per frame
    
    public void show()
    {
        QuestPan.gameObject.SetActive(true);
        textOB.text = dialogue;

    }
    public void hide()
    {
        QuestPan.gameObject.SetActive(false);
    }
}
