using UnityEngine;

public class TutorialEventTrigger : MonoBehaviour
{
    public TutorialStepSO stepToDisplay;

    [Header("Settings")]
    public bool triggerByProximity = true;
    public bool playOnlyOnce = true;
    public string uniqueId;
    public string playerTag = "Player";

    private bool hasBeenPlayed = false;

    private void Start()
    {
     
        if (string.IsNullOrEmpty(uniqueId)) uniqueId = gameObject.name;

        
        if (GameStateManager.Instance != null)
        {
            hasBeenPlayed = GameStateManager.Instance.GetFlag(uniqueId);
        }
    }

    public void TriggerShow()
    {
        
        if (playOnlyOnce && hasBeenPlayed) return;

        if (CompanionTutorial.Instance != null && stepToDisplay != null)
        {
            CompanionTutorial.Instance.ShowStep(stepToDisplay);

            if (playOnlyOnce)
            {
                hasBeenPlayed = true;

                
                if (GameStateManager.Instance != null)
                {
                    GameStateManager.Instance.SetFlag(uniqueId, true);
                }
            }
        }
    }

    public void TriggerHide()
    {
        if (CompanionTutorial.Instance != null)
            CompanionTutorial.Instance.Hide();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerByProximity && other.CompareTag(playerTag))
            TriggerShow();
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerByProximity && other.CompareTag(playerTag))
            TriggerHide();
    }
}