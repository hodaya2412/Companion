using UnityEngine;
using UnityEngine.AI;

public class NavMeshBlocker : MonoBehaviour
{
    public string flagKey = "Forest_PuzzleSolved";
    public Transform targetPoint;
    public float speed = 3f; // מהירות גלגול יציבה ואיטית

    private bool isRolling = false;
    private bool hasReachedTarget = false;

    private void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // ננעל את האבן מראש כדי שהשחקן לא יזיז אותה
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
            if (rb != null) rb.isKinematic = true; // שומרים על kinematic כדי שלא תעוף בגלל פיזיקה
        }
    }

    private void Update()
    {
        if (isRolling && targetPoint != null)
        {
            // תנועה לעבר היעד בצורה מבוקרת (בלי פיזיקה שמעיפה את האבן)
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

            // סיבוב ימינה (ציר Z שלילי/חיובי בהתאם לכיוון המודל)
            // נסי את זה, אם היא מסתובבת הפוך, שנו את המספר ל-100
            transform.Rotate(0, 0, -100 * Time.deltaTime, Space.Self);

            // בדיקה האם הגענו ליעד
            if (Vector3.Distance(transform.position, targetPoint.position) < 0.05f)
            {
                FinishAndBlock();
            }
        }
    }

    private void FinishAndBlock()
    {
        isRolling = false;
        hasReachedTarget = true;

        // הצמדה סופית לנקודה
        transform.position = targetPoint.position;

        // הפיכה למכשול NavMesh קבוע
        if (GetComponent<NavMeshObstacle>() == null)
        {
            NavMeshObstacle obs = gameObject.AddComponent<NavMeshObstacle>();
            obs.carving = true;
            obs.carveOnlyStationary = true;
        }
    }
}