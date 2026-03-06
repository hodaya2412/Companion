using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Actions/Complete Quest Action")]
public class CompleteQuestAction : DialogueAction
{
    public QuestData quest;

    public override void Execute()
    {
        if (quest != null)
        {
            // כאן אנחנו קוראים לפונקציה שמסיימת את המשימה ב-QuestManager שלך
            QuestManager.Instance.CompleteQuest(quest);
        }
    }
}