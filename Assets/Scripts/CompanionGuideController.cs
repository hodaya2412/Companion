using UnityEngine;
using UnityEngine.AI;

public class CompanionGuideController : MonoBehaviour
{
    public NavMeshAgent agent;

    [Header("Targets")]
    public Transform firstDoorStop;
    public Transform player;

    [Header("Tuning")]
    public float stopDistance = 1.2f;
    public float waitForPlayerRadius = 2.0f;

    private bool guiding;
    private bool waitingAtDoor;

    [Header("Arrival Dialogue")]
    public DialogueAsset arrivalDialogue;
    private bool playedArrivalDialogue;

    [Header("Dialogue Actions")]
    public GuideToFirstDoorAction guideToFirstDoorAction;



    void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stopDistance;
        agent.autoBraking = true;

        
        agent.enabled = false;
    }

    void OnEnable()
    {
        GameEvents.OnDialogueEvent += HandleDialogueEvent;
    }

    void OnDisable()
    {
        GameEvents.OnDialogueEvent -= HandleDialogueEvent;
    }

    void Update()
    {
        if (!guiding) return;


        if (!waitingAtDoor)
        {
            if (HasArrived())
            {
                StopAgent();
                waitingAtDoor = true;
            }
            return;
        }


        if (player != null)
        {
            float d = Vector3.Distance(transform.position, player.position);

            if (d <= waitForPlayerRadius)
            {
                // אם עדיין לא שוחק דיאלוג ההגעה
                if (!playedArrivalDialogue && arrivalDialogue != null && DialogueManager.Instance != null)
                {
                    playedArrivalDialogue = true;

                    // קודם לשנות את מצב המשחק ל-Dialogue
                    GameStateManager.Instance.SetState(GameState.Dialogue);

                    Debug.Log("Starting arrival dialogue at the door!");
                    DialogueManager.Instance.StartDialogue(arrivalDialogue);
                }

                
                guiding = false;
                waitingAtDoor = false;
                agent.enabled = false;

              
                GameEvents.OnCompanionFollowEnabled?.Invoke(true);
               
            }
        }
    }

        void HandleDialogueEvent(DialogueAction action)
    {
        if (action != guideToFirstDoorAction) return;

        playedArrivalDialogue = false;

        GameEvents.OnCompanionFollowEnabled?.Invoke(false);

        agent.enabled = true;
        agent.isStopped = false;

        guiding = true;
        waitingAtDoor = false;

        agent.SetDestination(firstDoorStop.position);

 
    }


    bool HasArrived()
    {
        if (!agent.enabled) return false;
        if (agent.pathPending) return false;

        float direct = Vector3.Distance(transform.position, agent.destination);

        bool okRemaining = agent.remainingDistance <= agent.stoppingDistance + 0.05f;
        bool okDirect = direct <= agent.stoppingDistance + 0.1f;

        return okRemaining || okDirect;
    }

    void StopAgent()
    {
        if (!agent.enabled) return;

        agent.isStopped = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
    }
}
