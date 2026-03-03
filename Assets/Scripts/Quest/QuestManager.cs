using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    private Dictionary<string, QuestStatus> quests = new Dictionary<string, QuestStatus>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ✅ הפעלת משימה
    public void StartQuest(QuestData quest)
    {
        quests[quest.questID] = QuestStatus.InProgress;
        Debug.Log($"Quest '{quest.questName}' started.");
    }

    // ✅ סיום משימה בהצלחה
    public void CompleteQuest(QuestData quest)
    {
        quests[quest.questID] = QuestStatus.Completed;
        Debug.Log($"Quest '{quest.questName}' completed!");
    }

    // ✅ סיום משימה בכישלון
    public void FailQuest(QuestData quest)
    {
        quests[quest.questID] = QuestStatus.Failed;
        Debug.Log($"Quest '{quest.questName}' failed.");
    }

    // ✅ בדיקה של מצב משימה
    public QuestStatus GetStatus(string questID)
    {
        if (quests.ContainsKey(questID))
            return quests[questID];
        return QuestStatus.NotStarted;
    }
    public bool IsInProgress(QuestData quest) => GetStatus(quest.questID) == QuestStatus.InProgress;
    public bool IsComplete(QuestData quest) => GetStatus(quest.questID) == QuestStatus.Completed;
    public bool IsFailed(QuestData quest) => GetStatus(quest.questID) == QuestStatus.Failed;

}