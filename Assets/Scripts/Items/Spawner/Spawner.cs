using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Vector3 objectSize;


    public GameObject Spawn(GameObject prefab)
    {
        int x = (int) objectSize.x;
        int y = (int) objectSize.y;
        int z = (int) objectSize.z;

        float rx = transform.position.x + Random.Range(0f, x/2f);
        float rz = transform.position.z + Random.Range(0f, z / 2f);
        
        GameObject enemy = Instantiate(prefab,
            new Vector3(rx, 0, rz), Quaternion.identity);
        
        return enemy;
    }
}
