using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eWeaponType
{
    Pistol,
    Mitraillette,
    Mitrailleuse,
    Shotgun,
    Bfg,
    Assault,
    MachineGun,
    FlameThrower,
    Mortar
}


[CreateAssetMenu(fileName ="Weapon", menuName = "ScriptableObjects/Weapon", order = 1)]
public class SO_Weapon : ScriptableObject
{
    [SerializeField] private string name;
    public string Name => name;
    [SerializeField] private eWeaponType weaponType;
    public eWeaponType WeaponType => weaponType;
    [SerializeField] private float range;
    public float Range => range;

    [SerializeField] private float damage;
    public float Damage => damage;

    [SerializeField] private float fireRate;
    public float FireRate => fireRate;

    [SerializeField] private int maxAmmo;
    public int MaxAmmo => maxAmmo;

    [SerializeField] private int numberOfBulletsPerShot;
    public int NumberOfBulletsPerShot => numberOfBulletsPerShot;

    [SerializeField] [Range(0f, 180f)] private float diffusionAngle;
    public float DiffusionAngle => diffusionAngle;

    [SerializeField] private int price;
    public int Price => price;

    [SerializeField] private AudioClip weaponSound;
    public AudioClip WeaponSound => weaponSound;

    [SerializeField] private GameObject muzzleFlash;
    public GameObject MuzzleFlash => muzzleFlash;
    
    [SerializeField] private Material upgradedTexture;
    public Material UpgradedTexture => upgradedTexture;
}
