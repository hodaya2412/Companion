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

        // עוקף סלוט: השחקן נכנס לי לפרצוף? עוברים ישר לסטייט התקפה
        if (brain.movement.IsPlayerInRange(brain.combat.attackRange + brain.playerAttackAllowance))
        {
            brain.ChangeState(brain.AttackingState);
            return;
        }

        timer -= Time.deltaTime;

        
        if (timer <= 0f || !hasPosition)
        {
            timer = Random.Range(2f, 4f);
            Vector3 playerPos = brain.movement.GetPlayerPosition();
            Vector3 dir = (brain.transform.position - playerPos).normalized;

            if (dir.sqrMagnitude < 0.01f) dir = brain.transform.right;

            Vector3 sideDir = Vector3.Cross(Vector3.up, dir).normalized;
            float side = Random.value < 0.5f ? -1f : 1f;

            currentHoldPosition = playerPos + dir * brain.holdDistance + sideDir * side * brain.holdSideStepDistance;
            hasPosition = true;
            brain.movement.MoveToPosition(currentHoldPosition, 0.5f);
        }

        if (brain.movement.HasReachedPosition(currentHoldPosition, 0.7f))
        {
            brain.movement.StopMoving();
        }

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
        // השחקן בטווח? מעבירים את השליטה ישירות לסטייט התקפה שהוא ינהל את זה!
        if (brain.movement.IsPlayerInRange(brain.combat.attackRange + brain.playerAttackAllowance))
        {
            brain.ChangeState(brain.AttackingState);
            return;
        }

        if (!brain.HasAttackSlot) { brain.ChangeState(brain.HoldingState); return; }
        if (brain.IsTooFarToKeepSlot()) { brain.ReleaseAttackSlotIfNeeded(); brain.ChangeState(brain.HoldingState); return; }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = brain.chaseUpdateInterval;
            brain.CurrentChaseTarget = EnemyAttackCoordinator.Instance != null ?
                EnemyAttackCoordinator.Instance.GetAttackPosition(brain) : brain.movement.GetPlayerPosition();
        }

        // כל עוד הוא לא בטווח, הוא זז בכל פריץ ישירות אל היעד שלו!
        brain.movement.MoveToPosition(brain.CurrentChaseTarget, 1.0f);
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
    private float attackDurationTimer = 0f;
    private bool hasAttacked = false;

    // ⏱️ אורך האנימציה (בשניות). 
    private const float ATTACK_ANIMATION_DURATION = 0.8f;

    public EnemyAttackingState(EnemyBrain brain) => this.brain = brain;

    public override void Enter()
    {
        hasAttacked = false;
        ExecuteAttackChain();
    }

    public override void Execute()
    {
        if (hasAttacked)
        {
            attackDurationTimer -= Time.deltaTime;

            // 🎯 התיקון הקריטי: אם האנימציה עדיין רצה אבל השחקן כבר הספיק להתרחק מהטווח,
            // אנחנו לא מחכים שהאנימציה תסתיים באוויר סתם! חוזרים מיד לרדוף אחריו.
            if (!brain.movement.IsPlayerInRange(brain.combat.attackRange + brain.playerAttackAllowance))
            {
                brain.ChangeState(brain.ChasingState); // 🏃‍♂️ חוזר לרוץ ולצמצם טווח!
                return;
            }

            if (attackDurationTimer <= 0f)
            {
                // האנימציה הנוכחית הסתיימה בהצלחה, בודקים מה הצעד הבא
                EvaluateNextAction();
            }
        }
    }

    private void ExecuteAttackChain()
    {
        // בודקים אם הוא בטווח האמיתי לפגיעה
        if (brain.movement.IsPlayerInRange(brain.combat.attackRange + brain.playerAttackAllowance))
        {
            brain.movement.StopMoving(); // עצירה לצורך הנפת הנשק
            brain.combat.TryAttack();    // מפעיל את האנימציה

            attackDurationTimer = ATTACK_ANIMATION_DURATION;
            hasAttacked = true;
        }
        else
        {
            // 🏃‍♂️ אם הוא לא בטווח בכלל, שיחזור מיד לרדוף במקום לעמוד!
            brain.ChangeState(brain.ChasingState);
        }
    }

    private void EvaluateNextAction()
    {
        // אם השחקן עדיין פה והצינון נגמר - דופקים עוד מכה ברצף
        if (brain.movement.IsPlayerInRange(brain.combat.attackRange + brain.playerAttackAllowance) && brain.combat.CanAttack())
        {
            ExecuteAttackChain();
        }
        // אם הוא פה אבל יש Cooldown, נעמדים לשבריר שנייה ומחכים לצינון
        else if (brain.movement.IsPlayerInRange(brain.combat.attackRange + brain.playerAttackAllowance))
        {
            brain.movement.StopMoving();
        }
        // אם הוא לא בטווח - חוזרים מיד למרדף!
        else
        {
            brain.ChangeState(brain.ChasingState);
        }
    }
}