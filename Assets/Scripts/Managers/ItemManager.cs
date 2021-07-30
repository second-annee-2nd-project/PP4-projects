using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum eItemType
{
    fares,
    yannis
}
// faire une classe au dessus et la faire dériver

public class ItemManager : MonoBehaviour
{
    [SerializeField] protected eTeam team = eTeam.neutral;
    public eTeam Team => team;

    [SerializeField] protected GameObject container;

    protected List<GameObject> instantiatedItems;

    public List<GameObject> InstantiatedItems => instantiatedItems;

    protected virtual void Start()
    {
        instantiatedItems = new List<GameObject>();
        if (container == null) Instantiate(new GameObject(name + " Container"));
    }
    
    public virtual void AddItemToList(GameObject item)
    {
        instantiatedItems.Add(item);
    }
    
    public virtual void RemoveItemFromList(GameObject item)
    {
        instantiatedItems.Remove(item);
    }

    public virtual void Restart()
    {
        GameObject newGO;
        while (instantiatedItems.Count > 0)
        {
            newGO = instantiatedItems.First();
            instantiatedItems.Remove(newGO);
            Destroy(newGO);
        }
    }

    

}
