using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue Actions/Start Quest Action")]
public class StartQuestAction : DialogueAction
{
    public QuestData quest;

    public override void Execute()
    {
        if (quest != null)
        {
            QuestManager.Instance.StartQuest(quest);
        }
    }
}