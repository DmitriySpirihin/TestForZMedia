using UnityEngine;
using Zenject;

public class UnitView : MonoBehaviour
{
    [Inject] private BattleManager _battleManager; 
    
    private UnitData _data;
    private IHealth _health;
    private UnitAttack _attack;
    private UnitMove _movement;
    
    // army center
    private Vector3 _armyCenter;

    public void Initialize(UnitData data, Vector3 armyCenter)
    {
        _data = data;
        _armyCenter = armyCenter;
        _health = GetComponent<IHealth>();
        _health?.Init(_data);
        _attack = GetComponent<UnitAttack>();
        _attack?.Init(data);
        _movement = GetComponent<UnitMove>();
        _movement?.Init(data);
    }

    public void ApplyVisualConfig()
    {
        var renderer = GetComponent<Renderer>();
        var mpb = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(mpb);
        mpb.SetColor("_BaseColor", _data.ColorConfig.COLOR);
        renderer.SetPropertyBlock(mpb);
        
        transform.localScale = Vector3.one * _data.ScaleConfig.SCALE;

        float rotationY = (_data.Army == GameEnums.ArmyType.ArmyA) ? 0f : 180f;
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        
    }

    public void ChangeFormation()
    {
        // get current formation
        var formation = _battleManager.GetFormation(_data.Army);
        Vector2 offset = Vector2.zero;
        
        if (formation != null && formation.FORMATION_OFFSETS != null && _data.Id < formation.FORMATION_OFFSETS.Length) offset = formation.FORMATION_OFFSETS[_data.Id];
        
        // apply offset to position
        transform.position = _armyCenter + new Vector3(offset.x, 0f, offset.y);
    }
}
