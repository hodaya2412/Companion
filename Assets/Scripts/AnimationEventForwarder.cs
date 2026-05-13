using UnityEngine;

public class AnimationEventForwarder : MonoBehaviour
{
    public void OnAttackEnded()
    {
        GetComponentInParent<PlayerCombat>().OnAttackEnded();
    }
}

