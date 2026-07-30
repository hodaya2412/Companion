using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public BanditCaveAgent movement;
    [SerializeField] public EnemyCombat combat;
    [SerializeField] public EnemyHealth health;

    [Header("Behavior")]
    public float waitBeforeAttackMin = 0.3f;
    public float waitBeforeAttackMax = 1.2f;

    [Header("Holding")]
    public float holdDistance = 4.5f;
    public float holdTolerance = 1f;
    public float holdUpdateInterval = 0.5f;
    public float holdSideStepDistance = 2f;

    [Header("Slot Logic")]
    public float loseAttackSlotDistance = 8f;

    [Header("Type")]
    [SerializeField] public EnemyTypeData typeData;

    [Header("Chasing")]
    public float chaseUpdateInterval = 0.5f;
    public float attackPositionReachDistance = 0.75f;
    public float playerAttackAllowance = 1.0f;

   
    private IEnemyState currentState;
    public Vector3 CurrentChaseTarget { get; set; }
    public bool HasAttackSlot { get; private set; }
    private GameplayState currentGameplayState;
    private AttackRole currentAttackRole = AttackRole.None;

    
    public EnemyReturningState ReturningState;
    public EnemyHoldingState HoldingState;
    public EnemyChasingState ChasingState;
    public EnemyWaitingState WaitingState;
    public EnemyAttackingState AttackingState;

    private void Awake()
    {
        if (movement == null) movement = GetComponent<BanditCaveAgent>();
        if (combat == null) combat = GetComponent<EnemyCombat>();
        if (health == null) health = GetComponent<EnemyHealth>();

       
        ReturningState = new EnemyReturningState(this);
        HoldingState = new EnemyHoldingState(this);
        ChasingState = new EnemyChasingState(this);
        WaitingState = new EnemyWaitingState(this);
        AttackingState = new EnemyAttackingState(this);
    }

    private void OnEnable()
    {
        GameEvents.OnGameplayStateChanged += HandleGameplayStateChanged;
        GameEvents.OnCombatTriggered += HandleCombatTriggered;
        GameEvents.OnCombatReset += HandleCombatReset;
        currentGameplayState = GameEvents.RequestCurrentGameplayState?.Invoke() ?? GameplayState.Playing;
    }

    private void OnDisable()
    {
        ReleaseAttackSlotIfNeeded();
        GameEvents.OnGameplayStateChanged -= HandleGameplayStateChanged;
        GameEvents.OnCombatTriggered -= HandleCombatTriggered;
        GameEvents.OnCombatReset -= HandleCombatReset;
    }

    private void Start()
    {
        ChangeState(ReturningState);
    }

    private void Update()
    {
        if (health == null || currentState == null) return;
        currentState.Execute();
    }

    public void ChangeState(IEnemyState newState)
    {
        if (newState == null || currentState == newState) return;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

  
    public void SetAttackRole(AttackRole role) => currentAttackRole = role;

  
    public bool PrefersFlank() => typeData != null && typeData.preferFlank;

   
    private void HandleGameplayStateChanged(GameplayState newState)
    {
        currentGameplayState = newState;
        if (newState != GameplayState.Combat) { ReleaseAttackSlotIfNeeded(); ChangeState(ReturningState); }
    }

    private void HandleCombatTriggered() { ReleaseAttackSlotIfNeeded(); ChangeState(HoldingState); }
    private void HandleCombatReset() { ReleaseAttackSlotIfNeeded(); ChangeState(ReturningState); }

    public void ReleaseAttackSlotIfNeeded()
    {
        if (!HasAttackSlot) return;
        if (EnemyAttackCoordinator.Instance != null) EnemyAttackCoordinator.Instance.ReleaseAttackSlot(this);
        HasAttackSlot = false;
        currentAttackRole = AttackRole.None;
    }

    public void TryBecomeActive()
    {
        if (EnemyAttackCoordinator.Instance == null) { ActivateChasing(); return; }
        if (EnemyAttackCoordinator.Instance.TryReserveAttackSlot(this)) ActivateChasing();
    }

    private void ActivateChasing()
    {
        HasAttackSlot = true;
        CurrentChaseTarget = EnemyAttackCoordinator.Instance != null ?
            EnemyAttackCoordinator.Instance.GetAttackPosition(this) : movement.GetPlayerPosition();
        ChangeState(ChasingState);
    }

    public bool IsTooFarToKeepSlot()
    {
        return Vector3.Distance(transform.position, movement.GetPlayerPosition()) > loseAttackSlotDistance;
    }

    public GameplayState GetCurrentGameplayState() => currentGameplayState;
}