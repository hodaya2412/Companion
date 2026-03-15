using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CompanionGuideController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;

    [Header("Tuning")]
    public float stopDistance = 1.2f;
    public float waitForPlayerRadius = 2.0f;

    private bool guiding;
    private bool waitingAtTarget;

    private DialogueAsset currentArrivalDialogue;
    private bool playedArrivalDialogue;

    [Header("Scene Targets")]
    public List<GuideTargetMapping> targets = new List<GuideTargetMapping>();
    private Dictionary<GuideTargetID, GuideTargetMapping> targetDict;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        agent.stoppingDistance = stopDistance;
        agent.autoBraking = true;
        agent.enabled = false;

        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.radius = 0.5f;

        targetDict = new Dictionary<GuideTargetID, GuideTargetMapping>();
        foreach (var t in targets)
        {
            if (t.target != null && !targetDict.ContainsKey(t.id))
                targetDict.Add(t.id, t);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnGuideRequested += StartGuiding;
    }

    private void OnDisable()
    {
        GameEvents.OnGuideRequested -= StartGuiding;
    }

    private void Update()
    {
        if (!guiding) return;

        if (!waitingAtTarget)
        {
            if (HasArrived())
            {
                StopAgent();
                waitingAtTarget = true;
            }
            return;
        }

        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= waitForPlayerRadius)
        {
            if (!playedArrivalDialogue && currentArrivalDialogue != null)
            {
                playedArrivalDialogue = true;

                // עוברים ל-UI של דיאלוג
                GameEvents.RequestUIStateChange?.Invoke(UIState.Dialogue);

                // מתחילים את דיאלוג ההגעה
                DialogueManager.Instance.StartDialogue(currentArrivalDialogue);
            }

            guiding = false;
            waitingAtTarget = false;
            agent.enabled = false;

            // חוזרים למצב עולם רגיל
            GameEvents.RequestGameplayStateChange?.Invoke(GameplayState.Playing);

            GameEvents.OnCompanionFollowEnabled?.Invoke(true);
        }
    }

    public void StartGuiding(GuideTargetID targetID)
    {
        if (!targetDict.TryGetValue(targetID, out GuideTargetMapping mapping))
        {
            Debug.LogWarning($"GuideTargetID {targetID} לא נמצא במפה!");
            return;
        }

        currentArrivalDialogue = mapping.arrivalDialogue;
        playedArrivalDialogue = false;

        GameEvents.OnCompanionFollowEnabled?.Invoke(false);

        // מצב עולם: BeingGuided
        GameEvents.RequestGameplayStateChange?.Invoke(GameplayState.BeingGuided);

        agent.enabled = true;
        agent.isStopped = false;

        guiding = true;
        waitingAtTarget = false;

        agent.SetDestination(mapping.target.position);
    }

    private bool HasArrived()
    {
        if (!agent.enabled) return false;
        if (!agent.isOnNavMesh) return false;
        if (agent.pathPending) return false;

        return agent.remainingDistance <= agent.stoppingDistance + 0.05f;
    }

    private void StopAgent()
    {
        if (!agent.enabled) return;

        agent.isStopped = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
    }
}

[System.Serializable]
public class GuideTargetMapping
{
    public GuideTargetID id;
    public Transform target;
    public DialogueAsset arrivalDialogue;
}