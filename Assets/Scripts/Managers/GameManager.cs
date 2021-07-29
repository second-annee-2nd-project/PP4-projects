using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum eGameState
{
    Wave,
    Shop,
    AutoLoot
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get => instance;
    }

    private eGameState _eGameState;

    public eGameState EGameState => _eGameState;

    private Grid actualGrid;

    public Grid ActualGrid => actualGrid;
    
    private Joystick joystick;
    public Joystick Joystick => joystick;
    
    private FireButton fireButton;
    public FireButton FireButton => fireButton;
    
    #region Managers
    
        private WaveManager waveManager;
        public WaveManager P_WaveManager => waveManager;

        private SoundManager soundManager;
        public SoundManager P_SoundManager => soundManager;

        private UI_Manager uiManager;
        public UI_Manager P_UiManager => uiManager;
        private ShopManager shopManager;
        public ShopManager P_ShopManager => shopManager;
        
        private EnemiesManager enemiesManager;
        public EnemiesManager P_EnemiesManager => enemiesManager;
        
        private TurretManager turretManager;
        public TurretManager P_TurretManager => turretManager;

        private TeamManager teamManager;
        public TeamManager P_TeamManager => teamManager;
        private LootManager lootManager;
        public LootManager P_LootManager => lootManager;

        private PathRequestManager pathRequestManager;
        public PathRequestManager P_PathRequestManager => pathRequestManager;

        private PlayerBehaviour player;
        public PlayerBehaviour Player => player;

        private UI_Manager UI_Manager;
        public UI_Manager P_UI_Manager => UI_Manager;
        #endregion

        public CameraController CC => cc;
    
    
    
    private CameraController cc;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        Init();
    }

    private void Start()
    {
        
        
        cc = FindObjectOfType<CameraController>();

        StartGame();
    }

    private void Init()
    {
        fireButton = FindObjectOfType<FireButton>();
        joystick = FindObjectOfType<Joystick>();
        uiManager = FindObjectOfType<UI_Manager>();
        actualGrid = FindObjectOfType<Grid>();
        lootManager = FindObjectOfType<LootManager>();
        teamManager = FindObjectOfType<TeamManager>();
        teamManager.Init();
        shopManager = FindObjectOfType<ShopManager>();
        enemiesManager = FindObjectOfType<EnemiesManager>();
        turretManager = FindObjectOfType<TurretManager>();
        waveManager = FindObjectOfType<WaveManager>();
        pathRequestManager = FindObjectOfType<PathRequestManager>();
        soundManager = FindObjectOfType<SoundManager>();
        player = FindObjectOfType<PlayerBehaviour>();
        UI_Manager = FindObjectOfType<UI_Manager>();
    }

    public void Restart()
    {
        GameManager.Instance.P_UI_Manager.RenderRetryButton(false);
        
        teamManager.Restart();
        shopManager.Restart();
        enemiesManager.Restart();
        turretManager.Restart();
        waveManager.Restart();
        actualGrid.Restart();
        pathRequestManager.Restart();
        player.Restart();

        StartGame();
    }

    private void StartGame()
    {
        cc.Init();
        pathRequestManager.Init();
        waveManager.Init();
        ChangePhase(eGameState.Shop);

        //



    }

    public void ChangePhase(eGameState newEGameState)
    {
        switch (newEGameState)
        {
            case eGameState.Shop:
                    _eGameState = eGameState.Shop;
                    shopManager.StartShopSequence();
                break;
            
            case eGameState.Wave:
                    _eGameState = eGameState.Wave;
                    //TODO
                    //Gérer l'UI
                    waveManager.StartWaveSequence();
                    break;
            case eGameState.AutoLoot:
                _eGameState = eGameState.AutoLoot;
                lootManager.StartAutoLootSequence();
                break;
        }

        cc.ChangeState(newEGameState);

    }

    private IEnumerator Game()
    {
        _eGameState = eGameState.Wave;
        while (!enemiesManager.isWaveFinished())
        {
            
            yield return null;
        }
        _eGameState = eGameState.Shop;
        
    }
}
