using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Actions/Set Flag Action")]
public class SetFlagAction : DialogueAction
{
    public string flagName;
    public bool value = true;

    public override void Execute()
    {
        GameStateManager.Instance.SetFlag(flagName, value);
        Debug.Log($"Flag '{flagName}' set to {value}");
    }
}