using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Actions/Mark Dialogue Played")]
public class MarkDialoguePlayedAction : DialogueAction
{
    public DialogueAsset dialogue; 
    public override void Execute()
    {
        if (dialogue == null || GameStateManager.Instance == null) return;

        string id = string.IsNullOrWhiteSpace(dialogue.dialogueId) ? dialogue.name : dialogue.dialogueId;
        GameStateManager.Instance.SetFlag(id, true);
    }
}