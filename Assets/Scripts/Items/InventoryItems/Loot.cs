using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    private int amountToLoot;
    private float rotatingSpeed = 20f;
    private float speed;
    private Transform player;
    private LootManager lootManager;
    private float a;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        
        a = 0;
    }

    private void Start()
    {
        lootManager = GameManager.Instance.P_LootManager;
        speed = lootManager.Speed;
    }

    private void Update()
    {
        transform.Rotate(transform.up  * rotatingSpeed * Time.deltaTime);
    }

    public int AmountToLoot
    {
        get => amountToLoot;
        set => amountToLoot = value;
    }

    public void Looted(PlayerBehaviour playerBehaviour)
    {
        playerBehaviour.Loot(amountToLoot);
        lootManager.RemoveItemFromList(gameObject);
        Destroy(gameObject);
    }

    private IEnumerator AutoLoot()
    {
        while(transform.position != player.transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position,player.position , speed * Time.deltaTime);
            yield return null;
        }
       
    }

    public void StartAutoLoot()
    {
        StartCoroutine(AutoLoot());
    }
}
