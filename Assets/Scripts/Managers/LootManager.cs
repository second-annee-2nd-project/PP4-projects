using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : ItemManager
{
  [SerializeField] private float speed;
  public float Speed => speed;
  public void StartAutoLootSequence()
  {
    StartCoroutine(nameof(AutoLootSequence));
  }

  IEnumerator AutoLootSequence()
  {
    foreach (var item in instantiatedItems)
    {
      item.GetComponent<Loot>().StartAutoLoot();
    }
    while (InstantiatedItems.Count > 0)
    {
      yield return null;
    }
    GameManager.Instance.ChangePhase(eGameState.Shop);
  }
}
