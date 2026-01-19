using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Actions/Guide To First Door")]
public class GuideToFirstDoorAction : DialogueAction
{
    public override void Execute()
    {
        // שומרים את הלוגיקה כמו שהיה: עדיין מפעילים דרך GameEvents
        GameEvents.OnDialogueEvent?.Invoke("GuideToFirstDoor");
    }
}
