using UnityEngine;

[CreateAssetMenu(fileName = "ScaleConfigSO", menuName = "Configs/Units/ScaleConfig", order = 2)]
public class ScaleConfigSO : UnitConfigSO
{
    [Header("For debug")]
    public string DebugScaleId = "SCALE_BIG";
    [Header("Scale default 1 is small")]
    [Range(1, 5)]public float SCALE = 1f;
}