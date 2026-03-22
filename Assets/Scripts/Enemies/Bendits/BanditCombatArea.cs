using UnityEngine;

public class BanditCombatArea : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameStateManager.Instance == null) return;

        if (GameStateManager.Instance.CurrentGameplayState != GameplayState.Combat)
            return;

        Debug.Log("[BanditCombatArea] Player left combat area. Resetting combat.");

        GameEvents.RequestGameplayStateChange?.Invoke(GameplayState.Playing);
        GameEvents.OnCombatReset?.Invoke();
    }
}