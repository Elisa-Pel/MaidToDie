using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public Dialogue dialogueData;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText, nameText;
    public Image npcPortrait;

    private int dialogueIndex;
    private bool isTalking;
    private bool isTyping;
    private bool inRange;
    private bool hasTalked;

    void Start()
    {
        
    }


    void Update()
    {
        if (inRange && !isTalking && !hasTalked)
        {
            StartDialogue();
        }
        else if (isTalking &&  Input.GetKeyDown(KeyCode.E) || isTalking && Input.GetKeyDown(KeyCode.Mouse0))
        {
            NextLine();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    void StartDialogue()
    {
        isTalking = true;
        dialogueIndex = 0;
        dialoguePanel.SetActive(true);
        
        nameText.SetText(dialogueData.npcName);
        npcPortrait.sprite = dialogueData.lines[dialogueIndex].portrait;

        StartCoroutine(TypeLine());

        hasTalked = true;
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
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");
        string line = dialogueData.lines[dialogueIndex].text;

        DialogueLine currentLine = dialogueData.lines[dialogueIndex];
        npcPortrait.sprite = currentLine.portrait;

        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);

        }

        isTyping = false;
    }
}
