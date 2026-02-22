using UnityEngine;
using GameEnums;
using UniRx;
using System;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    // statistics (for UI)
    public readonly ReactiveProperty<float> TotalAArmyDamage = new ReactiveProperty<float>();
    public readonly ReactiveProperty<float> TotalBArmyDamage = new ReactiveProperty<float>();
    public readonly ReactiveProperty<int> TotalHits = new ReactiveProperty<int>();
    public readonly ReactiveProperty<ArmyType> Winners = new ReactiveProperty<ArmyType>();

    // global settings
    public readonly ReactiveProperty<GameState> Gamestate = new ReactiveProperty<GameState>(GameState.MainMenu);
    
    // disposables
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    // two armies arrays (logic data)
    public UnitData[] _armyA = new UnitData[20];
    public UnitData[] _armyB = new UnitData[20];

    //For fast targeting: tuple key (ArmyType, Id)
    private readonly Dictionary<(ArmyType, int), UnitView> _unitViews = new();
    
    // For army formation (My additional feature)
    private readonly Dictionary<ArmyType, FormationConfigSO> _armyFormations = new()
    {
        {ArmyType.ArmyA, null},
        {ArmyType.ArmyB, null}
    };
    
    // getter
    public FormationConfigSO GetFormation(ArmyType army) => 
        _armyFormations.TryGetValue(army, out var f) ? f : null;

    public void RegisterUnit(UnitView view, UnitData data)
    {
        _unitViews[(data.Army, data.Id)] = view;
        
        if (data.Army == ArmyType.ArmyA) 
            _armyA[data.Id] = data;
        else  
            _armyB[data.Id] = data;
    }

    public void UnregisterUnit(ArmyType army, int id)
    {
        _unitViews.Remove((army, id));
    }

    // Finding nearest enemy by sqr magnitude, less cpu load
    public UnitView FindNearestEnemy(ArmyType myArmy, Vector3 position)
    {
       var enemyArray = (myArmy == ArmyType.ArmyA) ? _armyB : _armyA;
       var enemyArmyType = (myArmy == ArmyType.ArmyA) ? ArmyType.ArmyB : ArmyType.ArmyA;
    
       UnitView nearest = null;
       float minSqrDist = float.MaxValue;
    
       foreach (var enemyData in enemyArray)
       {
        if (!enemyData.IsAlive) continue;
        var key = (enemyArmyType, enemyData.Id);
        if (!_unitViews.TryGetValue(key, out var enemyView) || enemyView == null) continue;
        
        float sqrDist = Vector3.SqrMagnitude(position - enemyView.transform.position);
        
          if (sqrDist < minSqrDist)
          {
            minSqrDist = sqrDist;
            nearest = enemyView;
          }
       }
    
       return nearest;
    }   

    public void NotifyUnitDied(ArmyType armyType, int id)
    {
        // Update logic data
        if (armyType == ArmyType.ArmyA) _armyA[id].IsAlive = false;
        else _armyB[id].IsAlive = false;

        UnregisterUnit(armyType, id);
        
        // Checking for game end
        var (isEnd, winner) = IsGameEnd();
        if (isEnd)
        {
            Gamestate.Value = GameState.Finished;
            Winners.Value = winner;
        }
    }

    private (bool, ArmyType) IsGameEnd()
    {
        bool isArmyADead = true;
        bool isArmyBDead = true;
        
        for (int i = 0; i < _armyA.Length; i++)
        {
            if (_armyA[i].IsAlive) 
            { 
                isArmyADead = false; 
                break; 
            }
        }
        for (int i = 0; i < _armyB.Length; i++)
        {
            if (_armyB[i].IsAlive) 
            { 
                isArmyBDead = false; 
                break; 
            }
        }

        bool gameOver = isArmyADead || isArmyBDead;
        ArmyType winner = isArmyADead ? ArmyType.ArmyB : ArmyType.ArmyA;
        
        return (gameOver, winner);
    }

    public void SetFormation(ArmyType armyType, FormationConfigSO config)
    {
        _armyFormations[armyType] = config;
        // update for each unit of this army only
        foreach (var kvp in _unitViews)
        {
            if (kvp.Key.Item1 == armyType)
                kvp.Value?.ChangeFormation();
        }
    }

    public void CleanupArmy(ArmyType army)
    {   
        var keysToRemove = new List<(ArmyType, int)>();
    
        foreach (var kvp in _unitViews)
        {
            if (kvp.Key.Item1 == army)
            {
                var view = kvp.Value;
                if (view != null)
                {
                    Destroy(view.gameObject);
                    keysToRemove.Add(kvp.Key);
                }
            }
        }
        
        foreach (var key in keysToRemove)
        {
            _unitViews.Remove(key);
        }
        
        if (army == ArmyType.ArmyA) 
            Array.Clear(_armyA, 0, _armyA.Length);
        else 
            Array.Clear(_armyB, 0, _armyB.Length);
    }

    public void AddStatistic(ArmyType army, float damage)
    {
        TotalHits.Value++;
        if (army == ArmyType.ArmyA) TotalBArmyDamage.Value += damage; 
        else  TotalAArmyDamage.Value += damage;
    }
    
    private void ClearStatistics()
    {
        TotalHits.Value = 0;
        TotalBArmyDamage.Value = 0f;
        TotalAArmyDamage.Value = 0f;
    }
    
    public void CleanupAll()
    {
        CleanupArmy(ArmyType.ArmyA);
        CleanupArmy(ArmyType.ArmyB);
        ClearStatistics();
    }
    
    void OnDestroy()
    {
        CleanupAll();
        _disposables?.Dispose();
    }
}

