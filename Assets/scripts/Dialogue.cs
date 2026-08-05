using System.Collections;
using UnityEngine;
using TMPro;


public class Dialogue : MonoBehaviour
{
    private bool isPlayerInRange;
    private bool didDialogueStart;
    private int lineIndex;
    private float typingTime = 0.05f;
    

    [SerializeField] private GameObject dialogueMark;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField, TextArea(3, 10)] private string[] dialogue;
    [SerializeField] private GameObject Osito;


    // Update is called once per frame


    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!didDialogueStart)
            {
                StartDialogue();

            }
            else if (dialogueText.text == dialogue[lineIndex])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                dialogueText.text = dialogue[lineIndex];
                
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
         
            if (dialogueMark != null)
            {
                dialogueMark.SetActive(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        
            if (dialogueMark != null)
            {
                dialogueMark.SetActive(false);
            }
        }
    }
    
    private void StartDialogue()
    {
        didDialogueStart = true;
        dialoguePanel.SetActive(true);
        dialogueMark.SetActive(false);
        lineIndex= 0;
      
        StartCoroutine(ShowLine());
    }

    private IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty;
        foreach (char c in dialogue[lineIndex].ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typingTime);
        }
    }

    private void NextLine()
    {
        lineIndex++;
        if (lineIndex < dialogue.Length)
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            dialoguePanel.SetActive(false);
            didDialogueStart = false;
            lineIndex = 0;
           Osito.GetComponent<Rigidbody2D>().velocity = new Vector2(1f, 0f); // Reanuda el movimiento de osito
        }
    }

}
