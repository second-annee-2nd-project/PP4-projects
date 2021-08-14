using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    private static ShopManager instance;
    public static ShopManager Instance => instance;
    
    private int coins;

    public int Coins
    {
        get
        {
            return coins;
        }
        set
        {
            coins = value;
        }
    }

    private Coroutine cor;
    private Coroutine placingCor;

    [SerializeField] private int healPrice;
    public int HealPrice => healPrice;
    
    [SerializeField] private int bCoins;
    // Décaler ça dans wave manager
    
    [Header("UI")]
    [SerializeField] private GameObject UI_ShopSequence;
    [SerializeField] private Text coins_Text;
    [SerializeField] private Text headerShopSequence_Text;
    
    
    private GameObject equipedPrefabInstance;
    private Turret turretScript;

    private WaveManager waveManager;
    private CameraController cc;

    private bool isSkipShopButtonPressed;

    public GameObject EquipedPrefab
    {
        get => equipedPrefabInstance;
        set => equipedPrefabInstance = value;
    }
    private TurretManager turretManager;
    private PlayerBehaviour playerBehaviour;
    [SerializeField] private GameObject joystickController;
    private Turret newTurretScript;

    private void Awake()
    {
        Init();
        if (instance == null)
        {
            instance = this;
           // DontDestroyOnLoad(this);
        }
    }
    
    private void Init()
    {
        turretManager = GameObject.FindObjectOfType<TurretManager>().GetComponent<TurretManager>();
        playerBehaviour = GameObject.FindObjectOfType<PlayerBehaviour>();
        coins = bCoins;
        coins_Text.text = " : " + coins;
        // weaponUIList = new List<WeaponUI>(FindObjectsOfType<WeaponUI>());

    }


    // à mettre dans l'UI Manager
    public void UpdateCoins(int amount)
    {
        coins += amount;
        coins_Text.text = " : " + coins;
    }
    
    public void StartShopSequence()
    {
        cc = GameManager.Instance.CC;
        if (cor == null)
        {
            cor = StartCoroutine(ShopSequence());
        }
    }

    public void SkipButtonPressed()
    {
        isSkipShopButtonPressed = true;
    }

    private IEnumerator ShopSequence()
    {
        UI_ShopSequence.SetActive(true);
        GameManager.Instance.P_WeaponUI.PossibleToBuyWeapon();
        GameManager.Instance.P_TurretBtnUI.PossibleToBuyTurret();
        joystickController.SetActive(false);
        isSkipShopButtonPressed = false;
        // SetWeaponsBtn();
        
        while (!isSkipShopButtonPressed)
        {
            headerShopSequence_Text.text = "Phase de préparation, posez des tourelles\n";
            yield return null;
        }
        
        UI_ShopSequence.SetActive(false);
        joystickController.SetActive(true);
        if (placingCor != null)
        {
            StopCoroutine(placingCor);
            placingCor = null;
        }

        cor = null;

        
        if (turretScript != null)
        {
            RefundTurret();
        }
        yield return null;
        yield return new WaitForEndOfFrame();
        GameManager.Instance.ChangePhase(eGameState.Wave);
    }

    public void StartPlacingTurret(GameObject turretPrefab)
    {
      
        turretScript = turretPrefab.GetComponent<Turret>();
        if (placingCor ==  null && coins >= turretScript.SoTurret.Price)
        {
            equipedPrefabInstance = Instantiate(turretPrefab);
            placingCor = StartCoroutine(PlaceTurret());
        }
    }

    public void BuyWeapon(GameObject weaponGO)
    {
        
         Weapon wp = weaponGO.GetComponent<Weapon>();
       
        if (wp)
        {
            //même arme feedback
            if (wp.WeaponStats.WeaponType == playerBehaviour.P_Weapon.WeaponStats.WeaponType)
            {
                return;
            }
            if (coins >= wp.WeaponStats.Price)
            {
                
                GameObject newInstance = Instantiate(weaponGO);
                playerBehaviour.PickUpWeapon(newInstance);
                UpdateCoins(-wp.WeaponStats.Price);
                GameManager.Instance.P_WeaponUI.PossibleToBuyWeapon();
                GameManager.Instance.P_UpgradeWeapon.SetUpgradedWeaponInfo();
               
               
            }
            else
            {
                //introduce feedback here;
            }
             if (coins >= wp.UpgradeWeaponStats.Price)
             {
                 GameManager.Instance.P_UpgradeWeapon.UpgradeBtn.interactable = true;
             }
             else
             {
                 GameManager.Instance.P_UpgradeWeapon.UpgradeBtn.interactable = false;
             }
           
            
        }
        
        
        
    }

    private void RefundTurret()
    {
        //coins += turretScript.SoTurret.Price;
        Destroy(equipedPrefabInstance);
        equipedPrefabInstance = null;
        turretScript = null;
    }
    
    
    // Start is called before the first frame update
    private IEnumerator PlaceTurret()
    {
        bool isTurretPlaced = false;

         newTurretScript = equipedPrefabInstance.GetComponent<Turret>();
        newTurretScript.enabled = false;
        UI_ShopSequence.SetActive(false);
        while (!isTurretPlaced)
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f);
            Ray castPoint = cc.CurrentCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
            {
                Node n = GameManager.Instance.ActualGrid.GetNodeWithPosition(hit.point);
                    equipedPrefabInstance.transform.position = n.position;

                    if (CheckIfTurretable(n))
                {
                    if (Input.GetAxisRaw("Fire1") == 1f)
                    {
                       
                        newTurretScript.enabled = true;
                        GameManager.Instance.P_TurretManager.AddItemToList(equipedPrefabInstance);
                        UpdateCoins(turretScript.SoTurret.Price * -1);
                        newTurretScript.Deploy(n.position, n.internalPosition, Quaternion.identity);
                        GameManager.Instance.P_UiManager.FloatingTextInstantiate(new Vector3(newTurretScript.transform.position.x,newTurretScript.transform.position.y+1f,newTurretScript.transform.position.z),
                            GameManager.Instance.P_UiManager.CanvasContainerWorld,
                            GameManager.Instance.P_UiManager.FloatingTextPrefabWorld, 2f,turretScript.SoTurret.Price);
                        GameManager.Instance.P_TurretBtnUI.PossibleToBuyTurret();
                        equipedPrefabInstance = null;
                        isTurretPlaced = true;
                        break;
                    }
                }
            }

            yield return null;
        }
        UI_ShopSequence.SetActive(true);
        placingCor = null;
    }

    bool CheckIfTurretable(Node n)
    {
        return n.isTurretable;
    }

    
    public void Restart()
    {
        Init();
        if (placingCor != null)
        {
            StopCoroutine(placingCor);
        }
        placingCor = null;
    }
    
    // public void SetWeaponsBtn()
    // {
    //     List<GameObject> weapons =  GameManager.Instance.P_WeaponsManager.GetRandomWeapons(weaponUIList.Count);
    //     for (int i = 0; i < weapons.Count; i++)
    //     {
    //         weaponUIList[i].SetWeaponInfo(weapons[i]);
    //     }
    // }
    
}
