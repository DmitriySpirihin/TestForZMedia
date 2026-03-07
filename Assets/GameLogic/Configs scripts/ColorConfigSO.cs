using UnityEngine;

[CreateAssetMenu(fileName = "ColorConfigSO", menuName = "Configs/Units/ColorConfig", order = 1)]
public class ColorConfigSO : UnitConfigSO
{
    [Header("For debug")]
    public string DebugColorId = "COLOR_RED";
    [Header("Color for material")]
    public Color COLOR;
}
