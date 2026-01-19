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

    void HandleEvent(string eventId)
    {
        if (eventId == "EnableMap")
        {
            mapUIRoot.SetActive(true);
        }
    }
}
