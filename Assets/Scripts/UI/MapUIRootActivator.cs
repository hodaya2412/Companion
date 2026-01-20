using UnityEngine;

public class MapUIRootActivator : MonoBehaviour
{
    public GameObject mapUIRoot;

    [Header("Dialogue Actions")]
    public EnableMapAction enableMapAction;

    void OnEnable()
    {
        GameEvents.OnDialogueEvent += HandleEvent;
    }

    void OnDisable()
    {
        GameEvents.OnDialogueEvent -= HandleEvent;
    }

    void HandleEvent(DialogueAction action)
    {
        if (action != enableMapAction) return;

        mapUIRoot.SetActive(true);
    }
}
