using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("PlayerLives")]
    [SerializeField] public float Life;
    [SerializeField] public float MaxLife;
    [SerializeField] private Image lifeBar;
    [SerializeField] public bool gameIsOver;
    [SerializeField] public bool hasDied;

    [Header("GameOver")]
    public GameObject gameOverScreen;

    [Header("Win And More")]

    public GameObject winScreen;
    public int trashCounter;
    public Menù menu;
    public int trashCount;
    public bool getHome;
    public GameObject pauseMenu;
    public GameObject doneCleaning;
    public bool isPaused;

    private bool notified;


    [Header("Music")]
    public AudioSource levelMusic;

    private void Start()
    {
        Time.timeScale = 1.0f;
        trashCounter = 0;
        gameIsOver = false;
        hasDied = false;
        getHome = false;
        notified = false;

        levelMusic.Play();
    }

    private void Update()
    {
        if (Life <= 0 && !hasDied)
        {
            GameOver();
        }

        if (Input.GetKeyUp(KeyCode.Escape))

        {

            PauseGame();

        }

        WinGame();
    }

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateLifeBar()
    {
        lifeBar.fillAmount = Life / MaxLife;
    }


    public void GameOver()
    {
        gameIsOver = true;
        StartCoroutine(TimeToDie());
    }

    public void PauseGame()
    {
        if (Time.timeScale == 1.0f)
        {
            Time.timeScale = 0.0f;
            pauseMenu.SetActive(true);
        }
        else if (Time.timeScale == 0.0f)
        {
            Time.timeScale = 1.0f;
            pauseMenu.SetActive(false);
        }
    }

    public IEnumerator TimeToDie()
    {
        yield return new WaitForSeconds(0.3f);
        hasDied = true;
        yield return new WaitForSeconds(1.5f);
        gameOverScreen.SetActive(true);
        Time.timeScale = 0.0f;
    }

    private void WinGame()
    {
        if (trashCounter >= trashCount)
        {
            getHome = true;
            if (!notified) { 
            StartCoroutine(Notif());
            }
        }
    }

    public void CleanedTrash()
    {
        trashCounter++;
        Debug.Log(trashCounter);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && getHome)
        {
            winScreen.SetActive(true);
            Time.timeScale = 0.0f;
        }

    }

    private IEnumerator Notif()
    {
        notified = true;
        doneCleaning.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        doneCleaning.SetActive(false);
    }

}
