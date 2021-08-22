using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public enum eEnemyType
{
    classic,
    assault,
    armored,
    speed,
    warrior,
    explosive
}

public class EnemiesManager : UnitManager
{
    [SerializeField] private EnemyDictionary enemyPrefabs;
    [SerializeField] private int sizeToPool = 50;
    public EnemyDictionary EnemyPrefabs => enemyPrefabs;
    // Properties utilisé dans chaque Spawner
    private Coroutine cor;
    private Dictionary<eEnemyType, List<GameObject>> enemiesPoolDictionary;
   [SerializeField] private List<EnemyBehaviour> ebList = new List<EnemyBehaviour>();
    private List<ExplosiveEnemyBehaviour> explosiveEnemyList = new List<ExplosiveEnemyBehaviour>();
    
    protected override void Start()
    {
        base.Start();
        team = eTeam.enemy;
        enemiesPoolDictionary = new Dictionary<eEnemyType, List<GameObject>>();
        Init();
       
    }
    
    private void Init()
    {
        foreach (eEnemyType enemyType in (eEnemyType[]) Enum.GetValues(typeof(eEnemyType)))
        {
            GameObject prefab = GetPrefab(enemyType);
            List<GameObject> newInstantiatedList = new List<GameObject>();
            
            for (int i = 0; i < sizeToPool; i++)
            {
                GameObject newEnemy = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                newEnemy.transform.parent = transform;
                foreach (var eb in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    ebList.Add(eb.GetComponent<EnemyBehaviour>());
                }
                foreach (var explosive in GameObject.FindGameObjectsWithTag("EnemyExplose"))
                {
                    explosiveEnemyList.Add(explosive.GetComponent<ExplosiveEnemyBehaviour>());
                }
                newEnemy.SetActive(false);
                newInstantiatedList.Add(newEnemy);
            }

            enemiesPoolDictionary.Add(enemyType, newInstantiatedList);

        }
    }

    void Update()
    {
        foreach (var eb in ebList)
        {
            if (eb.gameObject.activeInHierarchy && eb.IsDead)
            {
               eb.Respawn();
            }
        }
        foreach (var exb in explosiveEnemyList)
        {
            if (exb.gameObject.activeInHierarchy && exb.IsDead)
            {
                exb.Respawn();
            }
        }
    }
    public override void Restart()
    {
        for (int i = 0; i < instantiatedItems.Count; i++)
        {
            instantiatedItems[i].SetActive(false);
        }
        instantiatedItems.Clear();
    }
    public bool isWaveFinished()
    {
        if (instantiatedItems.Count <= 0 && itemsLeftToSpawn == 0) return true;
        return false;
    }
    public GameObject GetEnemyInstance(eEnemyType enemyType)
    {
        foreach (GameObject enemyGO in enemiesPoolDictionary[enemyType])
        {
            if (enemyGO.activeSelf)
            {
                continue;
            }

            AddItemToList(enemyGO);
            return enemyGO;
        }

        return null;
    }

    public void ReleaseEnemyInstance(GameObject goInstance, eEnemyType enemyType)
    {
        goInstance.SetActive(false);
        instantiatedItems.Remove(goInstance);
    }
   
    public GameObject GetPrefab(eEnemyType enemyType)
    {
        return enemyPrefabs[enemyType];
    }
}
    /*public List<GameObject>()
    {
        
    }*/
/*public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();
     
    [SerializeField]
    private List<TValue> values = new List<TValue>();
     
    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach(KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }
     
    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        this.Clear();
 
        if(keys.Count != values.Count)
            throw new System.Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));
 
        for(int i = 0; i < keys.Count; i++)
            this.Add(keys[i], values[i]);
    }
}*/






