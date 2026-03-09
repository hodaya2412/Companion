using UnityEngine;

public class MapUIRootActivator : MonoBehaviour
{
    public GameObject mapUIRoot;

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
        if (action is TriggerGameEventAction triggerAction)
        {
            if (triggerAction.eventType == DialogueGameEventType.EnableMap)
            {
                mapUIRoot.SetActive(true);
            }
        }
    }
}