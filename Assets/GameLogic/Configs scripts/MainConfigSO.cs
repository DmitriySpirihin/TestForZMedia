using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig", order = 0)]
public class GameConfigSO : ScriptableObject
{
    public int MAX_UNITS_PER_ARMY = 20;
    [Header("Default units stats")]
    public float HP = 100f;
    public float ATK = 10f;
    public float SPEED = 10f;
    public float ATKSPD = 1f;
}