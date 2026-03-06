using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Conditional Dialogue")]
public class ConditionalDialogueAsset : ScriptableObject
{
    public DialogueAsset dialogue;

    [Header("תנאי Quest")]
    public QuestConditionSO[] questConditions;

    [Header("תנאי Flag (חדש!)")]
    public FlagConditionSO[] flagConditions; // הוסיפי את השורה הזו

    public bool CanPlay()
    {
        // בדיקת תנאי משימות (הקוד הקיים שלך)
        if (questConditions != null)
        {
            foreach (var condition in questConditions)
            {
                if (condition != null && !condition.IsMet()) return false;
            }
        }

        // בדיקת תנאי דגלים (התוספת החדשה)
        if (flagConditions != null)
        {
            foreach (var condition in flagConditions)
            {
                if (condition != null && !condition.IsMet()) return false;
            }
        }

        return true;
    }
}