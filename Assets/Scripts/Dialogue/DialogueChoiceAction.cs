using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Actions/Dialogue Choice")]
public class DialogueChoiceAction : DialogueAction 
{
    [Header("Choice UI Texts")]
    public string positiveText = "Yes";
    public string negativeText = "No";

    [Header("Choice Results")]
    public List<DialogueAction> positiveActions = new List<DialogueAction>();
    public List<DialogueAction> negativeActions = new List<DialogueAction>();

    public override void Execute()
    {
        Debug.Log("Dialogue Choice executed!");
        GameEvents.OnDialogueChoiceRequested?.Invoke(this);
    }

    public void ExecutePositiveActions()
    {
        foreach (var action in positiveActions)
        {
            action?.Execute();
        }
    }

    public void ExecuteNegativeActions()
    {
        foreach (var action in negativeActions)
        {
            action?.Execute();
        }
    }
}