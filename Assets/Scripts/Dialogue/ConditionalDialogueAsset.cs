using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Conditional Dialogue")]
public class ConditionalDialogueAsset : ScriptableObject
{
    [Header("הדיאלוג עצמו")]
    public DialogueAsset dialogue;

    [Header("תנאי Quest (למשימות)")]
    public QuestConditionSO[] questConditions;

    //[Header("תנאי Flags (לאירועים כלליים בלבד)")]
    //public FlagConditionSO[] flagConditions;

    public bool CanPlay()
    {
        // בדיקת תנאי משימות
        if (questConditions != null)
        {
            foreach (var condition in questConditions)
            {
                if (condition != null && !condition.IsMet())
                    return false;
            }
        }

        //// בדיקת תנאי Flags
        //if (flagConditions != null)
        //{
        //    foreach (var condition in flagConditions)
        //    {
        //        if (condition != null && !condition.IsMet())
        //            return false;
        //    }
        //}

        return true;
    }
}