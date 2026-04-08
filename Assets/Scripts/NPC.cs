using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public GameManager manager;

   
    public NpcDialogueManager dialogueManager;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText, nameText;
    public Image npcPortrait;

    public int speed;

    public Transform WaypointParent;
    private Transform[] waypoints;
    private int currentWaypointIndex;

    private Dialogue dialogueData;
    private Rigidbody2D rb;

    private int dialogueIndex;
    private bool isTalking;
    private bool isTyping;
    private bool inRange;
    private bool hasTalked;

    void Start()
    {
        dialogueData = dialogueManager.GetCurrentDialogue();
        rb = GetComponent<Rigidbody2D>();

        waypoints = new Transform[WaypointParent.childCount];
        for (int i = 0; i < WaypointParent.childCount; i++)
        {
            waypoints[i] = WaypointParent.GetChild(i);
        }

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

        MoveToWaypoint();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inRange = true;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inRange = false;
        }
    }


    void StartDialogue()
    {
        dialogueData = dialogueManager.GetCurrentDialogue();

        isTalking = true;
        dialogueIndex = 0;
        dialoguePanel.SetActive(true);
        
        nameText.SetText(dialogueData.npcName);
        npcPortrait.sprite = dialogueData.lines[dialogueIndex].portrait;

        StartCoroutine(TypeLine());

        hasTalked = true;

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
        StartCoroutine(SpeakAgain());

        dialogueManager.NextDialogue();

        currentWaypointIndex = Mathf.Min(currentWaypointIndex + 1, waypoints.Length - 1);
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

    private IEnumerator SpeakAgain()
    {
        yield return new WaitForSeconds(3);
        hasTalked = false;
    }

    void MoveToWaypoint()
    {
        Transform moveTo = waypoints[currentWaypointIndex];
        Vector3 direction = (moveTo.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        if (Vector2.Distance(transform.position, moveTo.position) < 0.1f)
            {
            rb.linearVelocity = new Vector2(0, 0) * speed;
        }

    }

}
