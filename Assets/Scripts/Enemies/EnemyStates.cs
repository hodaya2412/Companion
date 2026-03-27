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
    public EnemyHoldingState(EnemyBrain brain) => this.brain = brain;

    public override void Enter()
    {
        // מאפסים את הטיימר כדי שהעדכון הראשון יקרה מיד
        timer = 0f;
    }

    public override void Execute()
    {
        if (brain.GetCurrentGameplayState() != GameplayState.Combat) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = brain.holdUpdateInterval;

            Vector3 playerPos = brain.movement.GetPlayerPosition();
            Vector3 dir = (brain.transform.position - playerPos).normalized;
            if (dir.sqrMagnitude < 0.01f) dir = brain.transform.right;

            Vector3 targetPos = playerPos + dir * brain.holdDistance;
            Vector3 sideDir = Vector3.Cross(Vector3.up, dir).normalized;
            targetPos += sideDir * (Random.value < 0.5f ? -1f : 1f) * brain.holdSideStepDistance;

            brain.movement.MoveToPosition(targetPos, 0.8f);
            brain.TryBecomeActive();
        }
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

// --- מצב תקיפה ---
public class EnemyAttackingState : IEnemyState
{
    private EnemyBrain brain;
    public EnemyAttackingState(EnemyBrain brain) => this.brain = brain;

    public override void Execute()
    {
        // אם השחקן ברח בזמן שיצאנו להתקפה
        if (!brain.movement.IsPlayerInRange(brain.combat.attackRange + brain.playerAttackAllowance))
        {
            brain.ChangeState(brain.ChasingState);
            return;
        }

        brain.combat.TryAttack();

        // אחרי התקפה תמיד חוזרים להמתין (Waiting יגריל טיימר חדש ב-Enter)
        brain.ChangeState(brain.WaitingState);
    }
}