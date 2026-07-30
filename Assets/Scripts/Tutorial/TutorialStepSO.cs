using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTutorialStep", menuName = "Dialogue Actions/Tutorial Step")]
public class TutorialStepSO : DialogueAction
{
    [TextArea] public string message;
    public KeyCode keyToClose = KeyCode.E; 
    
    public List<KeyCode> keysToClose = new List<KeyCode>();

    public override void Execute()
    {
        if (CompanionTutorial.Instance != null)
            CompanionTutorial.Instance.ShowStep(this);
    }
}