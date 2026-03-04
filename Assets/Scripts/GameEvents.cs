using System;
using UnityEngine;

public static class GameEvents
{
    public static Action<DialogueAction> OnDialogueEvent;
    public static Action<InventoryItemData> Clicked;
    public static Action<bool> OnCompanionFollowEnabled;
    public static Action OnInventoryChanged;
    public static Action<GameState> OnStateChanged;
    public static Action<GuideTargetID> OnGuideRequested;
    public static Action<DialogueAsset> OnDialogueRequested;
    public static Action<DialogueChoiceAction> OnDialogueChoiceRequested;


}
