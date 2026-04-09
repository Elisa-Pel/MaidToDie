using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menù : MonoBehaviour
{
    public TextMeshProUGUI continueText;
    public GameObject errorText;
  

    private void Start()
    {
        if (PlayerPrefs.GetInt("CurrentScene") == 2 || PlayerPrefs.GetInt("CurrentScene") == 3)
        {
            continueText.SetText("VUOI CONTINUARE DAL GIORNO " + (PlayerPrefs.GetInt("CurrentScene")) + "?");
        }
        else
        {
            continueText.SetText("NESSUNA PARTITA SALVATA");
           
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Notte1");
        PlayerPrefs.SetInt("CurrentScene", 1);
    }

    public void SecondNight()
    {
        SceneManager.LoadScene("Notte2");
        PlayerPrefs.SetInt("CurrentScene", 2);
    }

    public void ThirdNight()
    {
        SceneManager.LoadScene("Notte3");
        PlayerPrefs.SetInt("CurrentScene", 3);
    }


    public void Continue()
    {
        if (PlayerPrefs.GetInt("CurrentScene") == 1)
        {
            errorText.SetActive(true);
        }
        else if (PlayerPrefs.GetInt("CurrentScene") == 2) 
        {
            SceneManager.LoadScene("Notte2");
        }
        else if (PlayerPrefs.GetInt("CurrentScene") == 3)
        {
            SceneManager.LoadScene("Notte3");
        }
    }


    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }


    public void ToMenu()
    {
        SceneManager.LoadScene("Main Menu");

    }

    public void Unpause()
    {
        Time.timeScale = 1.0f;
    }

    public void Progress()
    {
        PlayerPrefs.SetInt("CurrentScene", PlayerPrefs.GetInt("CurrentScene") + 1);
    }
}

