using TMPro;
using UnityEngine;


public class HidingPlace : MonoBehaviour
{
    private bool inRange;


    private Animator anim;


    public GameObject Prompt;
    public TextMeshProUGUI promptText;

    public GameObject SeePlayer;
    public PlayerController Player;
    public GameObject playerAnim;

    public bool isEmpty;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E) && !Player.isKnockedBack && !Player.isChilling)
        {
            if (!Player.isHiding && isEmpty)
            {
                Player.isHiding = true;
                isEmpty = false;

                SeePlayer.SetActive(false);
                playerAnim.SetActive(false);
                Player.transform.position = transform.position;


                promptText.SetText("[E] Esci");
            }
            else if (Player.isHiding && !isEmpty) 
            {
                Player.isHiding = false;
                isEmpty = true;

                SeePlayer.SetActive(true);
                playerAnim.SetActive(true);
                Player.transform.position= new Vector2(transform.position.x, transform.position.y - 1);


                promptText.SetText("[E] Nasconditi");
            }

        }

        HandAnimation();
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            inRange = true;
            Prompt.SetActive(true);
            promptText.SetText("[E] Nasconditi");

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

    private void HandAnimation()
    {
        anim.SetBool("IsEmpty", isEmpty);
    }

}
