using UnityEngine;


[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Quest Data")]
public class QuestData : ScriptableObject
{
    public string questID; 
    public string questName; 
    [TextArea] public string description; 
}

public enum QuestStatus { NotStarted, InProgress, Completed, Failed }