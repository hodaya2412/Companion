using System.Collections.Generic;
using UnityEngine;

public class TutorialStepSO : DialogueAction
{
    [TextArea] public string message;
    public KeyCode keyToClose = KeyCode.E; // מקש ברירת מחדל
    // אם את רוצה תמיכה בכמה מקשים (כמו WASD), אפשר להשתמש ברשימה:
    public List<KeyCode> keysToClose = new List<KeyCode>();

    public override void Execute()
    {
        if (CompanionTutorial.Instance != null)
            CompanionTutorial.Instance.ShowStep(this);
    }
}