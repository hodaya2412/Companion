using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Actions/Dialogue Choice")]
public class DialogueChoiceAction : DialogueAction
{
    [Header("Choice UI Texts")]
    public string positiveText = "Yes";
    public string negativeText = "No";

    [Header("Choice Results")]
    public DialogueAction positiveAction; // מה קורה אם בוחרים Yes
    public DialogueAction negativeAction; // מה קורה אם בוחרים No

    public override void Execute()
    {
        Debug.Log("DialogueChoiceAction executed!");
        GameEvents.OnDialogueChoiceRequested?.Invoke(this);
    }
}
