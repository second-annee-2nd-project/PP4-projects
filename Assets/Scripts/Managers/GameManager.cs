using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum eGameState
{
    Wave,
    Shop
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
    
    #region Managers
    
        private WaveManager waveManager;
        public WaveManager P_WaveManager => waveManager;

        private ShopManager shopManager;
        public ShopManager P_ShopManager => shopManager;
        
        private EnemiesManager enemiesManager;
        public EnemiesManager P_EnemiesManager => enemiesManager;
        
        private TurretManager turretManager;
        public TurretManager P_TurretManager => turretManager;

        private TeamManager teamManager;
        public TeamManager P_TeamManager => teamManager;
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
        
        actualGrid = FindObjectOfType<Grid>();
        teamManager = FindObjectOfType<TeamManager>();
        teamManager.Init();
        shopManager = FindObjectOfType<ShopManager>();
        enemiesManager = FindObjectOfType<EnemiesManager>();
        turretManager = FindObjectOfType<TurretManager>();
        waveManager = FindObjectOfType<WaveManager>();
    }

    private void Start()
    {
        
        
        cc = FindObjectOfType<CameraController>();

        StartGame();
    }

    private void Update()
    {
       /* */
     
    }

    private void StartGame()
    {
        cc.Init();
        waveManager.UpdateWaveText();
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
