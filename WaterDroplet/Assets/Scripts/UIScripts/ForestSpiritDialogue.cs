using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForestSpiritDialog : MonoBehaviour
{
    public GameObject dialogPanal;    
    public Text dialogText;
    public string[] dialogue;
    private int index;
    public GameObject enterButton;


    private Coroutine typing;
    public float wordSpeed = 0.05f;
    private bool isTyping = false;
    private bool enterEnabled = false;

    private CanvasGroup canvasGroup;
    public float fadeInDuration = 0.5f; // Duration of the fade-in effect
    public float fadeOutDuration = 0.5f; // Duration of the fade-out effect
    public float fadeInDelay = 2f;


    public PlayerController playerController;

    //Animation
    //public Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = dialogPanal.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        //get the Animator component attached to the enter button panel
        //animator = enterButton.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            skipDialogue();
        }
    }

    public void skipDialogue()
    {
        if (isTyping && enterEnabled)
        {
            StopCoroutine(typing);
            dialogText.text = dialogue[index];
            isTyping = false;

            //stop sound
            SoundManager.StopSound(SoundType.FORESTVOICE, 0.1f);
        }
        else if (!isTyping && enterEnabled)
        {
            nextDialogue();
        }
    }

    public void StartDialogue()
    {
        dialogPanal.SetActive(true);
        playerController.enabled = false;
        StartCoroutine(fadeThanType());
    }


    private void closePanal()
    {
        dialogText.text = "";
        index = 0;

        StartCoroutine(FadeOut());
        //dialogPanal.SetActive(false);
        playerController.enabled = true;
    }


    private IEnumerator fadeThanType()
    {
        StartCoroutine(FadeIn());
        yield return new WaitForSeconds(fadeInDuration + fadeInDelay + 0.1f);


        enterEnabled = true;
        typing = StartCoroutine(Typing());
    }


    private IEnumerator Typing()
    {
        // Play forest spirit voice sound
        SoundManager.PlaySound(SoundType.FORESTVOICE, 1, 0.2f);

        isTyping = true;
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
            
        }
        isTyping = false;
        SoundManager.StopSound(SoundType.FORESTVOICE, 0.1f);
    }


    private void nextDialogue()
    {
        if(index < dialogue.Length - 1)
        {
            dialogText.text = "";
            index++;
            typing = StartCoroutine(Typing());
        }
        else
        {
            closePanal();
        }
    }



    private IEnumerator FadeIn()
    {
        //canvasGroup.alpha = 0f;
        yield return new WaitForSeconds(fadeInDelay);

        //canvasGroup.gameObject.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        //canvasGroup.gameObject.SetActive(false);
        dialogPanal.SetActive(false);
    }
}
