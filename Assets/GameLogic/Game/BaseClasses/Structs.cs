using UnityEngine;
using GameEnums;

// single unit data

public struct UnitData
{
    public int Id;
    public ArmyType Army;
    public float CurrentHP;
    public float MaxHP;
    public float ATK;
    public float SPEED;
    public float ATKSPD;
    public Vector2 FormationOffset;
    
    // for battle manager
    public UnitType UnitType;
    public bool IsAlive;
    public bool IsOccupied;

    public UnitData(int id,ArmyType armyType, float hp, float atk, float speed, float atkspd, Vector2 formationOffset, UnitType unitType, bool isAlive, bool isOccupied)
    {
        Id = id;
        Army = armyType;
        MaxHP = hp;
        CurrentHP = hp;
        ATK = atk;
        SPEED = speed;
        ATKSPD = atkspd;
        FormationOffset = formationOffset;

        UnitType = unitType;
        IsAlive = isAlive;
        IsOccupied = isOccupied;
    }
}