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

        // שלב 1: להגיע לנקודת עצירה ליד הדלת
        if (!waitingAtDoor)
        {
            if (HasArrived())
            {
                StopAgent();
                waitingAtDoor = true;
                if (!playedArrivalDialogue && arrivalDialogue != null && DialogueManager.Instance != null)
                {
                    playedArrivalDialogue = true;
                    DialogueManager.Instance.StartDialogue(arrivalDialogue);
                }
            }
            return;
        }

        
        if (player != null)
        {
            float d = Vector3.Distance(transform.position, player.position);
            if (d <= waitForPlayerRadius)
            {
               
                guiding = false;
                waitingAtDoor = false;

                
                agent.enabled = false;
                GameEvents.OnCompanionFollowEnabled?.Invoke(true);
            }
        }
    }

    void HandleDialogueEvent(string eventId)
    {
        if (eventId == "GuideToFirstDoor" && firstDoorStop != null)
        {
            playedArrivalDialogue = false;
            // מתחילים הובלה: מכבים Follow ומדליקים NavMeshAgent
            GameEvents.OnCompanionFollowEnabled?.Invoke(false);

            agent.enabled = true;
            agent.isStopped = false;

            guiding = true;
            waitingAtDoor = false;

            agent.SetDestination(firstDoorStop.position);
        }
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
