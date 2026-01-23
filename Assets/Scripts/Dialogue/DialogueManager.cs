using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI")]
    public GameObject panel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI bodyText;
    public Image portraitImage; 

    [Header("Player")]
    public PlayerMovement playerMovement; 

    [Header("Typing")]
    public float charsPerSecond = 45f;

    private DialogueAsset current;
    private int index;
    private Coroutine typingRoutine;
    private bool isTyping;


    InputActions inputAction;
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (panel != null)
            panel.SetActive(false);


        inputAction = new InputActions();
    }

    void OnEnable()
    {
        inputAction.Dialogue.Enable();
        inputAction.Dialogue.NextDialogue.performed += OnAdvance;
    }

    void OnDisable()
    {
        inputAction.Dialogue.NextDialogue.performed -= OnAdvance;
        inputAction.Dialogue.Disable();
    }

    private void OnAdvance(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (panel == null || !panel.activeSelf) return;
        Next();
    }


    public void StartDialogue(DialogueAsset dialogue)
    {
        if (dialogue == null || dialogue.lines.Count == 0) return;

        current = dialogue;
        index = 0;

        GameStateManager.Instance.SetState(GameState.Dialogue);

        panel.SetActive(true);
        ShowLine();
    }

    public void EndDialogue(DialogueAction followUpAction = null)
    {
        if (typingRoutine != null) StopCoroutine(typingRoutine);

        panel.SetActive(false);

        if (playerMovement != null)
            playerMovement.SetMovementEnabled(true);

        // הפעלת פעולות סיום דיאלוג
        if (current != null && current.endActions != null && current.endActions.Count > 0)
        {
            foreach (var action in current.endActions)
                if (action != null) action.Execute();
        }

        // קביעת מצב המשחק אחרי דיאלוג
        if (followUpAction is GuideToFirstDoorAction)
        {
            GameStateManager.Instance.SetState(GameState.BeingGuided);
        }
        else
        {
            GameStateManager.Instance.SetState(GameState.Playing);
        }

        current = null;
        index = 0;
        isTyping = false;
        typingRoutine = null;
    }

    public void SkipDialogue()
    {
        if (current == null) return;
        EndDialogue();
    }

    private void Next()
    {
        if (current == null) return;

        if (isTyping)
        {
            FinishTypingInstant();
            return;
        }

        index++;
        if (index >= current.lines.Count)
        {
            EndDialogue();
            return;
        }

        ShowLine();
    }

    private void ShowLine()
    {
        var line = current.lines[index];

        if (nameText != null) nameText.text = line.speaker;

        if (portraitImage != null)
        {
            if (line.portrait != null)
            {
                portraitImage.gameObject.SetActive(true);
                portraitImage.sprite = line.portrait;
            }
            else
            {
                portraitImage.gameObject.SetActive(false);
            }
        }

        if (typingRoutine != null) StopCoroutine(typingRoutine);
        typingRoutine = StartCoroutine(TypeLine(line.text));
    }

    private IEnumerator TypeLine(string text)
    {
        isTyping = true;
        if (bodyText != null) bodyText.text = "";

        float delay = 1f / Mathf.Max(1f, charsPerSecond);

        for (int i = 0; i < text.Length; i++)
        {
            bodyText.text += text[i];
            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
        typingRoutine = null;
    }

    private void FinishTypingInstant()
    {
        if (typingRoutine != null) StopCoroutine(typingRoutine);

        bodyText.text = current.lines[index].text;
        isTyping = false;
        typingRoutine = null;
    }
    

    
}