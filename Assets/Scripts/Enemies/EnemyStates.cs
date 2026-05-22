using UnityEngine;

// --- מצב חזרה למערה ---
public class EnemyReturningState : IEnemyState
{
    private EnemyBrain brain;
    public EnemyReturningState(EnemyBrain brain) => this.brain = brain;

    public override void Enter()
    {
        // קוראים לפקודת התנועה רק פעם אחת כשנכנסים למצב
        brain.movement.ReturnToCave();
    }

    public override void Execute()
    {
        // בודקים בכל פריים רק אם הגענו ליעד כדי לעצור
        if (brain.movement.HasReachedCave())
        {
            brain.movement.StopMoving();
        }
    }
}

// --- מצב החזקה (איגוף/סיבוב סביב השחקן) ---
public class EnemyHoldingState : IEnemyState
{
    private EnemyBrain brain;
    private float timer;

    private Vector3 currentHoldPosition;
    private bool hasPosition = false;

    public EnemyHoldingState(EnemyBrain brain) => this.brain = brain;

    public override void Enter()
    {
        timer = 0f;
        hasPosition = false;
    }

    public override void Execute()
    {
        if (brain.GetCurrentGameplayState() != GameplayState.Combat)
            return;

        timer -= Time.deltaTime;

        // רק מדי פעם בוחרים נקודה חדשה
        if (timer <= 0f || !hasPosition)
        {
            timer = Random.Range(2f, 4f);

            Vector3 playerPos = brain.movement.GetPlayerPosition();

            Vector3 dir = (brain.transform.position - playerPos).normalized;

            if (dir.sqrMagnitude < 0.01f)
                dir = brain.transform.right;

            Vector3 sideDir = Vector3.Cross(Vector3.up, dir).normalized;

            // בחירת צד אקראי
            float side = Random.value < 0.5f ? -1f : 1f;

            currentHoldPosition =
                playerPos +
                dir * brain.holdDistance +
                sideDir * side * brain.holdSideStepDistance;

            hasPosition = true;

            brain.movement.MoveToPosition(currentHoldPosition, 0.5f, true);
        }

        // אם הגיע לנקודה -> לעצור ולעמוד
        if (brain.movement.HasReachedPosition(currentHoldPosition, 0.7f))
        {
            brain.movement.StopMoving();
        }

        // מדי פעם מנסים להפוך לאקטיביים
        brain.TryBecomeActive();
    }
}
// --- מצב מרדף ---
public class EnemyChasingState : IEnemyState
{
    private EnemyBrain brain;
    private float timer;
    public EnemyChasingState(EnemyBrain brain) => this.brain = brain;

    public override void Enter()
    {
        // חשוב: מאפסים טיימר כדי שלא יחכה מהפעם הקודמת שהיה במצב הזה
        timer = 0f;
    }

    public override void Execute()
    {
        if (!brain.HasAttackSlot) { brain.ChangeState(brain.HoldingState); return; }
        if (brain.IsTooFarToKeepSlot()) { brain.ReleaseAttackSlotIfNeeded(); brain.ChangeState(brain.HoldingState); return; }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = brain.chaseUpdateInterval;
            brain.CurrentChaseTarget = EnemyAttackCoordinator.Instance != null ?
                EnemyAttackCoordinator.Instance.GetAttackPosition(brain) : brain.movement.GetPlayerPosition();
        }

        brain.movement.MoveToPosition(brain.CurrentChaseTarget, 1.0f);

        if (brain.movement.HasReachedPosition(brain.CurrentChaseTarget, 1.0f) &&
            brain.movement.IsPlayerInRange(brain.combat.attackRange + brain.playerAttackAllowance))
        {
            brain.ChangeState(brain.WaitingState);
        }
    }
}

// --- מצב המתנה לתקיפה ---
public class EnemyWaitingState : IEnemyState
{
    private EnemyBrain brain;
    private float waitTimer;
    public EnemyWaitingState(EnemyBrain brain) => this.brain = brain;

    public override void Enter()
    {
        // הגרלת זמן ההמתנה קורית פעם אחת בכניסה
        waitTimer = Random.Range(brain.waitBeforeAttackMin, brain.waitBeforeAttackMax);
        brain.movement.StopMoving();
    }

    public override void Execute()
    {
        if (!brain.HasAttackSlot) { brain.ChangeState(brain.HoldingState); return; }

        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0f)
        {
            brain.ChangeState(brain.AttackingState);
        }
    }
}

public class EnemyAttackingState : IEnemyState
{
    private EnemyBrain brain;
    private bool isRetreating = false;
    private float retreatTimer = 0f;
    private const float RETREAT_DURATION = 0.5f; // כמה זמן הוא יברח אחורה

    public EnemyAttackingState(EnemyBrain brain) => this.brain = brain;

    public override void Enter()
    {
        isRetreating = false;

        // 1. מבצעים את התקיפה מיד בכניסה למצב
        if (brain.movement.IsPlayerInRange(brain.combat.attackRange + brain.playerAttackAllowance))
        {
            brain.combat.TryAttack();
            StartRetreat();
        }
        else
        {
            // אם הוא יצא מטווח רגע לפני, פשוט חוזרים לרדוף
            brain.ChangeState(brain.ChasingState);
        }
    }

    private void StartRetreat()
    {
        isRetreating = true;
        retreatTimer = RETREAT_DURATION;

        // חישוב נקודת נסיגה
        Vector3 playerPos = brain.movement.GetPlayerPosition();
        Vector3 retreatDir = (brain.transform.position - playerPos).normalized;
        Vector3 retreatTarget = brain.transform.position + retreatDir * 3f;

        // פקודה לזוז לנקודה
        brain.movement.MoveToPosition(retreatTarget, 0.1f);
    }

    public override void Execute()
    {
        if (isRetreating)
        {
            retreatTimer -= Time.deltaTime;
            if (retreatTimer <= 0f)
            {
                // רק אחרי שסיים לסגת, עוברים להמתנה
                brain.ChangeState(brain.WaitingState);
            }
        }
    }
}
