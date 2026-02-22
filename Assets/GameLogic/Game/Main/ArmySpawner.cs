using UnityEngine;
using GameEnums;
using Zenject;

public class ArmySpawner : MonoBehaviour
{
    [Inject] private UnitFactory _unitFactory;
    [Inject] private UnitCreator _unitCreator;
    [Inject] private BattleManager _battleManager;
    
    [SerializeField] private Transform _armyAPosition;
    [SerializeField] private Transform _armyBPosition;

    public void SpawnArmy(ArmyType army)
    {   
        _battleManager.CleanupArmy(army);
        var centerPos = (army == ArmyType.ArmyA) ? _armyAPosition.position : _armyBPosition.position;
        var rotation = Quaternion.Euler(0f, (army == ArmyType.ArmyA) ? 0f : 180f, 0f);
        
        var formation = _battleManager.GetFormation(army);
         
        for (int i = 0; i < 20; i++)
        {
            // offset from formation
            Vector2 offset = Vector2.zero;
            if (formation != null && formation.FORMATION_OFFSETS != null && i < formation.FORMATION_OFFSETS.Length) offset = formation.FORMATION_OFFSETS[i];
            
            // new unit data
            var data = _unitCreator.CreateUnit(army, i, offset);
            
            // position center + offset
            Vector3 spawnPos = centerPos + new Vector3(offset.x, 0f, offset.y);
            // use fabric
            var unitView = _unitFactory.Create(data, spawnPos, rotation, centerPos);
            // set visual to unit
            unitView.ApplyVisualConfig();
            // register unit in battle manager
            _battleManager.RegisterUnit(unitView, data);
        }
    }
}