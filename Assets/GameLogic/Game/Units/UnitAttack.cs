using UnityEngine;
using GameEnums;
using UniRx;
using Zenject;
using System;

public class UnitAttack : MonoBehaviour, IAttack
{
    // reactive props
    private readonly ReactiveProperty<bool> _isAttacking = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> IsAttacking => _isAttacking;
    
    // data
    private int _id;
    private ArmyType _army;
    private float _atk;
    private float _atkspd; // just delay
    
    // references
    private IHealth _target;
    [Inject] private BattleManager _battleManager;
    private CompositeDisposable _disposables = new CompositeDisposable();
    
    // attack state
    private bool _hasTarget;
    private float _attackRange = 2f;
    private IDisposable _attackSubscription;

    public void Init(UnitData data)
    {
        _id = data.Id;
        _army = data.ArmyType;
        _atk = data.ATK;
        _atkspd = data.ATKSPD;
    }

    public void SetTarget(IHealth target)
    {
        _target = target;
        _hasTarget = true;
        StartAttackLoop();
    }

    public void StopAttack()
    {
        _hasTarget = false;
        _isAttacking.Value = false;
        _attackSubscription?.Dispose();
    }

    private void StartAttackLoop()
    {
        float delay = Mathf.Max(0.1f, _atkspd);
        
        _attackSubscription = Observable.Timer(System.TimeSpan.FromSeconds(delay))
            .Repeat()
            .TakeUntilDestroy(this)
            .TakeUntil(_target?.State?.Where(s => s == HealthState.Dead))
            .Subscribe(_ => TryAttack())
            .AddTo(_disposables);
    }

    private void TryAttack()
    {
        if (!_hasTarget || _target == null)
        {
            StopAttack();
            return;
        }

        if (_target.State.Value != HealthState.Alive)
        {
            StopAttack();
            return;
        }

        float distance = Vector3.SqrMagnitude(transform.position - (_target as MonoBehaviour)?.transform?.position ?? Vector3.zero);
        if (distance > _attackRange * _attackRange)
        {
            _isAttacking.Value = false;
            return;
        }

        _isAttacking.Value = true;
        _target.TakeDamage(_atk);
    }

    // to stop if needed
    public void ForceStop()
    {
        StopAttack();
        _disposables.Clear();
    }

    private void OnDestroy()
    {
        _disposables?.Dispose();
        _attackSubscription?.Dispose();
    }
}