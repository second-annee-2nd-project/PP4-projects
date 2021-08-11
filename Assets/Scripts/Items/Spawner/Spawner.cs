using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Spawner : MonoBehaviour
{
    [SerializeField] private Vector3 objectSize;
    
    [SerializeField] private List<List<EnemyGroup>> enemyGroupsToSpawn;

    private TeamManager teamManager;

    private Dictionary<string, Coroutine> CoroutineGroup;

    public void StartSpawningSequence(params List<EnemyGroup>[] enemyGroups)
    {
        teamManager = GameManager.Instance.P_TeamManager;
        CoroutineGroup = new Dictionary<string, Coroutine>();
        for (int i = 0; i < enemyGroups.Length; i++)
        {
            List<EnemyGroup> enemyGroupList = enemyGroups[i];
            for (int j = 0; j < enemyGroups[i].Count; j++)
            {
                EnemyGroup enemyGroupToSpawn = enemyGroupList[j];
                string newCorname = "(" + i + "; " + j + ")";
                CoroutineGroup.Add(newCorname, StartCoroutine(StartSpawning(enemyGroupToSpawn, newCorname)));
            }
            
        }
        /*
        foreach (List<EnemyGroup> enemyGroupList in enemyGroups)
        {
            foreach (EnemyGroup enemyGroup in enemyGroupList)
            {
                StartCoroutine(BeginSpawn(enemyGroup));
            }
        }*/
    }
    private IEnumerator StartSpawning(EnemyGroup eg, string coroutineID)
    {
        int enemiesLeftToSpawn = eg.EnemyNumberToSpawn;
        float timerBeforeNextSpawn = eg.TimerBetweenSpawns;
        
        yield return new WaitForSeconds(eg.TimerBeforeFirstSpawn);
        while (enemiesLeftToSpawn > 0)
        {
            if (enemiesLeftToSpawn > 0)
            {
                if (timerBeforeNextSpawn <= 0f)
                {
                    GameObject prefab = GameManager.Instance.P_EnemiesManager.GetPrefab(eg.TypeOfEnemyToSpawn);
                    Spawn(prefab);

                    --enemiesLeftToSpawn;
                    timerBeforeNextSpawn = eg.TimerBetweenSpawns;
                }
                else
                {
                    timerBeforeNextSpawn -= Time.deltaTime;
                }
            }
            yield return null;
        }
        
        CoroutineGroup.Remove(coroutineID);
    }


    private void Spawn(GameObject prefab)
    {
    
        EnemiesManager enemiesManager = GameManager.Instance.P_EnemiesManager;
        int x = (int) objectSize.x;
        int y = (int) objectSize.y;
        int z = (int) objectSize.z;

        float rx = transform.position.x - x/2f + Random.Range(0f, x);
        float ry = GameManager.Instance.ActualGrid.CenterPosition.y - GameManager.Instance.ActualGrid.P_GridHeight * 0.5f;
        float rz = transform.position.z - z/2f + Random.Range(0f, z);
        
        GameObject enemy = Instantiate(prefab,
            new Vector3(rx, ry, rz), Quaternion.identity);
        
        
        enemiesManager.AddItemToList(enemy);
        enemiesManager.ItemsLeftToSpawn--;
    }
}

public struct GroupID
{
    private int id;
    public int ID => id;
    private eEnemyType enemyType;
    public eEnemyType EnemyType => enemyType;
}

[System.Serializable]
public struct EnemyGroup
{
    [SerializeField] private eEnemyType typeOfEnemyToSpawn; 
    public eEnemyType TypeOfEnemyToSpawn => typeOfEnemyToSpawn;
    [SerializeField] private int enemyNumberToSpawn;
    public int EnemyNumberToSpawn => enemyNumberToSpawn;
    [SerializeField] private float timerBetweenSpawns;
    public float TimerBetweenSpawns => timerBetweenSpawns;
    [SerializeField] private float timerBeforeFirstSpawn;
    public float TimerBeforeFirstSpawn => timerBeforeFirstSpawn;


}


