using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BanditCaveAgent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cavePoint;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private Animator animator;

    [Header("2.5D Visuals")]
    [SerializeField] private Transform characterVisuals;
    private bool facingRight = true;

    [Header("Movement")]
    [SerializeField] private float stopDistanceFromPlayer = 2f;

    [SerializeField] private float flipThreshold = 0.35f;
    [SerializeField] private float flipCooldown = 0.25f;

    private float lastFlipTime;

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
        HandleAnimation();
    }

    private void HandleVisualFlip()
    {
        if (characterVisuals == null || agent == null) return;
        if (Time.time < lastFlipTime + flipCooldown) return;

        float velocityX = agent.velocity.x;

        if (velocityX > flipThreshold && !facingRight)
        {
            Flip();
        }
        else if (velocityX < -flipThreshold && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = characterVisuals.localScale;
        scale.x = Mathf.Abs(scale.x) * (facingRight ? 1 : -1);
        characterVisuals.localScale = scale;

        lastFlipTime = Time.time;
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

    // שים לב: מחקנו את הפרמטר walk שהיה כאן כי עכשיו הכל זו הליכה
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

    private void HandleAnimation()
    {
        if (animator == null || agent == null) return;

        // בודק אם הסוכן לא עצור ויש לו מהירות תנועה
        bool isMoving = !agent.isStopped && agent.velocity.magnitude > 0.1f;

        // מעדכן רק את ההליכה. שורת ה-IsRunning נמחקה!
        animator.SetBool("IsWalking", isMoving);
    }

    public void KnockBackFrom(Vector3 sourcePosition, float distance = 0.6f)
    {
        if (agent == null || !agent.enabled) return;

        Vector3 dir = (transform.position - sourcePosition).normalized;
        Vector3 target = transform.position + dir * distance;

        agent.Warp(target);
    }

    public Vector3 GetKnockbackSourcePosition()
    {
        return playerTarget != null ? playerTarget.position : transform.position;
    }
}