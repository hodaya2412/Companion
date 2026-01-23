using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Actions/Unlock Puzzle")]
public class UnlockPuzzleAction : DialogueAction
{
    public override void Execute()
    {
        // זורק אירוע עם Action עצמו
        GameEvents.OnDialogueEvent?.Invoke(this);
    }
}
