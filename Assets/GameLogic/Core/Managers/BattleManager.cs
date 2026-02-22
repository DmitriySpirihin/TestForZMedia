using UnityEngine;
using GameEnums;
using UniRx;
using System;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    // statistics (for UI)
    public readonly ReactiveProperty<float> TotalRedArmyDamage = new ReactiveProperty<float>();
    public readonly ReactiveProperty<float> TotalBlueArmyDamage = new ReactiveProperty<float>();
    public readonly ReactiveProperty<int> TotalHits = new ReactiveProperty<int>();
    public readonly ReactiveProperty<ArmyType> Winners = new ReactiveProperty<ArmyType>();

    // global settings
    public readonly ReactiveProperty<GameState> Gamestate = new ReactiveProperty<GameState>(GameState.Running);
    
    // disposables
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    // two armies arrays (logic data)
    public UnitData[] _armyA = new UnitData[20];
    public UnitData[] _armyB = new UnitData[20];

    // For fast targeting
    private readonly Dictionary<int, UnitView> _unitViews = new();
    // For army formation (My additional feature)
    public readonly Dictionary<ArmyType, FormationConfigSO> _armyFormations = new()
    {
        {ArmyType.ArmyA, null},
        {ArmyType.ArmyB, null}
    };

    public void RegisterUnit(UnitView view, UnitData data)
    {
        _unitViews[data.Id] = view;
        if(data.ArmyType == ArmyType.ArmyA) _armyA[data.Id] = data;
        else  _armyB[data.Id] = data;
    }

    public void UnregisterUnit(int id)
    {
        _unitViews.Remove(id);
    }

    //Finding nearest enemy by sqr magnitude, less cpu load
    public UnitView FindNearestEnemy(ArmyType myArmy, Vector3 position)
    {
        var enemyArray = (myArmy == ArmyType.ArmyA) ? _armyB : _armyA;
        
        UnitView nearest = null;
        float minSqrDist = float.MaxValue;
        
        foreach (var enemyData in enemyArray)
        {
            if (!enemyData.IsAlive) continue;
            
            if (!_unitViews.TryGetValue(enemyData.Id, out var enemyView) || enemyView == null)
                continue;
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

        UnregisterUnit(id);
        
        // Checking for game enf
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
        // Checking ArmyA
        for (int i = 0; i < _armyA.Length; i++)
        {
            if (_armyA[i].IsAlive) 
            { 
                isArmyADead = false; 
                break; 
            }
        }
        // Checking ArmyB
        for (int i = 0; i < _armyB.Length; i++)
        {
            if (_armyB[i].IsAlive) 
            { 
                isArmyBDead = false; 
                break; 
            }
        }

        //Game over if one army is fully dead
        bool gameOver = isArmyADead || isArmyBDead;
        // Winner is the opposite army
        ArmyType winner = isArmyADead ? ArmyType.ArmyB : ArmyType.ArmyA;
        
        return (gameOver, winner);
    }

    public void SetFormation(ArmyType armyType, FormationConfigSO config)
    {
        _armyFormations[armyType] = config;
        // update for each unit
        foreach (var unit in _unitViews.Values)
           unit.ChangeFormation();
    }

    public void CleanupArmy(ArmyType army)
    {   
        foreach (var unit in _unitViews.Values)
        {
            if (unit != null)
            {
                UnregisterUnit(unit.gameObject.GetInstanceID());
                Destroy(unit.gameObject);
            }
        }
        _unitViews.Clear();
    }
    public void CleanupAll()
    {
        CleanupArmy(ArmyType.ArmyA);
        CleanupArmy(ArmyType.ArmyB);
    }
    void OnDestroy()
    {
        CleanupAll();
        _disposables?.Dispose();
    }
}

