using UnityEngine;

public enum DialogueGameEventType
{
    EnableMap,
    UnlockPuzzle
}

[CreateAssetMenu(menuName = "Dialogue Actions/Trigger Game Event")]
public class TriggerGameEventAction : DialogueAction
{
    public DialogueGameEventType eventType;

    public override void Execute()
    {
        GameEvents.OnDialogueEvent?.Invoke(this);
    }
}