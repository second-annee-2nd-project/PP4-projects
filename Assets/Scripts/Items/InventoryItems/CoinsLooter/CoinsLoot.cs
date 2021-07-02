using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsLoot : MonoBehaviour
{
    [SerializeField] private GameObject coinsPrefab;
    public void LootCoins(Vector3 pos)
    {
        Instantiate(coinsPrefab,pos, Quaternion.identity);
    }
    
}
