using UnityEngine;

public class EnemyGroupDefeatFlag : MonoBehaviour
{
    [SerializeField] private int totalEnemies = 4;
    [SerializeField] private string flagToSet = "Forest_BookAccessible";
    [SerializeField] private bool setOnlyOnce = true;

    private int deadCount = 0;
    private bool alreadySet = false;

    public void OnEnemyDied()
    {
        if (setOnlyOnce && alreadySet) return;

        deadCount++;

        Debug.Log($"[EnemyGroupDefeatFlag] Enemy died. deadCount = {deadCount}/{totalEnemies}");

        if (deadCount >= totalEnemies)
        {
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.SetFlag(flagToSet, true);
                Debug.Log($"[EnemyGroupDefeatFlag] Flag set: {flagToSet} = true");
                alreadySet = true;
            }
            else
            {
                Debug.LogWarning("[EnemyGroupDefeatFlag] GameStateManager.Instance is null");
            }
        }
    }
}