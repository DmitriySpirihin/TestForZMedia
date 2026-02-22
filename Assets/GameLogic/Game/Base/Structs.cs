using UnityEngine;
using GameEnums;

// single unit data

public struct UnitData
{
    // id
    public int Id;
    public ArmyType ArmyType;
    
    // stats
    public float CurrentHP;
    public float MaxHP;
    public float ATK;
    public float SPEED;
    public float ATKSPD;
    
    public Vector2 FormationOffset;
    
    // configs
    public ShapeConfigSO ShapeConfig;
    public ColorConfigSO ColorConfig;
    public ScaleConfigSO ScaleConfig;

    //for manager
    public bool IsAlive;
    public bool IsOccupied;
    
    // constructor
    public UnitData(int id, ArmyType armyType,  float hp, float atk, float speed, float atkspd, Vector2 formationOffset, ShapeConfigSO shapeConfig, ColorConfigSO colorConfig, ScaleConfigSO scaleConfig)
    {
        Id = id;
        ArmyType = armyType;
        MaxHP = hp;
        CurrentHP = hp;
        ATK = atk;
        SPEED = speed;
        ATKSPD = atkspd;
        FormationOffset = formationOffset;
        ShapeConfig = shapeConfig;
        ColorConfig = colorConfig;
        ScaleConfig = scaleConfig;

        IsAlive = true;
        IsOccupied = false;
    }
}