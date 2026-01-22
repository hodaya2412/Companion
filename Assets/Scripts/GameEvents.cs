using System;

public static class GameEvents
{
    public static Action<DialogueAction> OnDialogueEvent;

    public static Action<bool> OnCompanionFollowEnabled;
    public static Action OnInventoryChanged;

}
