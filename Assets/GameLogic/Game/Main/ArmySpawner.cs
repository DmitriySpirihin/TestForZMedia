using UnityEngine;
using GameEnums;
using Zenject;
using System.Collections.Generic;

public class ArmySpawner : MonoBehaviour
{
    [Inject] private UnitFactory _unitFactory;
    [Inject] private UnitCreator _unitCreator;
    
    [SerializeField] private Transform _armyAPosition;
    [SerializeField] private Transform _armyBPosition;

    public void SpawnArmy(ArmyType army)
    {   
        var centerPos = (army == ArmyType.ArmyA) ? _armyAPosition.position : _armyBPosition.position;
        var rotation = Quaternion.Euler(0f, (army == ArmyType.ArmyA) ? 0f : 180f, 0f);
        
        // generating 20 units
        for (int i = 0; i < 20; i++)
        {
            var data = _unitCreator.CreateUnit(army, i);
            _unitFactory.Create(data, Vector3.zero, rotation);
        }
    }
}