using UnityEngine;
using UnityEngine.UI;

public class DialogueChoiceUI : MonoBehaviour
{
    public GameObject panel;
    public Button yesButton;
    public Button noButton;
    public DialogueChoiceAction onlyForChoice;

    private DialogueChoiceAction currentChoice;

    void OnEnable()
    {
        GameEvents.OnDialogueChoiceRequested += ShowChoice;
    }

    void OnDisable()
    {
        GameEvents.OnDialogueChoiceRequested -= ShowChoice;
    }

    void ShowChoice(DialogueChoiceAction choice)
    {
        if (onlyForChoice != null && choice != onlyForChoice)
            return;
        Debug.Log("ShowChoice triggered! Positive: " + choice.positiveText + ", Negative: " + choice.negativeText);

        panel.SetActive(true);
        currentChoice = choice;

        // אם את משתמשת ב-TextMeshPro, שימי לב להחליף את השורות האלו ל-TMP
        var yesText = yesButton.GetComponentInChildren<UnityEngine.UI.Text>();
        var noText = noButton.GetComponentInChildren<UnityEngine.UI.Text>();

        if (yesText != null) yesText.text = choice.positiveText;
        if (noText != null) noText.text = choice.negativeText;

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(() => OnChoiceSelected(true));
        noButton.onClick.AddListener(() => OnChoiceSelected(false));
    }

    void OnChoiceSelected(bool positive)
    {
        panel.SetActive(false);

        if (positive && currentChoice.positiveAction != null)
            currentChoice.positiveAction.Execute();
        else if (!positive && currentChoice.negativeAction != null)
            currentChoice.negativeAction.Execute();

        currentChoice = null;
    }
}