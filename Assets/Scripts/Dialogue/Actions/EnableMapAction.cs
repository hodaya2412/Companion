using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Actions/Enable Map")]
public class EnableMapAction : DialogueAction
{
    public override void Execute()
    {
        GameEvents.OnDialogueEvent?.Invoke(this);
    }
}
