using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Actions/Fail Quest Action")]
public class FailQuestAction : DialogueAction
{
    public QuestData quest;

    public override void Execute()
    {
        if (quest != null)
        {
            // אנחנו קוראים לפונקציה שמכשילה את המשימה ב-QuestManager
            QuestManager.Instance.FailQuest(quest);
        }
    }
}