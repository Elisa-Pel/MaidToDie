using UnityEngine;

public class NpcDialogueManager : MonoBehaviour
{
    public Dialogue[] dialogues; 
    private int currentIndex = 0;

    public Dialogue GetCurrentDialogue()
    {
        return dialogues[currentIndex];
    }

    public void SetDialogue(int index)
    {
        if (index >= 0 && index < dialogues.Length)
        {
            currentIndex = index;
        }
    }

    public void NextDialogue()
    {
        currentIndex = (currentIndex + 1) % dialogues.Length;
    }
}
