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
    
    [SerializeField] private Joystick joystick;
    public Joystick Joystick => joystick;
    
    private FireButton fireButton;
    public FireButton FireButton => fireButton;
    
    private PlayerBehaviour player;
    public PlayerBehaviour Player => player;
    
    private UpgradeWeapon upgradeWeapon;
    public UpgradeWeapon  P_UpgradeWeapon =>  upgradeWeapon;

    private TurretBtnUI turretBtnUI;
    public TurretBtnUI P_TurretBtnUI => turretBtnUI;
    #region Managers
    
        private WaveManager waveManager;
        public WaveManager P_WaveManager => waveManager;

        private SoundsManager soundsManager;
        public SoundsManager P_SoundsManager => soundsManager;

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

        private UI_Manager UI_Manager;
        public UI_Manager P_UI_Manager => UI_Manager;
        
        private WeaponsManager weaponsManager;
        public WeaponsManager P_WeaponsManager => weaponsManager;
        #endregion

        public CameraController CC => cc;
    
    
    
    private CameraController cc;
    private WeaponUI weaponUI;
    public WeaponUI P_WeaponUI => weaponUI;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this);
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
        turretBtnUI = FindObjectOfType<TurretBtnUI>();
        fireButton = FindObjectOfType<FireButton>();
        // joystick = FindObjectOfType<Joystick>();
        uiManager = FindObjectOfType<UI_Manager>();
        upgradeWeapon = FindObjectOfType<UpgradeWeapon>();
        actualGrid = FindObjectOfType<Grid>();
        lootManager = FindObjectOfType<LootManager>();
        teamManager = FindObjectOfType<TeamManager>();
        teamManager.Init();
        shopManager = FindObjectOfType<ShopManager>();
        enemiesManager = FindObjectOfType<EnemiesManager>();
        turretManager = FindObjectOfType<TurretManager>();
        waveManager = FindObjectOfType<WaveManager>();
        pathRequestManager = FindObjectOfType<PathRequestManager>();
        soundsManager = FindObjectOfType<SoundsManager>();
        player = FindObjectOfType<PlayerBehaviour>();
        UI_Manager = FindObjectOfType<UI_Manager>();
        weaponsManager = FindObjectOfType<WeaponsManager>();
        weaponUI = FindObjectOfType<WeaponUI>();
    }

    public void Restart()
    {
        GameManager.Instance.P_UI_Manager.RenderRetryButton(false);
        ChangePhase(eGameState.Shop);
        
        teamManager.Restart();
        enemiesManager.Restart();
        turretManager.Restart();
        actualGrid.Restart();
        lootManager.Restart();
        pathRequestManager.Restart();
        player.Restart();
        shopManager.Restart();
        waveManager.Restart();
       shopManager.UIRestart();
        Time.timeScale = 1;
        if (enemiesManager.ItemsLeftToSpawn > 0)
        {
            enemiesManager.ItemsLeftToSpawn = 0;
        
        }
        StartGame();
        waveManager.ActualWaveNumber = 0;
    }

    private void StartGame()
    {
        ChangePhase(eGameState.Shop);
        cc.Init();
        pathRequestManager.Init();
        waveManager.Init();
        

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
    public void PauseGame ()
    {
        Time.timeScale = 0;
        P_UiManager.RenderJoystick(false);
        P_UiManager.RenderPause(true);
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
        P_UiManager.RenderJoystick(true);
        P_UiManager.RenderPause(false);
    }
    
}
