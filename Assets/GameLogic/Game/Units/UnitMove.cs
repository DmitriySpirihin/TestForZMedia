using UnityEngine;
using GameEnums;
using UniRx;
using Zenject;

public class UnitMove : MonoBehaviour, IMove
{
    // reactive props
    private readonly ReactiveProperty<bool> _isMoving = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> IsMoving => _isMoving;
    
    // data
    private int _id;
    private ArmyType _army;
    private float _speed;
    
    // state
    private UnitView _currentTarget;
    private bool _hasTarget;
    private float _stopDistance = 2f; // melee range
    
    // references
    [Inject] private BattleManager _battleManager;
    private UnitAttack _attack;
    private CompositeDisposable _disposables = new CompositeDisposable();

    public void Init(UnitData data)
    {
        _id = data.Id;
        _army = data.ArmyType;
        _speed = data.SPEED;
        _attack = GetComponent<UnitAttack>();
        
        // stop is game state is not runnig
        _battleManager.Gamestate.Where(state => state != GameState.Running).Take(1).Subscribe(_ => Stop()).AddTo(_disposables);
    }

    // Set target via battle manager
    public void FindAndSetTarget(Vector3 searchPosition)
    {
        var target = _battleManager.FindNearestEnemy(_army, searchPosition);
        SetTargetView(target);
    }

    private void SetTargetView(UnitView targetView)
    {
        if (targetView == null)
        {
            Stop();
            return;
        }
        
        _currentTarget = targetView;
        _hasTarget = true;

        // Give IHealth to attack comp
        if (targetView.TryGetComponent<IHealth>(out var health))
            _attack?.SetTarget(health);
        else
            Debug.LogWarning($"[UnitMove] Target has no IHealth component (ID: {_id})");
        
        StartMoveLoop();
    }

    public void Stop()
    {
        _hasTarget = false;
        _isMoving.Value = false;
        _currentTarget = null;
        _attack?.StopAttack(); // stop attack too
    }

    private void StartMoveLoop()
    {
        Observable.EveryUpdate()
            .TakeUntilDestroy(this)
            .Subscribe(_ => UpdateMove())
            .AddTo(_disposables);
    }

    private void UpdateMove()
    {
        if (_battleManager.Gamestate.Value != GameState.Running)
        {
            _isMoving.Value = false;
            return;
        }

        if (!_hasTarget || _currentTarget == null)
        {
            _isMoving.Value = false;
            return;
        }
        
        if (_currentTarget.TryGetComponent<IHealth>(out var targetHealth) &&  targetHealth.State.Value == HealthState.Dead)
        {
            // need a new one if current dead
            FindAndSetTarget(transform.position);
            return;
        }

        // check magnitude
        float sqrDistance = Vector3.SqrMagnitude(transform.position - _currentTarget.transform.position);
        
        // Stop if in attack range
        if (sqrDistance <= _stopDistance * _stopDistance)
        {
            _isMoving.Value = false;
            return;
        }

        _isMoving.Value = true;
        transform.position = Vector3.MoveTowards( transform.position,  _currentTarget.transform.position, _speed * Time.deltaTime);
    }

    public void ForceStop()
    {
        Stop();
        _disposables.Clear();
    }

    private void OnDestroy()
    {
        _disposables?.Dispose();
    }
}
