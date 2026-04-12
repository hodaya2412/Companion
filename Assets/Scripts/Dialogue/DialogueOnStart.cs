using UnityEngine;
using System.Collections;

public class DialogueOnStart : MonoBehaviour
{
    public DialogueAsset dialogue;
    public float delay = 0.3f;

    private IEnumerator Start()
    {
        if (dialogue == null) yield break;

        string id = string.IsNullOrWhiteSpace(dialogue.dialogueId) ? dialogue.name : dialogue.dialogueId;

        bool alreadyPlayed = GameEvents.RequestFlagState?.Invoke(id) ?? false;
        if (alreadyPlayed) yield break;

        //  מחכה שכל המערכות יעלו
        yield return null;
        yield return null;

        //  הדיליי שלך
        yield return new WaitForSeconds(delay);

        Begin();
    }

    void Begin()
    {
        GameEvents.OnDialogueRequested?.Invoke(dialogue);
    }
}