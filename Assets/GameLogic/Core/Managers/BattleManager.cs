using GameEnums;
using UniRx;
using System;
using Zenject;

public class BattleManager : IDisposable
{
    // statistics
    public readonly ReactiveProperty<float> TotalRedArmyDamage = new ReactiveProperty<float>();
    public readonly ReactiveProperty<float> TotalBlueArmyDamage = new ReactiveProperty<float>();
    public readonly ReactiveProperty<int> TotalHits = new ReactiveProperty<int>();

    // global settings
    public readonly ReactiveProperty<GameState> Gamestate = new ReactiveProperty<GameState>(GameState.Running);
    
    
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    public UnitData[] _armyA = new UnitData[20];
    public UnitData[] _armyB = new UnitData[20];

    public BattleManager()
    {
        
    }

    public void NotifyUnitDied(ArmyType armyType, int id)
    {
        
    }
    
    public void Dispose() => _disposables.Dispose();
}

