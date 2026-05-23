using UnityEngine;

public class EndDemoTrigger : MonoBehaviour
{
    [SerializeField] private GameObject endDemoPanel;

    private void OnEnable()
    {
        GameEvents.OnDemoCompleted += ShowEndDemo;
    }

    private void OnDisable()
    {
        GameEvents.OnDemoCompleted -= ShowEndDemo;
    }

    private void Start()
    {
        endDemoPanel.SetActive(false);
    }

    
    private void ShowEndDemo()
    {
        Debug.Log("END DEMO EVENT RECEIVED");

        endDemoPanel.SetActive(true);
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}