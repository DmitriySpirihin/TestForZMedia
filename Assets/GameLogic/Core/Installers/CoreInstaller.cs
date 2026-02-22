using UnityEngine;
using Zenject;


  public class CoreInstaller : MonoInstaller
  {
    [Header("Global dependencies")]
    [SerializeField] private Transform _battleGroundTr;
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private GameConfigSO _gameConfig;
    [SerializeField] private ShapeConfigSO[] _shapeConfigs;
    [SerializeField] private ColorConfigSO[] _colorConfigs;
    [SerializeField] private ScaleConfigSO[] _scaleConfigs;
    [SerializeField] private FormationConfigSO[] _allFormations;

 
    public override void InstallBindings()
    {
        Container.Bind<BattleManager>().AsSingle().NonLazy();
        Container.Bind<Transform>().WithId("battleground_transform").FromInstance(_battleGroundTr);
        Container.Bind<BattleManager>().FromInstance(_battleManager).AsSingle().NonLazy();
        Container.Bind<GameConfigSO>().FromInstance(_gameConfig);
        Container.Bind<ShapeConfigSO[]>().FromInstance(_shapeConfigs);
        Container.Bind<ColorConfigSO[]>().FromInstance(_colorConfigs).AsSingle();
        Container.Bind<ScaleConfigSO[]>().FromInstance(_scaleConfigs).AsSingle();
        Container.Bind<FormationConfigSO[]>().FromInstance(_allFormations).AsSingle();
        Container.Bind<ArmySpawner>().AsSingle().NonLazy();
    }
  }
