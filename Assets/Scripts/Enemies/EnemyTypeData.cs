using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Type")]
public class EnemyTypeData : ScriptableObject
{
    [Header("Combat")]
    public float damage = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;

    [Header("Movement")]
    public float moveSpeed = 3.5f;

    [Header("Behavior")]
    public float aggression = 1f;
    public float flankDesire = 0.5f;

    [Header("Role Preference")]
    public bool preferFlank = false;
}