using UnityEngine;


public class EnemyReturningState : IEnemyState
{
    private EnemyBrain brain;
    public EnemyReturningState(EnemyBrain brain) => this.brain = brain;

    public override void Enter()
    {
        
        brain.movement.ReturnToCave();
    }

    public override void Execute()
    {
        
        if (brain.movement.HasReachedCave())
        {
            brain.movement.StopMoving();
        }
    }
}


public class EnemyHoldingState : IEnemyState
{
    private EnemyBrain brain;
    private float timer;

    private Vector3 currentHoldPosition;
    private bool hasPosition = false;

    private const float INITIAL_TIMER = 0f;
    private const float HOLD_TIMER_MIN = 2f;
    private const float HOLD_TIMER_MAX = 4f;
    private const float DIR_SQR_MAGNITUDE_THRESHOLD = 0.01f;
    private const float CHASE_MOVEMENT_SPEED_MULTIPLIER = 0.5f;
    private const float REACH_POSITION_TOLERANCE = 0.7f;
    public EnemyHoldingState(EnemyBrain brain) => this.brain = brain;

    public override void Enter()
    {
        timer = INITIAL_TIMER;
        hasPosition = false;
    }

    public override void Execute()
    {
        if (brain.GetCurrentGameplayState() != GameplayState.Combat)
            return;

     
        if (brain.movement.IsPlayerInRange(brain.combat.attackRange + brain.playerAttackAllowance))
        {
            brain.ChangeState(brain.AttackingState);
            return;
        }

        timer -= Time.deltaTime;

        
        if (timer <= 0f || !hasPosition)
        {
            timer = Random.Range(HOLD_TIMER_MIN, HOLD_TIMER_MAX);
            Vector3 playerPos = brain.movement.GetPlayerPosition();
            Vector3 dir = (brain.transform.position - playerPos).normalized;

            if (dir.sqrMagnitude < DIR_SQR_MAGNITUDE_THRESHOLD) dir = brain.transform.right;

            Vector3 sideDir = Vector3.Cross(Vector3.up, dir).normalized;
            float side = Random.value < 0.5f ? -1f : 1f;

            currentHoldPosition = playerPos + dir * brain.holdDistance + sideDir * side * brain.holdSideStepDistance;
            hasPosition = true;
            brain.movement.MoveToPosition(currentHoldPosition, DIR_SQR_MAGNITUDE_THRESHOLD);
        }

        if (brain.movement.HasReachedPosition(currentHoldPosition, REACH_POSITION_TOLERANCE))
        {
            brain.movement.StopMoving();
        }

        brain.TryBecomeActive();
    }
}

public class EnemyChasingState : IEnemyState
{
    private EnemyBrain brain;
    private float timer;

    private const float INITIAL_TIMER = 0f;
    private const float CHASE_MOVEMENT_SPEED_MULTIPLIER = 1.0f;
    public EnemyChasingState(EnemyBrain brain) => this.brain = brain;

    public override void Enter()
    {
       
        timer = INITIAL_TIMER;
    }

    public override void Execute()
    {
      
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

       
        brain.movement.MoveToPosition(brain.CurrentChaseTarget, CHASE_MOVEMENT_SPEED_MULTIPLIER);
    }
}
   
    public class EnemyWaitingState : IEnemyState
{
    private EnemyBrain brain;
    private float waitTimer;
    public EnemyWaitingState(EnemyBrain brain) => this.brain = brain;

    public override void Enter()
    {
       
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

   
    private const float ATTACK_ANIMATION_DURATION = 0.8f;
    private const float INITIAL_TIMER = 0f;
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

            
            if (!brain.movement.IsPlayerInRange(brain.combat.attackRange + brain.playerAttackAllowance))
            {
                brain.ChangeState(brain.ChasingState);
                return;
            }

            if (attackDurationTimer <= 0f)
            {
              
                EvaluateNextAction();
            }
        }
    }

    private void ExecuteAttackChain()
    {
        if (brain.movement.IsPlayerInRange(brain.combat.attackRange + brain.playerAttackAllowance))
        {
            brain.movement.StopMoving(); 
            brain.combat.TryAttack();   

            attackDurationTimer = ATTACK_ANIMATION_DURATION;
            hasAttacked = true;
        }
        else
        {
          
            brain.ChangeState(brain.ChasingState);
        }
    }

    private void EvaluateNextAction()
    {
        
        if (brain.movement.IsPlayerInRange(brain.combat.attackRange + brain.playerAttackAllowance) && brain.combat.CanAttack())
        {
            ExecuteAttackChain();
        }
        
        else if (brain.movement.IsPlayerInRange(brain.combat.attackRange + brain.playerAttackAllowance))
        {
            brain.movement.StopMoving();
        }
        
        else
        {
            brain.ChangeState(brain.ChasingState);
        }
    }
}