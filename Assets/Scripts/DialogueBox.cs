using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueBox : MonoBehaviour
{
    bool isPrinting;
    bool isFastForward;
    bool passNext = false;
    string textToPrint;
    [SerializeField] float arrowFlickeringSpeed;
    float timerArrowFlickering;
    [SerializeField] float writingDefalutDelay;
    float writingDelay;
    [SerializeField]float writingFastForward;

    Text text;
    Image dialogueWindow;
    Image arrow;
    AudioSource audioSource;
    [SerializeField] AudioClip voiceClip;
    [SerializeField] AudioClip dialogueFinishClip;
    

    void Awake()
    {
        writingDelay = writingDefalutDelay;
        dialogueWindow = GetComponent<Image>();
        text = transform.GetChild(0).GetComponent<Text>();
        
        arrow = transform.GetChild(1).GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
        timerArrowFlickering = arrowFlickeringSpeed;
    }

    void Update()
    {
        if (isFastForward)
        {
            writingDelay = writingFastForward;
        }
        else
        {
            writingDelay = writingDefalutDelay;
        }
        timerArrowFlickering -= Time.deltaTime;
        if(timerArrowFlickering < 0 && !isPrinting && dialogueWindow.IsActive())
        {
            timerArrowFlickering = arrowFlickeringSpeed;
            arrow.enabled = !arrow.enabled;
        }
    }

    public void OnFastForward(InputAction.CallbackContext context)
    {
        if(context.performed || context.canceled)
        {
            ///Debug.Log("Fastforward");
            if (dialogueWindow.IsActive())
            {
                isFastForward = !isFastForward;
            }
        }
    }

    public void OnPassNextDialogue(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isPrinting && dialogueWindow.IsActive())
            {
                passNext = true;
            }
        }
    }

    public void ShowDialogueBox(bool setter)
    {
        dialogueWindow.enabled = setter;
        text.enabled = setter;
        arrow.enabled = setter;

    }
    //public void Print(string dialogue)
    //{
    //    if (!isPrinting)
    //    {
    //        isPrinting = true;
    //        arrow.enabled = false;
    //        text.text = "";
    //        textToPrint = dialogue;
    //        StartCoroutine(CharPrinter());
    //    }
    //}

    public IEnumerator Print(Dialogue[] dialogues)
    {

        if (isPrinting)
            yield break;

        ShowDialogueBox(true);

        for (int i = 0; i < dialogues.Length; i++)
		{
            isPrinting = true;
            if(arrow)
            arrow.enabled = false;

            text.text = string.Empty;

            if (dialogues[i].italic)
                text.fontStyle = FontStyle.Italic;
            else
                text.fontStyle = FontStyle.Normal;

            yield return StartCoroutine(CharPrinter(dialogues[i].text));

			while (!passNext)
                yield return null;

            passNext = false;
        }
        ShowDialogueBox(false);
    }

    IEnumerator CharPrinter(string textToPrint)
    {
        for (int y = 0; y < textToPrint.Length; y++)
        {
            text.text = string.Concat(text.text + textToPrint[y]);
            
            audioSource.PlayOneShot(voiceClip);
            yield return new WaitForSeconds(writingDelay);
        }
        arrow.enabled = true;
        audioSource.PlayOneShot(dialogueFinishClip);
        isPrinting = false;
        yield return null;
    }
}
