using System.Collections;
using TMPro;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private bool isOpen;
    private bool inRange;

    private Animator anim;

    public TakeKey Key;
    public GameObject Collider;
    public PlayerController Player;

    public GameObject Prompt;
    public TextMeshProUGUI promptText;

    public AudioSource openDoor;
    public AudioSource closedDoor;

    void Start()
    {
        isOpen = false;
        inRange = false;
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E) && !Player.isKnockedBack && Key.keyTaken)
        {
            isOpen = true;
            Collider.SetActive(false);
            Prompt.SetActive(false);
            openDoor.PlayOneShot(openDoor.clip);
        }
        else if(inRange && Input.GetKeyDown(KeyCode.E) && !Player.isKnockedBack && !Key.keyTaken)
        {
        StartCoroutine(CannotOpen());
            closedDoor.PlayOneShot(closedDoor.clip);
        }

        HandAnimation();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !isOpen)
        {
            inRange = true;
            Prompt.SetActive(true);
            promptText.SetText("[E] Apri Porta");

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            inRange = false;


            if (Prompt == null)
            {
                return;
            }
            else
            {
                Prompt.SetActive(false);
            }

        }
    }

    public void HandAnimation()
    {
        anim.SetBool("IsOpen", isOpen);
    }

    private IEnumerator CannotOpen()
    {
        Prompt.SetActive(false);

        yield return new WaitForSeconds(0.3f);

        promptText.SetText("Manca la chiave");
        Prompt.SetActive(true);
    }
}
