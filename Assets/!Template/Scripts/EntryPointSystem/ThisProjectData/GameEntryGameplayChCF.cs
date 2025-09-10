using UnityEngine;

public class GameEntryGameplayChCF : GameEntryGameplay
{
    private IInput input;
    private GlobalMover globalMover;
    private Spawner spawner;
    private GrappableObjectMediator objectMediator;
    private GameStateManager stateManager;
    private ScoreSystem scoreSystem;
    private SpawnManager spawnManager;
    private ResourceSystem resourceSystem;

    public static GameSessionDataContainer DataContainer { get; private set; }

    private PlayerChCF player;
    private TimerStarter timerStarter;
    private GameTimer gameTimer;

    public static LevelData DataLevel;

    public override void Init()
    {
        //base.Init();
        Debug.Log("GameEntryGameplayChCF Init");

        InitializeReferences();
        InitializeLevel();
        InitializeInterface();
        SetupScoreSystem();
        SetupSpawnerAndProgressBar();
        SetupInterfaceEvents();
        SetupHealthContainer();
        SetupSpawnManager();
        SetupGameStateEvents();
        SetupPlayer();
        SetupWinLoseCondition();

        stateManager.SetState(GameState.Tutorial);
    }

    private void InitializeReferences()
    {
        input = InputFabric.GetOrCreateInpit();
        globalMover = Object.FindObjectOfType<GlobalMover>();
        spawner = Object.FindObjectOfType<Spawner>();

        objectMediator = new GrappableObjectMediator();
        GrapCollider.Mediator = objectMediator;
        stateManager = new GameStateManager();
        scoreSystem = new ScoreSystem();
        spawnManager = new SpawnManager();
        resourceSystem = new ResourceSystem();

        DataContainer = Resources.Load<GameSessionDataContainer>("GameSessionDataContainer").Clone();
        if (DataLevel == null)
            //DataLevel = new LevelData() { ScoreTarget = 100, Timer = 50 };
            DataLevel = Resources.Load<LevelData>($"Levels/{DataContainer.StandartLevelData}");

        timerStarter = (new GameObject()).AddComponent<TimerStarter>();
        timerStarter.Play(DataLevel.Timer);
        gameTimer = timerStarter.Timer;
    }

    private void InitializeInterface()
    {
        InterfaceManager.Init();
        InterfaceManager.BarMediator.ShowForID("Score", 0);
    }

    private void SetupScoreSystem()
    {
        scoreSystem.OnScoreChange += (score, point) =>
        {
            InterfaceManager.BarMediator.ShowForID("Score", score);
        };

        scoreSystem.OnAddScore += InterfaceManager.CreateScoreFlyingText;
        scoreSystem.OnRemoveScore += InterfaceManager.CreateScoreFlyingText;
    }

    private void SetupSpawnerAndProgressBar()
    {
        if (spawner == null)
            return;

        spawner.OnInstructionProgress += (id, progress) =>
        {
            InterfaceManager.BarMediator.ShowForID("Progress", progress);
        };
    }

    private void SetupInterfaceEvents()
    {
        InterfaceManager.OnClose += (window) =>
        {
            if (window is PauseWindowUI)
                stateManager.BackState();
        };

        InterfaceManager.OnOpen += (window) =>
        {
            if (window is PauseWindowUI)
                stateManager.SetState(GameState.Pause);
        };

        gameTimer.OnTick += (s) => InterfaceManager.BarMediator.ShowForID("Timer", s);
    }

    private void SetupHealthContainer()
    {
        DataContainer.HealthContainer.OnChangeValue += (life) => InterfaceManager.BarMediator.ShowForID("Life", life);
        DataContainer.HealthContainer.UpdateValue();
    }

    private void SetupSpawnManager()
    {
        var settings = Resources.Load<SpawnerSettings>("Spawn/SpawnerSettings");
        spawnManager.Init(spawner, settings);

        if(spawner)
            spawnManager.OnChangeSpeed += spawner.SetSpeed;
        spawnManager.OnChangeSpeed += (speed) => DataContainer.SpeedGame = speed;
        if(globalMover)
            spawnManager.OnChangeSpeed += globalMover.SetSpeedCoef;
    }

    private void SetupGameStateEvents()
    {
        stateManager.OnWin += () =>
        {
            RecordData recordData = LeaderBoard.GetScore($"score_{LevelSelectWindow.CurrentLvl}");
            InterfaceManager.ShowWinWindow(scoreSystem.Score, recordData != null ? recordData.score : 0);
            LeaderBoard.SaveScore($"score_{LevelSelectWindow.CurrentLvl}", scoreSystem.Score);
            LevelSelectWindow.CompliteLvl();
        };

        stateManager.OnLose += () =>
        {
            RecordData recordData = LeaderBoard.GetScore($"score_{LevelSelectWindow.CurrentLvl}");
            InterfaceManager.ShowLoseWindow(scoreSystem.Score, recordData != null ? recordData.score : 0);
        };

        stateManager.OnStateChange += (state) =>
        {
            GamePause.SetPause(state != GameState.Game);

            if (state == GameState.Win || state == GameState.Lose)
            {
                player.gameObject.SetActive(false);
                var musicSource = GameObject.Find("AudioSource(Music)")?.GetComponent<AudioSource>();
                musicSource?.Stop();
                gameTimer.Stop();
            }
        };

        GameObject musicGO = UnityEngine.GameObject.Find("AudioSource(Music)");
        musicGO.SetActive(false);
        TutorialWindow tutorialWindow = Object.FindObjectOfType<TutorialWindow>();

        tutorialWindow.Init(input);
        tutorialWindow.OnClick += () => musicGO.SetActive(true);
        tutorialWindow.OnClick += () => stateManager.SetState(GameState.Game);
    }

    private void SetupPlayer()
    {
        player = Object.FindObjectOfType<PlayerChCF>();
        player.Init(input);

        player.OnDamage += () =>
        {
            DataContainer.HealthContainer.RemoveValue(1);
        };

        objectMediator.Subscribe<AddScoreGrapAction>((beh, grapOb) =>
        {
            scoreSystem.AddScore(beh.AddScore);
        });

        player.OnMove += (v2) => scoreSystem.AddScore(1);
    }

    private void SetupWinLoseCondition()
    {
        gameTimer.OnComplete += () => stateManager.SetState(GameState.Win);

        DataContainer.HealthContainer.OnDownfullValue += (_) =>
        {
            stateManager.SetState(GameState.Lose);
        };
    }

    private void InitializeLevel()
    {
        Transform levelTr = GameObject.Find("Level").transform;
        if(DataLevel.LevelPrefab)
            GameObject.Instantiate(DataLevel.LevelPrefab, levelTr);
    }
}