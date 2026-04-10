using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonoLogue : MonoBehaviour
{
   public GameManager manager;
    public NpcDialogueManager dialogueManager;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText, nameText;
    public Image npcPortrait;
    public AudioSource voice;

    private Dialogue dialogueData;
    private int dialogueIndex;
    private bool isTalking;
    private bool isTyping;
    private bool inRange;

    private void Update()
    {
        if (inRange && !isTalking)
        {

                StartDialogue();

        }
        else if (isTalking && Input.GetKeyDown(KeyCode.E) || isTalking && Input.GetKeyDown(KeyCode.Mouse0))
        {
            NextLine();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        inRange = true;
    }

    void StartDialogue()
    {
        dialogueData = dialogueManager.GetCurrentDialogue();

        isTalking = true;
        dialogueIndex = 0;
        dialoguePanel.SetActive(true);

        DialogueLine currentLine = dialogueData.lines[dialogueIndex];
        nameText.SetText(currentLine.npcName);
        npcPortrait.sprite = dialogueData.lines[dialogueIndex].portrait;

        StartCoroutine(TypeLine());

        manager.isPaused = true;
    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.lines[dialogueIndex].text);
            isTyping = false;
        }
        else if (++dialogueIndex < dialogueData.lines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isTalking = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);

        manager.isPaused = false;

        dialogueManager.NextDialogue();

        Destroy(gameObject);

    }

    private IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");
        string line = dialogueData.lines[dialogueIndex].text;

        DialogueLine currentLine = dialogueData.lines[dialogueIndex];
        npcPortrait.sprite = currentLine.portrait;
        nameText.SetText(currentLine.npcName);


        foreach (char letter in line)
        {
            dialogueText.text += letter;
            voice.PlayOneShot(voice.clip);
            yield return new WaitForSeconds(dialogueData.typingSpeed);

        }

        isTyping = false;
    }

}
