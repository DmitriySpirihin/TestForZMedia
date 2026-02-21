using UnityEngine;


public static class BaseUnitStats
{
    public const float HP = 100f;
    public const float ATK = 10f;
    public const float SPEED = 10f;
    public const float ATKSPD = 1f;
}

[CreateAssetMenu(fileName = "ShapeConfigSO", menuName = "Scriptable Units/ShapeConfig")]
public class ShapeConfigSO : ScriptableObject
{
    public string DebugId = "SHAPE_CUBE";
    public GameObject SHAPE_PREFAB;
    [Range(-200, 200)]public float HP_MOD = 0f;
    [Range(-200, 200)]public float ATK_MOD = 0f;
    [Range(-200, 200)]public float SPEED_MOD = 0f;
    [Range(-200, 200)]public float ATKSPD_MOD = 0f;
}

[CreateAssetMenu(fileName = "ColorConfigSO", menuName = "Scriptable Units/ColorConfig")]
public class ColorConfigSO : ScriptableObject
{
    public string DebugColorId = "COLOR_RED";
    public Color COLOR;
    [Range(-200, 200)]public float HP_MOD = 0f;
    [Range(-200, 200)]public float ATK_MOD = 0f;
    [Range(-200, 200)]public float SPEED_MOD = 0f;
    [Range(-200, 200)]public float ATKSPD_MOD = 0f;
}

[CreateAssetMenu(fileName = "ScaleConfigSO", menuName = "Scriptable Units/ScaleConfig")]
public class ScaleConfigSO : ScriptableObject
{
    public string DebugScaleId = "SCALE_BIG";
    [Range(1, 5)]public float SCALE = 1f;
    [Range(-200, 200)]public float HP_MOD = 0f;
    [Range(-200, 200)]public float ATK_MOD = 0f;
    [Range(-200, 200)]public float SPEED_MOD = 0f;
    [Range(-200, 200)]public float ATKSPD_MOD = 0f;
}