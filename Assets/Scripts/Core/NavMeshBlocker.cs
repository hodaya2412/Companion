using UnityEngine;
using UnityEngine.AI;

public class NavMeshBlocker : MonoBehaviour
{
    public string flagKey = "Forest_PuzzleSolved";
    public Transform targetPoint;
    public float speed = 3f; 

    private bool isRolling = false;
    private bool hasReachedTarget = false;

    [SerializeField] private float rollRotationSpeedZ = -100f;
    [SerializeField] private float targetReachedThreshold = 0.05f;
    [SerializeField] private float defaultRotationX = 0f;
    [SerializeField] private float defaultRotationY = 0f;
    private void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
           
            rb.isKinematic = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    private void OnEnable() => GameEvents.OnFlagChanged += CheckFlag;
    private void OnDisable() => GameEvents.OnFlagChanged -= CheckFlag;

    private void Start()
    {
        if (GameStateManager.Instance != null && GameStateManager.Instance.GetFlag(flagKey))
        {
            transform.position = targetPoint.position;
            FinishAndBlock();
        }
    }

    private void CheckFlag(string key, bool value)
    {
        if (key == flagKey && value == true && !hasReachedTarget)
        {
            isRolling = true;
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true; 
        }
    }

    private void Update()
    {
        if (isRolling && targetPoint != null)
        {
           
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

            
           
            transform.Rotate(defaultRotationX, defaultRotationY, rollRotationSpeedZ * Time.deltaTime, Space.Self);

          
            if (Vector3.Distance(transform.position, targetPoint.position) < targetReachedThreshold)
            {
                FinishAndBlock();
            }
        }
    }

    private void FinishAndBlock()
    {
        isRolling = false;
        hasReachedTarget = true;

       
        transform.position = targetPoint.position;

        
        if (GetComponent<NavMeshObstacle>() == null)
        {
            NavMeshObstacle obs = gameObject.AddComponent<NavMeshObstacle>();
            obs.carving = true;
            obs.carveOnlyStationary = true;
        }
    }
}