using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class WaveManager
{ 
   [SerializeField] private int enemyNumberToSpawn;
   public int EnemyNumberToSpawn => enemyNumberToSpawn;

   [SerializeField] private Transform spawnerPos;
   public Transform SpawnerPos => spawnerPos;

   [SerializeField] private GameObject typeOfEnemyToSpawn; 
   public GameObject TypeOfEnemyToSpawn => typeOfEnemyToSpawn;
   
   // private int totalWaveNumber;
   // public int TotalWaveNumber => totalWaveNumber;
   //
   // private int waveNumber;
   // public int WaveNumber
   // {
   //    get => waveNumber;
   //    set => waveNumber = value;
   // }
}
