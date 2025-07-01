using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public Text textOB;
    public GameObject Activator;
    public string dialogue = "Dialogue";

    public float timer = 2f;



    void Start()
    {
        textOB.GetComponent<Text>().enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            textOB.GetComponent<Text>().enabled = true;
            textOB.text = dialogue.ToString();
            StartCoroutine(DisableText());
        }
    }

    IEnumerator DisableText()
    {
        yield return new WaitForSeconds(timer);
        textOB.GetComponent<Text>().enabled = false;
        Destroy(Activator);

    }


}
