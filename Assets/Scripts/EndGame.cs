using UnityEngine;

public class EndGame : MonoBehaviour
{
    public GameManager manager;
    public GameObject Roze;


    void Update()
    {
        if (manager.getHome)
        {
            Roze.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (manager.getHome && other.gameObject.CompareTag("NPC")) 
        {
            manager.winScreen.SetActive(true);
            Time.timeScale = 0.0f;
        }
    }
}
