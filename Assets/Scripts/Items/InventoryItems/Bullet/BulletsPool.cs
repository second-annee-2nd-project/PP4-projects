using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eBulletType
{
    BulletYannis,
    TurretBulletFares,
    MortarBullet,
    EnemyBullet
}
public class BulletsPool : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab = default;
    [SerializeField] private GameObject turretBulletPrefab = default;
    [SerializeField] private GameObject mortarBulletPrefab = default;
    [SerializeField] private GameObject enemyBulletPrefab = default;
    [SerializeField] private int poolSize = 50;

    private GameObject[] bulletInstances;
    private GameObject[] turretBulletInstances;
    private GameObject[] enemyBulletInstances;
    private GameObject[] mortarBulletInstances;
    
    //permet de récupérer toutes les instances 
    private Dictionary<eBulletType, int> bulletRow;
    private GameObject availableBulletInstance;

    private void Awake()
    {
        InitPool();
    }

    private void InitPool()
    {
        Transform tr = transform;

        bulletInstances = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            for (int j = 0; j < poolSize; j++)
            {
                GameObject newBullet = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
                newBullet.transform.parent = tr;
                newBullet.GetComponent<Bullet>().P_BulletsPool = this;
                newBullet.SetActive(false);
                bulletInstances[j] = newBullet;
            }
        }
        
        turretBulletInstances = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            for (int j = 0; j < poolSize; j++)
            {
                GameObject newBullet = Instantiate(turretBulletPrefab, Vector3.zero, Quaternion.identity);
                newBullet.transform.parent = tr;
                newBullet.GetComponent<Bullet>().P_BulletsPool = this;
                newBullet.SetActive(false);
                turretBulletInstances[j] = newBullet;
            }
        }
        mortarBulletInstances = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            for (int j = 0; j < poolSize; j++)
            {
                GameObject newBullet = Instantiate(mortarBulletPrefab, Vector3.zero, Quaternion.identity);
                newBullet.transform.parent = tr;
                newBullet.GetComponent<Bullet>().P_BulletsPool = this;
                newBullet.SetActive(false);
                mortarBulletInstances[j] = newBullet;
            }
        }
        enemyBulletInstances = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            for (int j = 0; j < poolSize; j++)
            {
                GameObject newBullet = Instantiate(enemyBulletPrefab, Vector3.zero, Quaternion.identity);
                newBullet.transform.parent = tr;
                newBullet.GetComponent<Bullet>().P_BulletsPool = this;
                newBullet.SetActive(false);
                enemyBulletInstances[j] = newBullet;
            }
        }

        availableBulletInstance = bulletInstances[0];
    }

    public GameObject GetNextBulletInstance(eBulletType bulletType)
    {
        switch (bulletType)
        {
            case eBulletType.BulletYannis:
                for (int i = 0; i < poolSize; i++)
                {
                    if (bulletInstances[i].activeSelf)
                    {
                        continue;
                    }
                    return bulletInstances[i];
                }

                break;
            case eBulletType.EnemyBullet:
                for (int i = 0; i < poolSize; i++)
                {
                    if (enemyBulletInstances[i].activeSelf)
                    {
                        continue;
                    }
                    return enemyBulletInstances[i];
                }
                break;
            case eBulletType.TurretBulletFares:
                int count = poolSize;
                for (int i = 0; i < poolSize; i++)
                {
                    if (turretBulletInstances[i].activeSelf)
                    {
                        count--;
                    }
                }
                //Debug.Log("Nombre de balles dispo : "+count);

                for (int i = 0; i < poolSize; i++)
                {
                    if (turretBulletInstances[i].activeSelf)
                    {
                        continue;
                    }
                    return turretBulletInstances[i];
                }
                break;
            case eBulletType.MortarBullet:
                int countMortar = poolSize;
                for (int i = 0; i < poolSize; i++)
                {
                    if (mortarBulletInstances[i].activeSelf)
                    {
                        countMortar--;
                    }
                }
                //Debug.Log("Nombre de balles dispo : "+count);

                for (int i = 0; i < poolSize; i++)
                {
                    if (mortarBulletInstances[i].activeSelf)
                    {
                        continue;
                    }
                    return mortarBulletInstances[i];
                }
                break;

        }
        
        
        return null;
    }

    public void ReleaseBulletInstance(GameObject bulletInstance, eBulletType bulletType)
    {
        switch (bulletType)
        {
            case eBulletType.BulletYannis:
                foreach (GameObject b in bulletInstances)
                {
                    if (bulletInstance != b)
                        continue;

                    b.SetActive(false);
                    break;
                }
                break;
            case eBulletType.EnemyBullet:
                foreach (GameObject b in enemyBulletInstances)
                {
                    if (bulletInstance != b)
                        continue;

                    b.SetActive(false);
                    break;
                }
                break;
            case eBulletType.MortarBullet:
                foreach (GameObject b in mortarBulletInstances)
                {
                    if (bulletInstance != b)
                        continue;

                    b.SetActive(false);
                    break;
                }
                break;
            case eBulletType.TurretBulletFares:
                foreach (GameObject b in turretBulletInstances)
                {
                    if (bulletInstance != b)
                        continue;

                    b.SetActive(false);
                    break;
                }
                break;

        }
        
    }
}
