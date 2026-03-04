using UnityEngine;
using UnityEngine.AI;

public class NavMeshBlocker : MonoBehaviour
{
    public string flagKey = "Forest_PuzzleSolved";
    public Transform targetPoint;

    private NavMeshAgent agent;
    private bool hasReachedTarget = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        // השורה הכי חשובה: מונעת מה-Agent לסובב את האבן
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
    }

    private void OnEnable() => GameEvents.OnFlagChanged += CheckFlag;
    private void OnDisable() => GameEvents.OnFlagChanged -= CheckFlag;

    private void Start()
    {
        if (GameStateManager.Instance.GetFlag(flagKey))
        {
            if (targetPoint != null)
            {
                agent.Warp(targetPoint.position);
                FinishAndBlock();
            }
        }
    }

    private void CheckFlag(string key, bool value)
    {
        if (key == flagKey && value == true && agent.enabled)
        {
            agent.SetDestination(targetPoint.position);
        }
    }

    private void Update()
    {
        // תנועה חלקה ליעד בלי סיבובים
        if (agent.enabled && agent.hasPath && agent.remainingDistance <= 0.1f && !hasReachedTarget)
        {
            FinishAndBlock();
        }
    }

    private void FinishAndBlock()
    {
        hasReachedTarget = true;
        agent.enabled = false;

        // הפיכה למכשול (Obstacle) כדי שהשודדים לא יעברו דרכה
        if (gameObject.GetComponent<NavMeshObstacle>() == null)
        {
            var obs = gameObject.AddComponent<NavMeshObstacle>();
            obs.carving = true;
        }
    }
}