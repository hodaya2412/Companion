using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BanditCaveAgent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cavePoint;
    [SerializeField] private Transform playerTarget;

    [Header("2.5D Visuals")]
    [Tooltip("גררי לכאן את האובייקט הילד שמכיל את ה-Sprite של האויב")]
    [SerializeField] private Transform characterVisuals;
    private bool facingRight = true;

    [Header("Movement")]
    [SerializeField] private float stopDistanceFromPlayer = 2f;

    private NavMeshAgent agent;
    private bool isChasingPlayer;
    private bool isReturningToCave;

    public Transform PlayerTarget => playerTarget;
    public Transform CavePoint => cavePoint;
    public bool IsReturningToCave => isReturningToCave;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // הגדרה קריטית: מונעים מה-NavMesh לסובב את האויב ב-3D
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
        }
    }

    private void Start()
    {
        ReturnToCave();
    }

    private void Update()
    {
        // מעדכנים את הכיוון הויזואלי בכל פריים לפי המהירות של הסוכן
        HandleVisualFlip();
    }

    private void HandleVisualFlip()
    {
        if (characterVisuals == null || agent == null) return;

        // בודקים אם האויב זז ימינה או שמאלה לפי המהירות שלו ב-NavMesh
        float velocityX = agent.velocity.x;

        if (velocityX > 0.1f && !facingRight)
        {
            Flip();
        }
        else if (velocityX < -0.1f && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        // הופכים רק את הויזואליה ב-180 מעלות
        characterVisuals.localRotation *= Quaternion.Euler(0, 180, 0);
    }

    public void GoToPlayer()
    {
        if (playerTarget == null) return;

        isChasingPlayer = true;
        isReturningToCave = false;

        agent.isStopped = false;
        agent.stoppingDistance = stopDistanceFromPlayer;
        agent.SetDestination(playerTarget.position);
    }

    public void ReturnToCave()
    {
        if (cavePoint == null) return;

        isChasingPlayer = false;
        isReturningToCave = true;

        agent.isStopped = false;
        agent.stoppingDistance = 0f;
        agent.SetDestination(cavePoint.position);
    }

    public void StopMoving()
    {
        isChasingPlayer = false;
        isReturningToCave = false;

        agent.isStopped = true;
        agent.ResetPath();
    }

    public bool IsPlayerInRange(float range)
    {
        if (playerTarget == null) return false;
        return Vector3.Distance(transform.position, playerTarget.position) <= range;
    }

    public bool HasReachedCave(float threshold = 0.5f)
    {
        if (cavePoint == null) return false;
        return Vector3.Distance(transform.position, cavePoint.position) <= threshold;
    }

    public Vector3 GetPlayerPosition()
    {
        if (playerTarget != null)
            return playerTarget.position;

        return transform.position;
    }

    public void MoveToPosition(Vector3 targetPosition, float stoppingDistance = 0.6f)
    {
        if (agent == null || !agent.enabled) return;

        isChasingPlayer = false;
        isReturningToCave = false;

        agent.isStopped = false;
        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(targetPosition);
    }

    public float DistanceToPosition(Vector3 targetPosition)
    {
        return Vector3.Distance(transform.position, targetPosition);
    }

    public bool HasReachedPosition(Vector3 targetPosition, float threshold = 0.6f)
    {
        return DistanceToPosition(targetPosition) <= threshold;
    }
}