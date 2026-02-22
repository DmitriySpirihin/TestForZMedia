using UnityEngine;
using Zenject;
using UniRx;

public class UnitView : MonoBehaviour
{
    [Inject] private BattleManager _battleManager; 
    
    private UnitData _data;
    private IHealth _health;
    private UnitAttack _attack;
    private UnitMove _movement;

    public void Initialize(UnitData data)
    {
        _data = data;
        // register this in dictionary for targeting
        _battleManager.RegisterUnit(this, data);
        _health = GetComponent<IHealth>();
        _health?.Init(_data);
        _attack = GetComponent<UnitAttack>();
        _attack?.Init(data);
        _movement = GetComponent<UnitMove>();
        _movement?.Init(data);
        SetupSubscriptions();
    }

    // Visual settings
    public void ApplyVisualConfig()
    {
        //Material
        var renderer = GetComponent<Renderer>();
        var mpb = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", _data.ColorConfig.COLOR);
        renderer.SetPropertyBlock(mpb);
        
        // Scale
        transform.localScale = Vector3.one * _data.ScaleConfig.SCALE;

        float rotationY = (_data.ArmyType == GameEnums.ArmyType.ArmyA) ? 0f : 180f;
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        ChangeFormation();
    }

    public void ChangeFormation()
    {
        //  position with formation offset by index as Id
        float offsetX = _battleManager._armyFormations[_data.ArmyType].FORMATION_OFFSETS?[_data.Id].x?? 0f;
        float offsetZ = _battleManager._armyFormations[_data.ArmyType].FORMATION_OFFSETS?[_data.Id].y?? 0f;
        transform.position = new Vector3(offsetX, transform.position.y, offsetZ);
    }

    //Just for future functionality
    private void SetupSubscriptions()
    {
        _health?.State.Where(state => state == GameEnums.HealthState.Dead).Take(1).Subscribe(_ => OnUnitDied()).AddTo(this);
    }

    private void OnUnitDied()
    {
        //Here we can add some vfx for the game
    }
}
