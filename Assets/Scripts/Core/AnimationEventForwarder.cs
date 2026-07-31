using UnityEngine;

public class AnimationEventForwarder : MonoBehaviour
{
    public void OnAttackEnded()
    {
        PlayerCombat combat = GetComponentInParent<PlayerCombat>();
        if (combat != null)
        {
            combat.OnAttackEnded();
        }
        else
        {
            Debug.LogWarning("[AnimationEventForwarder] PlayerCombat component not found in parents!", this);
        }
    }
}

