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
    public float isfacing;
    public float needsToFace;

    public Transform WaypointParent;
    private Transform[] waypoints;
    private int currentWaypointIndex;

    private Dialogue dialogueData;
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;

    public int stopUntilWaypointIndex = 2;

    private int dialogueIndex;
    private bool isTalking;
    private bool isTyping;
    private bool inRange;
    private bool hasTalked;
    private bool isWaiting;
    private bool requireWaypointToTalk = false;
    private bool isAtWaypoint = false;

    void Start()
    {
        dialogueData = dialogueManager.GetCurrentDialogue();
        rb = GetComponent<Rigidbody2D>();
        anim= GetComponentInChildren<Animator>();
        coll = GetComponent<Collider2D>();

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
            if (!requireWaypointToTalk || isAtWaypoint)
            {
                StartDialogue();
            }

        }
        else if (isTalking &&  Input.GetKeyDown(KeyCode.E) || isTalking && Input.GetKeyDown(KeyCode.Mouse0))
        {
            NextLine();
        }

        MoveToWaypoint();
        HandAnimation();
        CheckDirection();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && isAtWaypoint)
        {
            inRange = true;
        }

        if (other.gameObject.CompareTag("Exit"))
        {
            Destroy(gameObject);
        }

    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inRange = false;
           
        }
    }


    private void OnCollisionEnter2D (Collision2D other)
    {

        if (other.gameObject.CompareTag("Player")|| other.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(PassTrough());

        }
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

        dialogueManager.NextDialogue();

        isWaiting = false;
        //currentWaypointIndex = Mathf.Min(currentWaypointIndex + 1, waypoints.Length - 1);

        requireWaypointToTalk = true;
    }

    IEnumerator TypeLine()
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
            yield return new WaitForSeconds(dialogueData.typingSpeed);

        }

        isTyping = false;
    }

    void MoveToWaypoint()
    {

        if (isWaiting)
        {
            return;
        }

        Transform moveTo = waypoints[currentWaypointIndex];
        Vector3 direction = (moveTo.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        if (Vector2.Distance(transform.position, moveTo.position) < 0.1f)
        {
            
            hasTalked = false;

            if (currentWaypointIndex <= stopUntilWaypointIndex)
            {
                isAtWaypoint = true;
                rb.linearVelocity = new Vector2(0, 0) * speed;
                isWaiting = true;
                isfacing = needsToFace;


            }
             currentWaypointIndex = Mathf.Min(currentWaypointIndex + 1, waypoints.Length - 1); 
            
        }

    }
    private void CheckDirection()
    {
        if (rb.linearVelocity.y >= 1.1f)
        {
            isfacing = 1; //Up
        }
        else if (rb.linearVelocity.y <= -1.1f)
        {
            isfacing = 2;//Down
        }
        else if (rb.linearVelocity.x >= 1)
        {
            isfacing = 3;//Right
        }
        else if (rb.linearVelocity.x <= -1)
        {
            isfacing = 4;//Left
        }


        // Debug.Log(isfacing);
    }

    void HandAnimation()
    {

        anim.SetFloat("isFacing", isfacing);
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isWaiting", isWaiting);
        anim.SetBool("isPaused", manager.isPaused);

    }

    private IEnumerator PassTrough()
    {
        coll.enabled = false;
        yield return new WaitForSeconds(1.5f);
        coll.enabled = true;
    }

}
