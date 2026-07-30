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
    private TutorialStepSO currentStep;

    [SerializeField] private float hiddenCanvasAlpha = 0f;
    [SerializeField] private float visibleCanvasAlpha = 1f;
    [SerializeField] private float alphaThresholdToCheck = 0.1f;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (canvasGroup != null) canvasGroup.alpha = hiddenCanvasAlpha;
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
        if (canvasGroup == null || canvasGroup.alpha < alphaThresholdToCheck) return;

        
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.forward);
        }

       
        CheckForCloseInput();
    }

    private void CheckForCloseInput()
    {
        bool shouldClose = false;

       
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
            currentStep = step;
            lockFromAutoHide = true;
            Show(step.message);
        }
    }

    public void Show(string message)
    {
        if (tutorialText == null || canvasGroup == null) return;
        tutorialText.text = message;
        StopAllCoroutines();
        StartCoroutine(Fade(visibleCanvasAlpha));
    }

    public void Hide()
    {
        lockFromAutoHide = false;
        currentStep = null; 

        if (canvasGroup == null || !gameObject.activeInHierarchy) return;
        StopAllCoroutines();
        StartCoroutine(Fade(hiddenCanvasAlpha));
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