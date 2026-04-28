using UnityEngine;

public class ScenePortal : MonoBehaviour
{
    [Header("Settings")]
    public string sceneToLoad = "VillageScene";
    public string requiredFlag = "TalkedToGuard";

    private bool isTransitioning = false; // מונע קריאה כפולה בזמן שה-Fade כבר עובד

    private void OnTriggerEnter(Collider other)
    {
        // בדיקה שהשחקן נכנס ושאנחנו לא בתהליך מעבר כרגע
        if (other.CompareTag("Player") && !isTransitioning)
        {
            TryEnterPortal();
        }
    }

    private void TryEnterPortal()
    {
        // בדיקה שהדגל מהדיאלוג קיים ודלוק
        if (GameStateManager.Instance != null)
        {
            if (GameStateManager.Instance.GetFlag(requiredFlag))
            {
                LoadNextScene();
            }
            else
            {
                Debug.Log("המעבר חסום: דרוש דיאלוג קודם.");
            }
        }
        else
        {
            // אם אין GameStateManager, פשוט נעבור סצנה ליתר ביטחון
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        if (SceneFader.Instance != null)
        {
            isTransitioning = true;
            // שימוש בפונקציה החכמה של ה-Fader שלך
            SceneFader.Instance.LoadScene(sceneToLoad);
        }
        else
        {
            // גיבוי למקרה שה-Fader לא נמצא בסצנה
            Debug.LogWarning("SceneFader Instance not found! Using direct load.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
        }
    }
}