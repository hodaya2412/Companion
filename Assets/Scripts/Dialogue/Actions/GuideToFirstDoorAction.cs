using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Actions/Guide To First Door")]
public class GuideToFirstDoorAction : DialogueAction
{
    public override void Execute()
    {
        GameEvents.OnDialogueEvent?.Invoke(this);
    }
}
