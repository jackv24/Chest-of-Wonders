using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    public static DialogueBox instance;

    public Text nameText;
    public Text dialogueText;

    [Space()]
    public GameObject button;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
        {
            Debug.LogWarning("More than one Speech Dialog was found in the scene, and has been removed.");

            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //Dialogue box starts hidden
        gameObject.SetActive(false);
    }
}
