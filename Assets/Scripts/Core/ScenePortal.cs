using UnityEngine;

public class ScenePortal : MonoBehaviour
{
    [Header("Settings")]
    public string sceneToLoad = "VillageScene";
    public string requiredFlag = "TalkedToGuard";

    private bool isTransitioning = false; 

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player") && !isTransitioning)
        {
            TryEnterPortal();
        }
    }

    private void TryEnterPortal()
    {
        
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
            
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        if (SceneFader.Instance != null)
        {
            isTransitioning = true;
            
            SceneFader.Instance.LoadScene(sceneToLoad);
        }
        else
        {
           
            Debug.LogWarning("SceneFader Instance not found! Using direct load.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
        }
    }
}