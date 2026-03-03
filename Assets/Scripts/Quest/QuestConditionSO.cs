using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Conditions/Quest Condition")]
public class QuestConditionSO : ScriptableObject
{
    public QuestData quest;
    public QuestStatus requiredStatus;

    public bool IsMet()
    {
        if (quest == null)
        {
            Debug.LogWarning("QuestConditionSO: quest is null");
            return false;
        }

        return QuestManager.Instance.GetStatus(quest.questID) == requiredStatus;
    }
}