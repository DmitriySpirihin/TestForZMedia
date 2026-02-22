using UnityEngine;
using GameEnums;
using UniRx;
using Zenject;

public class UnitHealth : MonoBehaviour, IHealth
{
    // for log
    string army;
    //reactive pr - s
    private int _id;
    private ArmyType _army;
    private readonly ReactiveProperty<float> _currentHealth = new ReactiveProperty<float>();
    private readonly ReactiveProperty<float> _maxHealth = new ReactiveProperty<float>();
    private readonly ReactiveProperty<HealthState> _state = new ReactiveProperty<HealthState>(HealthState.Alive);
    
    public IReadOnlyReactiveProperty<float> CurrentHealth => _currentHealth;
    public IReadOnlyReactiveProperty<float> MaxHealth => _maxHealth;
    public IReadOnlyReactiveProperty<HealthState> State => _state;

    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    
    // references
    [Inject]private BattleManager _battleManager;
    
    public void Init(UnitData data)
    {
        _id = data.Id;
        _army = data.ArmyType;
        _maxHealth.Value = Mathf.Max(1f, data.MaxHP);
        _currentHealth.Value = _maxHealth.Value;
        army = _army == ArmyType.ArmyA ? "Army A" : "Army B";
        _currentHealth.DistinctUntilChanged().Subscribe(OnDeath).AddTo(_disposables);
    }
    
    public virtual void TakeDamage(float amount)
    {
        if (_state.Value != HealthState.Alive) return;

        float validDamage = Mathf.Max(_currentHealth.Value - amount, 0f);

        _currentHealth.Value = validDamage;

        Debug.Log($"Unit: {_id} from army: {army} takes {validDamage} point of damage");
    }
    
    private void OnDeath(float current)
    {
        if(current > 0)return;
        _state.Value = HealthState.Dead;
        _battleManager?.NotifyUnitDied(_army, _id);
        _disposables?.Dispose(); 
        Debug.Log($"Unit: {_id} from army: {army} has died");
        var collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false;
        var renderer = GetComponent<Renderer>();
        if (renderer != null) renderer.enabled = false;

        Destroy(gameObject, 0.1f);
    }
    
}


