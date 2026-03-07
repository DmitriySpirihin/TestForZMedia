using UnityEngine;
using Zenject;


  public class CoreInstaller : MonoInstaller
  {
    [Header("Global dependencies")]
    [SerializeField] private GameConfigSO _gameConfig;
    [SerializeField] private ShapeConfigSO[] _shapeConfigs;
    [SerializeField] private ColorConfigSO[] _colorConfigs;
    [SerializeField] private ScaleConfigSO[] _scaleConfigs;
    [SerializeField] private FormationConfigSO[] _allFormations;
    [SerializeField] private ArmySpawner _armySpawner;
 
    public override void InstallBindings()
    {
        
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;

        Container.Bind<GameConfigSO>().FromInstance(_gameConfig);
        Container.BindInstance(_shapeConfigs);
        Container.BindInstance(_colorConfigs).AsSingle();
        Container.BindInstance(_scaleConfigs).AsSingle();
        Container.BindInstance(_allFormations).AsSingle();
        Container.Bind<BattleManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<ArmySpawner>().FromInstance(_armySpawner).AsSingle().NonLazy();
        Container.Bind<UnitFactory>().AsSingle().NonLazy();
        Container.Bind<UnitCreator>().AsSingle().NonLazy();
    }
  }
