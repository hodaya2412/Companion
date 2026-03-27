using UnityEngine;

public abstract class IEnemyState
{
    public virtual void Enter() { }
    public virtual void Execute() { }
    public virtual void Exit() { }
}