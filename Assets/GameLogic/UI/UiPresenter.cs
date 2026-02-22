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

    [Header("=== Panels ===")]
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _runningPanel;
    [SerializeField] private GameObject _finishedPanel;

    [Header("=== Main Menu ===")]
    [SerializeField] private UnityEngine.UI.Button _btnFormationA;
    [SerializeField] private UnityEngine.UI.Button _btnFormationB;
    [SerializeField] private UnityEngine.UI.Button _btnStartGame;
    [SerializeField] private TextMeshProUGUI _txtFormationA;
    [SerializeField] private TextMeshProUGUI _txtFormationB;

    [Header("=== Running ===")]
    [SerializeField] private TextMeshProUGUI _txtArmyAAlive;
    [SerializeField] private TextMeshProUGUI _txtArmyBAlive;

    [Header("=== Finished ===")]
    [SerializeField] private TextMeshProUGUI _txtWinner;
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
        _btnStartGame?.onClick.AddListener(OnStartGame);
        _btnRestart?.onClick.AddListener(OnRestart);
        
        _battleManager.Gamestate.DistinctUntilChanged().Subscribe(OnGameStateChange).AddTo(_disposables);
        
        SetPanelActive(_mainMenuPanel);
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
            _txtFormationA.text = $"Form A: {newFormation.name}";
        }
        else
        {
            _formationIndexB = (_formationIndexB + 1) % _allFormations.Length;
            var newFormation = _allFormations[_formationIndexB];
            
            _battleManager.SetFormation(ArmyType.ArmyB, newFormation);
            _txtFormationB.text = $"Form B: {newFormation.name}";
        }
    }

    private void UpdateFormationLabels()
    {
        if (_allFormations != null && _allFormations.Length > 0)
        {
            _txtFormationA.text = $"Form A: {_allFormations[_formationIndexA].name}";
            _txtFormationB.text = $"Form B: {_allFormations[_formationIndexB].name}";
        }
    }

    private void OnGameStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                SetPanelActive(_mainMenuPanel);
                _battleManager.CleanupAll();
                UpdateFormationLabels();
            break;
                
            case GameState.Running:
                SetPanelActive(_runningPanel);
                _battleManager.CleanupAll();
                _armySpawner.SpawnArmy(ArmyType.ArmyA);
                _armySpawner.SpawnArmy(ArmyType.ArmyB);
            break;
                
            case GameState.Finished:
                SetPanelActive(_finishedPanel);
                ShowWinner(_battleManager.Winners.Value);
            break;
        }
    }

    private void SetPanelActive(GameObject activePanel)
    {
        _mainMenuPanel.SetActive(false);
        _runningPanel.SetActive(false);
        _finishedPanel.SetActive(false);
        activePanel?.SetActive(true);
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
    }

    private void OnStartGame()
    {
        _battleManager.Gamestate.Value = GameState.Running;
        // update counters
        Observable.EveryUpdate()
            .Where(_ => _battleManager.Gamestate.Value == GameState.Running)
            .Subscribe(_ => UpdateAliveCounts())
            .AddTo(_disposables);
    }

    private void OnRestart()
    {
        _battleManager.Gamestate.Value = GameState.MainMenu;
    }

    private void OnDestroy()
    {
        _disposables?.Dispose();
        _btnFormationA?.onClick.RemoveAllListeners();
        _btnFormationB?.onClick.RemoveAllListeners();
        _btnStartGame?.onClick.RemoveAllListeners();
        _btnRestart?.onClick.RemoveAllListeners();
    }
}
