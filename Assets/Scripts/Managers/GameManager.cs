using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameStateEnum
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

    private GameStateEnum gameState;

    public GameStateEnum GameState => gameState;

    private Grid actualGrid;

    public Grid ActualGrid => actualGrid;

    private ShopManager shopManager;
    public ShopManager P_ShopManager => shopManager;
    private EnemiesManager enemiesManager;
    public EnemiesManager P_EnemiesManager => enemiesManager;
    private CameraController cc;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Start()
    {
        actualGrid = FindObjectOfType<Grid>();
        shopManager = FindObjectOfType<ShopManager>();
        enemiesManager = FindObjectOfType<EnemiesManager>();
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
        ChangePhase(GameStateEnum.Shop);

        //



    }

    public void ChangePhase(GameStateEnum newGameState)
    {
        switch (newGameState)
        {
            case GameStateEnum.Shop:
                    gameState = GameStateEnum.Shop;
                    shopManager.StartShopSequence();
                break;
            
            case GameStateEnum.Wave:
                    gameState = GameStateEnum.Wave;
                    //TODO
                    //Gérer l'UI
                    enemiesManager.StartWaveSequence();
                    break;
        }
        // 
        //
        // Check si la vague est la dernière, si c'est la dernière changer de map
        // Sinon commencer le shop
        // actualiser le numéro de la vague
        // 
        
        // EnemiesManager.StartSpawning
        
    }

    private IEnumerator Game()
    {
        gameState = GameStateEnum.Wave;
        while (!enemiesManager.isWaveFinished())
        {
            
            yield return null;
        }
        gameState = GameStateEnum.Shop;
        
    }
}
