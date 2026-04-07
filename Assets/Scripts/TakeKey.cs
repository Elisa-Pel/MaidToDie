using System.Collections;
using TMPro;
using UnityEngine;

public class TakeKey : MonoBehaviour
{
    public PlayerController Player;
    public GameObject sparkle;

    private Collider2D hitbox;

    public GameObject Prompt;
    public TextMeshProUGUI promptText;

    public GameObject keyNotif;

    public bool keyTaken;
    private bool inRange;


    void Start()
    {
        keyTaken = false; 
        hitbox = GetComponent<Collider2D>();
    }

    
    void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E) && !Player.isKnockedBack)
        {
            keyTaken = true;
            Destroy(hitbox);
            sparkle.SetActive(false);
            Prompt.SetActive(false);

            StartCoroutine(KeyType());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            inRange = true;
            Prompt.SetActive(true);
            promptText.SetText("[E] Raccogli Chiave");

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

    private IEnumerator KeyType()
    {
        yield return new WaitForSeconds(0.3f);

        keyNotif.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        keyNotif.SetActive(false);
    }

}
