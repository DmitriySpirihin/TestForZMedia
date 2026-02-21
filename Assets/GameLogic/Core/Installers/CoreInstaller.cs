using UnityEngine;
using Zenject;


  public class CoreInstaller : MonoInstaller
  {
    [Header("Configuration")]
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private ShapeConfigSO[] _shapeConfigs;
    [SerializeField] private ColorConfigSO[] _colorConfigs;
    [SerializeField] private ScaleConfigSO[] _scaleConfigs;

 
    public override void InstallBindings()
    {
        Container.Bind<BattleManager>().FromInstance(_battleManager).AsSingle().NonLazy();
        Container.Bind<ShapeConfigSO[]>().FromInstance(_shapeConfigs);
        Container.Bind<ColorConfigSO[]>().FromInstance(_colorConfigs).AsSingle();
        Container.Bind<ScaleConfigSO[]>().FromInstance(_scaleConfigs).AsSingle();
    }
  }
