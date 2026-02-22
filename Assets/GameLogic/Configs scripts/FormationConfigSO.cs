using UnityEngine;

[CreateAssetMenu(fileName = "FormationConfig", menuName = "Configs/FormationConfig", order = 1)]
public class FormationConfigSO : ScriptableObject
{
    [Header("Name")]
    public string NAME = "Row";
    [Header("Formation as array of vectors (x represents the same, y nrepresents z)")]
    public Vector2[] FORMATION_OFFSETS = new Vector2[20]
    {
        new Vector2(-19f, 0f),
        new Vector2(-17f, 0f),
        new Vector2(-15f, 0f),
        new Vector2(-13f, 0f),
        new Vector2(-11f, 0f),
        new Vector2(-9f, 0f),
        new Vector2(-7f, 0f),
        new Vector2(-5f, 0f),
        new Vector2(-3f, 0f),
        new Vector2(-1f, 0f),
        new Vector2(1f, 0f),
        new Vector2(3f, 0f),
        new Vector2(5f, 0f),
        new Vector2(7f, 0f),
        new Vector2(9f, 0f),
        new Vector2(11f, 0f),
        new Vector2(13f, 0f),
        new Vector2(15f, 0f),
        new Vector2(17f, 0f),
        new Vector2(19f, 0f)
    };
}