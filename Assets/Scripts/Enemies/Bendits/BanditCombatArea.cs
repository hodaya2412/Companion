using UnityEngine;

public class BanditCombatArea : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string puzzleSolvedFlag = "Forest_PuzzleSolved";

    private bool hasTriggeredCombat = false;

    private void OnEnable()
    {
        GameEvents.OnCombatReset += ResetCombatTrigger;
    }

    private void OnDisable()
    {
        GameEvents.OnCombatReset -= ResetCombatTrigger;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggeredCombat) return;
        if (!other.CompareTag(playerTag)) return;

        bool puzzleSolved = GameEvents.RequestFlagState?.Invoke(puzzleSolvedFlag) ?? false;
        if (puzzleSolved) return;

        hasTriggeredCombat = true;

        GameEvents.RequestGameplayStateChange?.Invoke(GameplayState.Combat);
        GameEvents.OnCombatTriggered?.Invoke();
    }

    private void ResetCombatTrigger()
    {
        hasTriggeredCombat = false;
    }
}