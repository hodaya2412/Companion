using System;

public static class GameEvents
{
    public static Action <string> OnDialogueEvent;
    public static Action<bool> OnCompanionFollowEnabled;
}
