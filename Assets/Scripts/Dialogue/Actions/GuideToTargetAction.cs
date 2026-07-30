using UnityEngine;


public enum GuideTargetID
{
    None,
    FirstDoor,
    DragonCave,
    Tower,
    weapone
}


[CreateAssetMenu(menuName = "Dialogue Actions/Guide To Target")]
public class GuideToTargetAction : DialogueAction
{
    public GuideTargetID targetID;

    public override void Execute()
    {
        
        GameEvents.OnGuideRequested?.Invoke(targetID);
    }
}