using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    private int amountToLoot;

    public int AmountToLoot
    {
        get => amountToLoot;
        set => amountToLoot = value;
    }

    public void Looted(PlayerBehaviour playerBehaviour)
    {
        playerBehaviour.Loot(amountToLoot);
        Destroy(gameObject);
    }
}
