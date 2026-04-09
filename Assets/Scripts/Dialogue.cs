using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string npcName;
    public string text;
    public Sprite portrait;
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public DialogueLine[] lines;
    public float typingSpeed = 0.05f;
    public AudioSource voice;
}
