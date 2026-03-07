using UnityEngine;

public class UnitConfigSO : ScriptableObject
{
    [Header("Modifiers")]
    [Range(-200, 200)]public float HP_MOD = 0f;
    [Range(-200, 200)]public float ATK_MOD = 0f;
    [Range(-200, 200)]public float SPEED_MOD = 0f;
    [Range(-200, 200)]public float ATKSPD_MOD = 0f;
}
