using UnityEngine;

public class DialogueOnStart : MonoBehaviour
{
    public DialogueAsset dialogue;
    public float delay = 0.3f;

    void Start()
    {
        if (dialogue == null) return;

        string id = string.IsNullOrWhiteSpace(dialogue.dialogueId) ? dialogue.name : dialogue.dialogueId;

        bool alreadyPlayed = GameEvents.RequestFlagState?.Invoke(id) ?? false;
        if (alreadyPlayed) return;

        Invoke(nameof(Begin), delay);
    }

    void Begin()
    {
        GameEvents.OnDialogueRequested?.Invoke(dialogue);
    }
}