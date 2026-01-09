using UnityEngine;

public class DialogueOnStart : MonoBehaviour
{
    public DialogueAsset dialogue;
    public float delay = 0.2f;

    void Start()
    {
        if (dialogue == null) return;
        Invoke(nameof(Begin), delay);
    }

    void Begin()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }
}