using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class ItemDialogueMapping
{
    public string itemId;
    public DialogueAsset dialogue;
}

public class ItemSequenceManager : MonoBehaviour
{
    public List<ItemDialogueMapping> sequences = new List<ItemDialogueMapping>();

    private void OnEnable() { GameEvents.OnItemPickedUp += HandleItemPickedUp; }
    private void OnDisable() { GameEvents.OnItemPickedUp -= HandleItemPickedUp; }

    private void HandleItemPickedUp(string itemId)
    {
        var mapping = sequences.Find(m => m.itemId == itemId);
        if (mapping != null) GameEvents.OnDialogueRequested?.Invoke(mapping.dialogue);
    }
}