using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eBulletType
{
    BulletYannis,
    TurretBulletFares
}
public class BulletsPool : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab = default;
    [SerializeField] private GameObject turretBulletPrefab = default;
    [SerializeField] private int poolSize = 50;

    private GameObject[] bulletInstances;
    private GameObject[] turretBulletInstances;
    
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
            case eBulletType.TurretBulletFares:
                for (int i = 0; i < poolSize; i++)
                {
                    if (turretBulletInstances[i].activeSelf)
                    {
                        continue;
                    }
                    return turretBulletInstances[i];
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
