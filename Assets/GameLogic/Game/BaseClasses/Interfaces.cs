using UnityEngine;
using GameEnums;
using UniRx;

public interface IHealth
{
    IReadOnlyReactiveProperty<float> CurrentHealth { get; }
    IReadOnlyReactiveProperty<float> MaxHealth { get; }
    IReadOnlyReactiveProperty<HealthState> State { get; }

    public void TakeDamage(float amount);
}

