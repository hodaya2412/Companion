using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Conditions/Flag Condition")]
public class FlagConditionSO : ScriptableObject
{
    public string flagName;
    public bool requiredValue = true;

    public bool IsMet()
    {
        // בודק מול ה-GameStateManager שלך
        return GameStateManager.Instance.GetFlag(flagName) == requiredValue;
    }
}