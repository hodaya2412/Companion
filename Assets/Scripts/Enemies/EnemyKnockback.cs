using UnityEngine;

public class EnemyKnockback : MonoBehaviour
{
    [SerializeField] private BanditCaveAgent movement;
    [SerializeField] private float knockbackDistance = 0.3f;

    private void Awake()
    {
        if (movement == null)
            movement = GetComponent<BanditCaveAgent>();
    }

    public void Knockback()
    {
        if (movement == null) return;

        movement.KnockBackFrom(
            movement.GetKnockbackSourcePosition(),
            knockbackDistance
        );
    }
}