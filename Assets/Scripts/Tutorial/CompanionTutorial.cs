using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CompanionTutorial : MonoBehaviour
{
    public static CompanionTutorial Instance;

    public CanvasGroup canvasGroup;
    public TextMeshProUGUI tutorialText;
    public float fadeSpeed = 3f;

    private bool lockFromAutoHide = false;
    private TutorialStepSO currentStep; // שומר את הצעד הנוכחי כדי לדעת איזה מקשים לבדוק

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (canvasGroup != null) canvasGroup.alpha = 0;
    }

    private void OnEnable() { GameEvents.OnDialogueEnded += HandleDialogueEnded; }
    private void OnDisable() { GameEvents.OnDialogueEnded -= HandleDialogueEnded; }

    private void HandleDialogueEnded()
    {
        if (lockFromAutoHide)
        {
            lockFromAutoHide = false;
            return;
        }
        Hide();
    }

    private void Update()
    {
        if (canvasGroup == null || canvasGroup.alpha < 0.1f) return;

        // 1. Billboard - פונה למצלמה
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.forward);
        }

        // 2. בדיקת לחיצה על מקשים לסגירה
        CheckForCloseInput();
    }

    private void CheckForCloseInput()
    {
        bool shouldClose = false;

        // אם יש צעד פעיל עם מקשים ספציפיים (כמו WASD)
        if (currentStep != null && currentStep.keysToClose != null && currentStep.keysToClose.Count > 0)
        {
            foreach (KeyCode k in currentStep.keysToClose)
            {
                if (Input.GetKeyDown(k))
                {
                    shouldClose = true;
                    break;
                }
            }
        }
        else
        {
            // ברירת מחדל אם אין מקשים ספציפיים (E, Enter)
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.E))
            {
                shouldClose = true;
            }
        }

        if (shouldClose)
        {
            Hide();
        }
    }

    public void ShowStep(TutorialStepSO step)
    {
        if (step != null)
        {
            currentStep = step; // עדכון הצעד הנוכחי
            lockFromAutoHide = true;
            Show(step.message);
        }
    }

    public void Show(string message)
    {
        if (tutorialText == null || canvasGroup == null) return;
        tutorialText.text = message;
        StopAllCoroutines();
        StartCoroutine(Fade(1));
    }

    public void Hide()
    {
        lockFromAutoHide = false;
        currentStep = null; // איפוס הצעד כשסוגרים

        if (canvasGroup == null || !gameObject.activeInHierarchy) return;
        StopAllCoroutines();
        StartCoroutine(Fade(0));
    }

    private IEnumerator Fade(float target)
    {
        while (!Mathf.Approximately(canvasGroup.alpha, target))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime * fadeSpeed);
            yield return null;
        }
        canvasGroup.alpha = target;
    }
}