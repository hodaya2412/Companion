using System;
using UnityEngine;

public static class GameEvents
{
    public static Action<DialogueAction> OnDialogueEvent;
    public static Action<InventoryItemData> OnItemClicked;
    public static Action<GameState> OnStateChanged;        // מודיע לכולם שהמצב השתנה
    public static Action<GameState> RequestStateChange;    // UI מבקש מה-Manager לשנות מצב
    public static Action<bool> OnCompanionFollowEnabled;
    public static Action OnInventoryChanged;
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
    public static Action<string> OnPuzzleOpened;

    public static Action<GameObject, float> OnEnemyHit;
    public static Action<float> OnPlayerHit;
    public static Action <float, float> OnHealthChanged;

}