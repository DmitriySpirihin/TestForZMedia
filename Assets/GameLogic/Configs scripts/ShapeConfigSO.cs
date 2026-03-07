using UnityEngine;

[CreateAssetMenu(fileName = "ShapeConfigSO", menuName = "Configs/Units/ShapeConfig", order = 0)]
public class ShapeConfigSO : UnitConfigSO
{
    [Header("For debug")]
    public string DebugId = "SHAPE_CUBE";
    [Header("New prefab that only contains mesh renderer")]
    public GameObject SHAPE_PREFAB;
}
