using UnityEngine;
using GameEnums;
using UniRx;

public interface IHealth
{
    IReadOnlyReactiveProperty<float> CurrentHealth { get; }
    IReadOnlyReactiveProperty<float> MaxHealth { get; }
    IReadOnlyReactiveProperty<HealthState> State { get; }

    public void Init(UnitData unitData);
    public void TakeDamage(float amount);
}

public interface IMove
{
    IReadOnlyReactiveProperty<bool> IsMoving { get; }
    void Init(UnitData data);
    void Stop();
    void ForceStop();
}

public interface IAttack
{
    IReadOnlyReactiveProperty<bool> IsAttacking { get; }
    void Init(UnitData data);
    void SetTarget(IHealth target);
    void StopAttack();
    void ForceStop();
}