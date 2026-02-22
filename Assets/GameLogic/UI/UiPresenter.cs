using UnityEngine;
using GameEnums;
using UniRx;
using Zenject;
using TMPro;

public class UiPresenter : MonoBehaviour
{
    [Inject] private BattleManager _battleManager;
    [Inject] private ArmySpawner _armySpawner;
    [Inject] private FormationConfigSO[] _allFormations;

    [Header("Panels")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _runningPanel;
    [SerializeField] private GameObject _finishedPanel;

    [Header("Main Menu")]
    [SerializeField] private UnityEngine.UI.Button _btnRespawnA;
    [SerializeField] private UnityEngine.UI.Button _btnRespawnB;
    [SerializeField] private UnityEngine.UI.Button _btnFormationA;
    [SerializeField] private UnityEngine.UI.Button _btnFormationB;
    [SerializeField] private UnityEngine.UI.Button _btnStartGame;
    [SerializeField] private TextMeshProUGUI _txtFormationA;
    [SerializeField] private TextMeshProUGUI _txtFormationB;

    [Header("Running")]
    [SerializeField] private TextMeshProUGUI _txtArmyAAlive;
    [SerializeField] private TextMeshProUGUI _txtArmyBAlive;

    [Header("Finished")]
    [SerializeField] private TextMeshProUGUI _txtWinner;
    [SerializeField] private TextMeshProUGUI _txtStatistics;
    [SerializeField] private UnityEngine.UI.Button _btnRestart;

    // State
    private int _formationIndexA;
    private int _formationIndexB;
    private readonly CompositeDisposable _disposables = new();

    private void Awake()
    {
        // btns
        _btnFormationA?.onClick.AddListener(() => CycleFormation(ArmyType.ArmyA));
        _btnFormationB?.onClick.AddListener(() => CycleFormation(ArmyType.ArmyB));
        _btnRespawnA?.onClick.AddListener(() => _armySpawner.SpawnArmy(ArmyType.ArmyA));
        _btnRespawnB?.onClick.AddListener(() => _armySpawner.SpawnArmy(ArmyType.ArmyB));
        _btnStartGame?.onClick.AddListener(OnStartGame);
        _btnRestart?.onClick.AddListener(OnRestart);
        
        _battleManager.Gamestate.Where(v => v == GameState.Finished).Subscribe(_ => SetFinishPanel()).AddTo(_disposables);
        CycleFormation(ArmyType.ArmyA);
        CycleFormation(ArmyType.ArmyB);
        _armySpawner.SpawnArmy(ArmyType.ArmyA);
        _armySpawner.SpawnArmy(ArmyType.ArmyB);
        UpdateFormationLabels();
    }

    private void CycleFormation(ArmyType army)
    {
        if (_allFormations == null || _allFormations.Length == 0) return;
        
        if (army == ArmyType.ArmyA)
        {
            _formationIndexA = (_formationIndexA + 1) % _allFormations.Length;
            var newFormation = _allFormations[_formationIndexA];
            
            _battleManager.SetFormation(ArmyType.ArmyA, newFormation);
            _txtFormationA.text = $"Formation: {newFormation.name}";
        }
        else
        {
            _formationIndexB = (_formationIndexB + 1) % _allFormations.Length;
            var newFormation = _allFormations[_formationIndexB];
            
            _battleManager.SetFormation(ArmyType.ArmyB, newFormation);
            _txtFormationB.text = $"Formation: {newFormation.name}";
        }
    }

    private void UpdateFormationLabels()
    {
        if (_allFormations != null && _allFormations.Length > 0)
        {
            _txtFormationA.text = $"Formation: {_allFormations[_formationIndexA].name}";
            _txtFormationB.text = $"Formation: {_allFormations[_formationIndexB].name}";
        }
    }

    private void SetFinishPanel()
    {
        _runningPanel.SetActive(false);
        _finishedPanel.SetActive(true);
        ShowWinner(_battleManager.Winners.Value);
    }

    private void UpdateAliveCounts()
    {
        int aliveA = 0, aliveB = 0;
        foreach (var unit in _battleManager._armyA) if (unit.IsAlive) aliveA++;
        foreach (var unit in _battleManager._armyB) if (unit.IsAlive) aliveB++;
        
        _txtArmyAAlive?.SetText($"Army A: {aliveA}");
        _txtArmyBAlive?.SetText($"Army B: {aliveB}");
    }

    private void ShowWinner(ArmyType winner)
    {
        _txtWinner?.SetText(winner == ArmyType.ArmyA ? "Army A Wins!" : "Army B Wins!");
        _txtStatistics.SetText(
           $"Total hits : {_battleManager.TotalHits.Value}\nTotal A Army damage : {_battleManager.TotalAArmyDamage.Value}\n Total B Army damage : {_battleManager.TotalBArmyDamage.Value}"
        );
    }

    private void OnStartGame()
    {
        _battleManager.Gamestate.Value = GameState.Running;
        _mainMenuPanel.SetActive(false);
        _runningPanel.SetActive(true);
        // update counters
        Observable.EveryUpdate()
            .Where(_ => _battleManager.Gamestate.Value == GameState.Running)
            .Subscribe(_ => UpdateAliveCounts())
            .AddTo(_disposables);
    }

    private void OnRestart()
    {
        _battleManager.Gamestate.Value = GameState.MainMenu;
        _battleManager.CleanupAll();
        _armySpawner.SpawnArmy(ArmyType.ArmyA);
        _armySpawner.SpawnArmy(ArmyType.ArmyB);
        _finishedPanel.SetActive(false);
        _mainMenuPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        _disposables?.Dispose();
        _btnFormationA?.onClick.RemoveAllListeners();
        _btnFormationB?.onClick.RemoveAllListeners();
        _btnStartGame?.onClick.RemoveAllListeners();
        _btnRestart?.onClick.RemoveAllListeners();
        _btnRespawnA?.onClick.RemoveAllListeners();
        _btnRespawnB?.onClick.RemoveAllListeners();
    }
}
