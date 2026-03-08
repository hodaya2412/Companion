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

    public static Action<string, bool> OnFlagChanged;
    public static Action<string> OnPuzzleStoneClicked;
    public static Action OnCombatTriggered;
    public static Action<string> OnItemPickedUp;
    public static Action OnMirrorPlacedInForest;

    // חדש - להורדת צמידות
    public static Action OnDialogueStarted;
    public static Action OnDialogueEnded;

    public static Func<GameState> RequestCurrentGameState;
    public static Func<string, bool> RequestFlagState;
}