using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI")]
    public GameObject panel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI bodyText;
    public Image portraitImage;

    [Header("Typing")]
    public float charsPerSecond = 45f;

    [SerializeField] private int initialDialogueIndex = 0;
    [SerializeField] private float minCharsPerSecondLimit = 1f;
    [SerializeField] private float baseTimeDivider = 1f;

    private DialogueAsset current;
    private int index;
    private Coroutine typingRoutine;
    private bool isTyping;

    private InputActions inputAction;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (panel != null)
            panel.SetActive(false);

        inputAction = new InputActions();
    }

    void OnEnable()
    {
        inputAction.Dialogue.Enable();
        inputAction.Dialogue.NextDialogue.performed += OnAdvance;
        GameEvents.OnDialogueRequested += StartDialogue;
    }

    void OnDisable()
    {
        inputAction.Dialogue.NextDialogue.performed -= OnAdvance;
        inputAction.Dialogue.Disable();
        GameEvents.OnDialogueRequested -= StartDialogue;
    }

    private void OnAdvance(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (panel == null || !panel.activeSelf) return;
        Next();
    }

    public void StartDialogue(DialogueAsset dialogue)
    {
        if (dialogue == null || dialogue.lines == null || dialogue.lines.Count == 0)
            return;

        EventSystem.current?.SetSelectedGameObject(null);

        current = dialogue;
        index = initialDialogueIndex;

        GameEvents.OnDialogueStarted?.Invoke();
        GameEvents.RequestUIStateChange?.Invoke(UIState.Dialogue);

        if (panel != null)
            panel.SetActive(true);

        ShowLine();
    }

    public void EndDialogue()
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        if (panel != null)
            panel.SetActive(false);

        if (current != null && current.endActions != null)
        {
            foreach (var action in current.endActions)
            {
                if (action != null)
                    action.Execute();
            }
        }

        GameEvents.OnDialogueEnded?.Invoke();
        GameEvents.RequestUIStateChange?.Invoke(UIState.None);

        current = null;
        index = initialDialogueIndex;
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
        if (current == null || index < initialDialogueIndex || index >= current.lines.Count) return;

        var line = current.lines[index];

        if (nameText != null)
            nameText.text = line.speaker;

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

        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(TypeLine(line.text));
    }

    private IEnumerator TypeLine(string text)
    {
        isTyping = true;

        if (bodyText != null)
            bodyText.text = "";

        float delay = 1f / Mathf.Max(minCharsPerSecondLimit, charsPerSecond);

        for (int i = 0; i < text.Length; i++)
        {
            if (bodyText != null)
                bodyText.text += text[i];

            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
        typingRoutine = null;
    }

    private void FinishTypingInstant()
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        if (bodyText != null && current != null)
            bodyText.text = current.lines[index].text;

        isTyping = false;
        typingRoutine = null;
    }
}