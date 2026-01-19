using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueAsset", menuName = "Scriptable Objects/DialogueAsset")]
public class DialogueAsset : ScriptableObject
{
    public List<DialogueLine> lines = new();
    //Old- event string
    public List<string> endEvents;
    //New dialogue actions
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