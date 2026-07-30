using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueAsset", menuName = "Scriptable Objects/DialogueAsset")]
public class DialogueAsset : ScriptableObject
{
    [Header("Dialogue ID (for persistence)")]
    public string dialogueId;   

    public List<DialogueLine> lines = new();

    
    public List<DialogueAction> endActions;
}

[Serializable]
public class DialogueLine
{
    public string speaker;

    [TextArea(2, 6)]
    public string text;

    public Sprite portrait;
}