using UnityEngine;

// עכשיו זה הופך לנכס שאפשר ליצור בתיקייה (Asset)
[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Quest Data")]
public class QuestData : ScriptableObject
{
    public string questID; // המזהה הייחודי (למשל George_01)
    public string questName; // השם שיוצג
    [TextArea] public string description; // תיאור המשימה
}

public enum QuestStatus { NotStarted, InProgress, Completed, Failed }