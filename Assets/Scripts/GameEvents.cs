using System;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public static class GameEvents
{
    public static Action<DialogueAction> OnDialogueEvent;
    public static Action<InventoryItemData> OnItemClicked;

    // Gameplay State
    public static Action<GameplayState> OnGameplayStateChanged;
    public static Action<GameplayState> RequestGameplayStateChange;
    public static Func<GameplayState> RequestCurrentGameplayState;

    // UI State
    public static Action<UIState> OnUIStateChanged;
    public static Action<UIState> RequestUIStateChange;
    public static Func<UIState> RequestCurrentUIState;

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

    public static Action OnDialogueStarted;
    public static Action OnDialogueEnded;

    public static Func<string, bool> RequestFlagState;
    public static Action<string> OnPuzzleOpened;

    public static Action<GameObject, float> OnEnemyHit;
    public static Action<float> OnPlayerHit;
    public static Action<float, float> OnHealthChanged;
}