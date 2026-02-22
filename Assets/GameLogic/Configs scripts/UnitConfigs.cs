using UnityEngine;

[CreateAssetMenu(fileName = "ShapeConfigSO", menuName = "Configs/Units/ShapeConfig", order = 0)]
public class ShapeConfigSO : ScriptableObject
{
    [Header("For debug")]
    public string DebugId = "SHAPE_CUBE";
    [Header("New prefab that only contains mesh renderer")]
    public GameObject SHAPE_PREFAB;
    [Header("Modifiers")]
    [Range(-200, 200)]public float HP_MOD = 0f;
    [Range(-200, 200)]public float ATK_MOD = 0f;
    [Range(-200, 200)]public float SPEED_MOD = 0f;
    [Range(-200, 200)]public float ATKSPD_MOD = 0f;
}

[CreateAssetMenu(fileName = "ColorConfigSO", menuName = "Configs/Units/ColorConfig", order = 1)]
public class ColorConfigSO : ScriptableObject
{
    [Header("For debug")]
    public string DebugColorId = "COLOR_RED";
    [Header("Color for material")]
    public Color COLOR;
    [Header("Modifiers")]
    [Range(-200, 200)]public float HP_MOD = 0f;
    [Range(-200, 200)]public float ATK_MOD = 0f;
    [Range(-200, 200)]public float SPEED_MOD = 0f;
    [Range(-200, 200)]public float ATKSPD_MOD = 0f;
}

[CreateAssetMenu(fileName = "ScaleConfigSO", menuName = "Configs/Units/ScaleConfig", order = 2)]
public class ScaleConfigSO : ScriptableObject
{
    [Header("For debug")]
    public string DebugScaleId = "SCALE_BIG";
    [Header("Scale default 1 is small")]
    [Range(1, 5)]public float SCALE = 1f;
    [Header("Modifiers")]
    [Range(-200, 200)]public float HP_MOD = 0f;
    [Range(-200, 200)]public float ATK_MOD = 0f;
    [Range(-200, 200)]public float SPEED_MOD = 0f;
    [Range(-200, 200)]public float ATKSPD_MOD = 0f;
}