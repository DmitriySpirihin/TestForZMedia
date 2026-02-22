using GameEnums;
using UnityEngine;
using Zenject;

public class UnitCreator
{
    private readonly GameConfigSO _gameConfig;
    private readonly ShapeConfigSO[] _shapeConfigs;
    private readonly ColorConfigSO[] _colorConfigs;
    private readonly ScaleConfigSO[] _scaleConfigs;

    [Inject]
    public UnitCreator(GameConfigSO gameConfig, ShapeConfigSO[] shapes, ColorConfigSO[] colors, ScaleConfigSO[] scales)
    {
        _gameConfig = gameConfig;
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
        
        float hp = Mathf.Max(1f, _gameConfig.HP + shape.HP_MOD + color.HP_MOD + size.HP_MOD);
        float atk = _gameConfig.ATK + shape.ATK_MOD + color.ATK_MOD + size.ATK_MOD;
        float speed = _gameConfig.SPEED + shape.SPEED_MOD + color.SPEED_MOD + size.SPEED_MOD;
        float atkspd = _gameConfig.ATKSPD + shape.ATKSPD_MOD + color.ATKSPD_MOD + size.ATKSPD_MOD;

        return new UnitData(id, army, hp, atk, speed, atkspd, formationOffset, shape, color, size);
    }
}
