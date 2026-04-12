using System.Collections;
using UnityEngine;

public class BreakableForceFieldBarrier : MonoBehaviour, IPlayerAttackReceiver
{
    [Header("Requirements")]
    [SerializeField] private string[] allowedWeaponItemIds;

    [Header("Barrier Health")]
    [SerializeField] private int hitsToBreak = 3;

    [Header("Visuals")]
    [SerializeField] private ForceFieldPulse forceFieldPulse;
    [SerializeField] private GameObject breakEffectPrefab;
    [SerializeField] private Transform breakEffectSpawnPoint;
    [SerializeField] private GameObject visualsRoot;

    [Header("Disable On Break")]
    [SerializeField] private GameObject collidersToDisable;

    [Header("Persistence / Result")]
    [SerializeField] private string brokenFlagName;

    private int currentHits;
    private bool isBroken;

    private void Start()
    {
        if (forceFieldPulse == null)
            forceFieldPulse = GetComponent<ForceFieldPulse>();

        if (!string.IsNullOrEmpty(brokenFlagName) &&
            GameStateManager.Instance != null &&
            GameStateManager.Instance.GetFlag(brokenFlagName))
        {
            BreakImmediatelyWithoutEffect();
        }
    }

    public void ReceivePlayerAttack(PlayerAttackData attackData)
    {
        if (isBroken) return;

        bool canDamageBarrier = CanAttackBreakBarrier(attackData);

        if (!canDamageBarrier)
        {
            forceFieldPulse?.TriggerBlockedHitFeedback();
            return;
        }

        currentHits++;
        forceFieldPulse?.TriggerValidHitFeedback();

        float progress = Mathf.Clamp01((float)currentHits / hitsToBreak);
        forceFieldPulse?.SetBreakProgress(progress);

        if (currentHits >= hitsToBreak)
        {
            BreakBarrier();
        }
    }

    private bool CanAttackBreakBarrier(PlayerAttackData attackData)
    {
        if (!attackData.hasWeaponEquipped) return false;
        if (attackData.equippedWeapon == null) return false;

        string equippedItemId = attackData.equippedWeapon.itemId;
        if (string.IsNullOrEmpty(equippedItemId)) return false;
        if (allowedWeaponItemIds == null || allowedWeaponItemIds.Length == 0) return false;

        for (int i = 0; i < allowedWeaponItemIds.Length; i++)
        {
            if (allowedWeaponItemIds[i] == equippedItemId)
                return true;
        }

        return false;
    }

    private void BreakBarrier()
    {
        if (isBroken) return;
        isBroken = true;

        if (!string.IsNullOrEmpty(brokenFlagName) && GameStateManager.Instance != null)
        {
            GameStateManager.Instance.SetFlag(brokenFlagName, true);
        }

        StartCoroutine(BreakSequence());
    }

    private IEnumerator BreakSequence()
    {
        if (breakEffectPrefab != null)
        {
            Vector3 spawnPos = breakEffectSpawnPoint != null ? breakEffectSpawnPoint.position : transform.position;
            Quaternion spawnRot = breakEffectSpawnPoint != null ? breakEffectSpawnPoint.rotation : transform.rotation;
            GameObject effect = Instantiate(breakEffectPrefab, spawnPos, spawnRot);
            Destroy(effect, 3f);
        }

        if (collidersToDisable != null)
            collidersToDisable.SetActive(false);

        forceFieldPulse?.SetBreakProgress(1f);
        forceFieldPulse?.TriggerValidHitFeedback();

        if (visualsRoot != null)
        {
            BarrierVisualShard[] shards = visualsRoot.GetComponentsInChildren<BarrierVisualShard>(true);
            foreach (var shard in shards)
            {
                shard.PlayBreak();
            }
        }

        yield return new WaitForSeconds(1.5f);

        gameObject.SetActive(false);
    }

    private void BreakImmediatelyWithoutEffect()
    {
        isBroken = true;

        if (collidersToDisable != null)
            collidersToDisable.SetActive(false);

        if (visualsRoot != null)
            visualsRoot.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}