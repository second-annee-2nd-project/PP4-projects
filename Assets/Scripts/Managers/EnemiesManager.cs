using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public enum eEnemyType
{
    classic,
    assault
}

public class EnemiesManager : UnitManager
{
    [SerializeField] private EnemyDictionary enemyPrefabs;

    public EnemyDictionary EnemyPrefabs => enemyPrefabs;
    // Properties utilisé dans chaque Spawner
    private Coroutine cor;
    
    

    protected override void Start()
    {
        base.Start();
        team = eTeam.enemy;

    }

    public bool isWaveFinished()
    {
        if (instantiatedItems.Count <= 0 && itemsLeftToSpawn == 0) return true;
        return false;
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






