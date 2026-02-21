using GameEnums;
using UnityEngine;
using Zenject;

public class UnitCreator
{
    private readonly ShapeConfigSO[] _shapeConfigs;
    private readonly ColorConfigSO[] _colorConfigs;
    private readonly ScaleConfigSO[] _scaleConfigs;

    [Inject]
    public UnitCreator(ShapeConfigSO[] shapes, ColorConfigSO[] colors, ScaleConfigSO[] scales)
    {
        _shapeConfigs = shapes;
        _colorConfigs = colors;
        _scaleConfigs = scales;
    }

    public UnitData CreateUnit(ArmyType army, int id, Vector2 formationOffset = default)
    {
        var shape = _shapeConfigs[Random.Range(0, _shapeConfigs.Length)];
        var color = _colorConfigs[Random.Range(0, _colorConfigs.Length)];
        var size = _scaleConfigs[Random.Range(0, _scaleConfigs.Length)];

        // Base: 100 HP, 10 ATK, 10 SPEED, 1 ATKSPD
        // ATKSPD: if higher than slower > (delay)
        
        float hp = Mathf.Max(1f, 100f + shape.HP_MOD + color.HP_MOD + size.HP_MOD);
        float atk = 10f + shape.ATK_MOD + color.ATK_MOD + size.ATK_MOD;
        float speed = 10f + shape.SPEED_MOD + color.SPEED_MOD + size.SPEED_MOD;
        float atkspd = 1f + shape.ATKSPD_MOD + color.ATKSPD_MOD + size.ATKSPD_MOD;

        return new UnitData(id, army, hp, atk, speed, atkspd, formationOffset, GetUnitType(hp), true, false);
    }

    // helpers
    private UnitType GetUnitType(float hp)
    {
        return hp < 100f ? UnitType.Week : hp < 200f ? UnitType.Normal : UnitType.Strong;
    }
}
