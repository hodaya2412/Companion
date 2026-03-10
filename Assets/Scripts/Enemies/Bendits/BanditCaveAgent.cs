using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BanditCaveAgent : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cavePoint;
    [SerializeField] private Transform playerTarget;

    [Header("Puzzle Filter")]
    [SerializeField] private string listenedPuzzleId = "ForestStones";

    [Header("Movement")]
    [SerializeField] private float stopDistanceFromPlayer = 2f;

    private NavMeshAgent agent;
    private bool isChasingPlayer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        GameEvents.OnCombatTriggered += GoToPlayer;
        GameEvents.OnPuzzleOpened += HandlePuzzleOpened;
    }

    private void OnDisable()
    {
        GameEvents.OnCombatTriggered -= GoToPlayer;
        GameEvents.OnPuzzleOpened -= HandlePuzzleOpened;
    }

    private void Start()
    {
        ReturnToCave();
    }

    private void Update()
    {
        if (isChasingPlayer && playerTarget != null)
        {
            agent.SetDestination(playerTarget.position);
        }
    }

    private void HandlePuzzleOpened(string puzzleId)
    {
        if (puzzleId != listenedPuzzleId) return;
        ReturnToCave();
    }

    public void GoToPlayer()
    {
        if (playerTarget == null) return;

        isChasingPlayer = true;
        agent.isStopped = false;
        agent.stoppingDistance = stopDistanceFromPlayer;
        agent.SetDestination(playerTarget.position);
    }

    public void ReturnToCave()
    {
        if (cavePoint == null) return;

        isChasingPlayer = false;
        agent.isStopped = false;
        agent.stoppingDistance = 0f;
        agent.SetDestination(cavePoint.position);
    }
}