using UnityEngine;

public class SetActiveIfFlag : MonoBehaviour
{
    [SerializeField] private string flagName;
    [SerializeField] private GameObject targetObject;

    private void Start()
    {
        if (GameStateManager.Instance == null) return;
        if (targetObject == null) return;

        bool flagValue = GameStateManager.Instance.GetFlag(flagName);
        targetObject.SetActive(flagValue);
    }
}