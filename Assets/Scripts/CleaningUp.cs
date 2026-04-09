using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CleaningUp : MonoBehaviour
{
    public GameManager gameManager;

    public PlayerController player;
    public Image progressBar;
    public GameObject Prompt;
    public TextMeshProUGUI promptText;

    public bool isInRange;
    public GameObject progressBarBase;

    private float cleaningProgress = 0;
    [SerializeField] private float maxCleaningProgress;
   
    private bool isBeingCleaned;
    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isInRange && !player.isKnockedBack && !player.isChilling)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (player.isMoving)
                {
                    StartCoroutine(isPlayerSTillMoving());
                }
                else
                {
           
                    player.isCleaning = true;
                    isBeingCleaned = true;
                    Prompt.SetActive(false);

                }
                //Debug.Log("E");
            }
        }

        if (player.isCleaning && isBeingCleaned)
        {
            progressBarBase.SetActive(true);
            cleaningProgress += Time.deltaTime * 1;
            UpdateProgressBar();
            CleanUp();
        }
        else if (player.isMoving && isInRange)
        {
            progressBarBase.SetActive(false);
            cleaningProgress = 0;
            Prompt.SetActive(true);
        }

      

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            isInRange = true;

            Prompt.SetActive(true);
            promptText.SetText("[E] Pulisci");

            // Debug.Log("Bruh");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            isInRange = false;

            isBeingCleaned = false;
            cleaningProgress = 0;
            UpdateProgressBar();


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

    private IEnumerator isPlayerSTillMoving()
    {
    yield return new WaitForSeconds(0.5f);
        if(player.isMoving == false)
        {

            player.isCleaning = true;
            isBeingCleaned = true;

            CleanUp();

            Prompt.SetActive(false);
        }
    }

    private void CleanUp()
    {
        if (cleaningProgress >= maxCleaningProgress)
        {
            Destroy(gameObject);
            player.isCleaning = false;
            isBeingCleaned = false;
            gameManager.CleanedTrash();
            progressBarBase.SetActive(false);
            cleaningProgress = 0;
        }

    }

    private void UpdateProgressBar()
    {

        progressBar.fillAmount = cleaningProgress / maxCleaningProgress;
    }

}

