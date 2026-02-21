using UnityEngine;
using GameEnums;
using UniRx;
using Zenject;

public class UnitHealth : IHealth
{
    //reactive pr - s
    private int _id;
    private ArmyType _army;
    private readonly ReactiveProperty<float> _currentHealth = new ReactiveProperty<float>();
    private readonly ReactiveProperty<float> _maxHealth = new ReactiveProperty<float>();
    private readonly ReactiveProperty<HealthState> _state = new ReactiveProperty<HealthState>(HealthState.Alive);
    
    public IReadOnlyReactiveProperty<float> CurrentHealth => _currentHealth;
    public IReadOnlyReactiveProperty<float> MaxHealth => _maxHealth;
    public IReadOnlyReactiveProperty<HealthState> State => _state;
    
    // references
    [Inject]private BattleManager _battleManager;
    
    public UnitHealth(UnitData data, GameObject context)
    {
        _id = data.Id;
        _army = data.Army;
        _maxHealth.Value = Mathf.Max(1f, data.MaxHP);
        _currentHealth.Value = _maxHealth.Value;
        _currentHealth.DistinctUntilChanged().Subscribe(OnDeath).AddTo(context);
    }
    
    public virtual void TakeDamage(float amount)
    {
        if (_state.Value != HealthState.Alive) return;

        float validDamage = Mathf.Max(_currentHealth.Value - amount, 0f);

        _currentHealth.Value = validDamage;
    }
    
    private void OnDeath(float current)
    {
        if(current > 0)return;
        _state.Value = HealthState.Dead;
        _battleManager?.NotifyUnitDied(_army, _id);
    }
}


