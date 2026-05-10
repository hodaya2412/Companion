using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Actions/Start Dialogue")]
public class StartDialogueAction : DialogueAction
{
    [Header("Dialogue To Start")]
    public DialogueAsset dialogueToStart;

    public override void Execute()
    {
        if (dialogueToStart == null)
        {
            Debug.LogWarning("No dialogue assigned to StartDialogueAction");
            return;
        }

        GameEvents.OnDialogueRequested?.Invoke(dialogueToStart);
    }
}