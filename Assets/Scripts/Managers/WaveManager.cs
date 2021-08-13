using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


// TODO
// Wave manager qui supériorise enemies manager
// Pour chaque wave
//    Pour chaque spawner
//       Pour chaque type d'ennemis
//          Pouvoir décider du nombre d'ennemi à spawn

public class WaveManager : MonoBehaviour
{
   [Header("Wave Infos")]
   private int numberOfWaves;
   public int NumberOfWaves
   {
      get => numberOfWaves;
      set => numberOfWaves = value;
   }
   
   [SerializeField] private List<WaveData> waveDatas;
   public List<WaveData> WaveDatas => waveDatas;
   
   private int actualWaveNumber;
   public int ActualWaveNumber => actualWaveNumber;
   [Header("UI")]
   [SerializeField] private Text waveCount_Text;
   private Coroutine cor;

   private EnemiesManager enemiesManager;

   public void Init()
   {
      actualWaveNumber = 1;
      numberOfWaves = waveDatas.Count;
      UpdateWaveText();
      enemiesManager = GameManager.Instance.P_EnemiesManager;
   }

   public void Restart()
   {
      Init();
   }

   public void StartWaveSequence()
   {
      if(cor == null)
         cor = StartCoroutine(StartWave());
   }

   public void UpdateWaveText()
   {
      waveCount_Text.text = "wave " + actualWaveNumber + "/" + numberOfWaves;
   }

   // Commence une vague et spawn des ennemis
   IEnumerator StartWave()
   {
         UpdateWaveText();
         GameManager.Instance.P_EnemiesManager.GetTargets();
         GameManager.Instance.P_TurretManager.GetTargets();

         WaveData actualWave = waveDatas[actualWaveNumber - 1];
         foreach (SpawnerData spawnerData in actualWave.SpawnerDatas)
         {
            spawnerData.SpawnerTr.GetComponent<Spawner>().StartSpawningSequence(spawnerData.EnemyGroups);
            foreach (EnemyGroup enemyGroup in spawnerData.EnemyGroups)
            {
               enemiesManager.ItemsLeftToSpawn += enemyGroup.EnemyNumberToSpawn;
            }
         }

         while (!enemiesManager.isWaveFinished())
         {
            yield return null;
         }
         actualWaveNumber++;
         if (actualWaveNumber > numberOfWaves)
         {
            /*actualWaveNumber = 1;
            Debug.Log("WAVE FINI NEXT MAP SHOULD BE LOADED");*/
            
            SceneManager.Instance.LoadNextScene();
         }

      GameManager.Instance.ChangePhase(eGameState.AutoLoot);
      cor = null;
      
   }
}

[System.Serializable]
public struct SpawnerData
{
   private Spawner spawnerScript;

   public Spawner SpawnerScript
   {
      get => spawnerScript;
      set => spawnerScript = value;
   }
    
   [SerializeField] private Transform spawnerTr;
   public Transform SpawnerTr => spawnerTr;
    
   [SerializeField] private List<EnemyGroup> enemyGroups;
   public List<EnemyGroup> EnemyGroups => enemyGroups;
}


[System.Serializable]
public struct WaveData
{
   [SerializeField] private List<SpawnerData> spawnerDatas;
   public List<SpawnerData> SpawnerDatas => spawnerDatas;
}


