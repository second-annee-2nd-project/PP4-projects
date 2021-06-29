using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField] private float delayBeforeFirstSpawn;
    [SerializeField] private float delayBetweenSpawns;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject container;
    [SerializeField] private List<Spawner> spawners;

    private List<GameObject> targets;
    public List<GameObject> Targets => targets;

    private float timerBeforeNextSpawn;
    
    private int enemiesLeftToSpawn;
    private List<GameObject> aliveEnemies;
    private Coroutine cor;
    [Space()] [Header("Size = total wave number")]
   [SerializeField] private List<WaveManager> waveManager;

    void Start()
    {
        aliveEnemies = new List<GameObject>();
        if (container == null) GameObject.Find("EnemyContainer");
        // StartCoroutine(nameof(StartWave));
        ShopManager.Instance.NumberOfWaves = waveManager.Count;

    }

    public void StartWaveSequence()
    {
        if(cor == null)
            cor = StartCoroutine(StartWave());
    }

    // Commence une vague et spawn des ennemis
    IEnumerator StartWave()
    {
        enemiesLeftToSpawn = waveManager[ShopManager.Instance.ActualWaveNumber-1].EnemyNumberToSpawn;
        GetTargets();
        yield return new WaitForSeconds(delayBeforeFirstSpawn);
        while (!isWaveFinished())
        {
            if (enemiesLeftToSpawn > 0)
            {
                if (timerBeforeNextSpawn <= 0f)
                {
                    GameObject enemy = Instantiate(waveManager[ShopManager.Instance.ActualWaveNumber-1].TypeOfEnemyToSpawn,
                        new Vector3(waveManager[ShopManager.Instance.ActualWaveNumber-1].SpawnerPos.position.x, 0,waveManager[ShopManager.Instance.ActualWaveNumber-1].SpawnerPos.position.z), Quaternion.identity);
                    enemy.transform.parent = container.transform;
                    aliveEnemies.Add(enemy);

                    --enemiesLeftToSpawn;
                    timerBeforeNextSpawn = delayBetweenSpawns;
                }
                else
                {
                    timerBeforeNextSpawn -= Time.deltaTime;
                }
            }

            yield return null;
        }

        cor = null;
        GameManager.Instance.ChangePhase(GameStateEnum.Shop);
    }

    public void RemoveEnemyFromList(GameObject enemy)
    {
        aliveEnemies.Remove(enemy);
    }

    public bool isWaveFinished()
    {
        if (aliveEnemies.Count <= 0 && enemiesLeftToSpawn == 0) return true;
        return false;
    }

    private void GetTargets()
    {
        targets = new List<GameObject>(GameObject.FindGameObjectsWithTag("Turret"));
    }

    public Transform GetNearestTarget(Vector3 pos)
    {
        if (aliveEnemies.Count <= 0) return null;
        Transform nearest = aliveEnemies[0].transform;
        float minSqrDist = (nearest.position - pos).sqrMagnitude;
        for (int i = 1; i < aliveEnemies.Count; i++)
        {
            float sqrDist = (aliveEnemies[i].transform.position - pos).sqrMagnitude;
            if (minSqrDist > sqrDist)
            {
                nearest = aliveEnemies[i].transform;
                minSqrDist = sqrDist;
            }
        }

        return nearest;
    }
    
    /*public List<GameObject>()
    {
        
    }*/
}
