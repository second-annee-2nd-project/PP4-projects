using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
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
    
    

    [SerializeField] private int bCoins;
    [SerializeField] private float bShopTimer;
    private float shopTimer;
    // Décaler ça dans wave manager
    
    [Header("UI")]
    [SerializeField] private GameObject UI_ShopSequence;
    [SerializeField] private Text coins_Text;
    [SerializeField] private Text headerShopSequence_Text;
    
    
    private GameObject equipedPrefab;
    private GameObject newTurret;
    private Turret turretScript;

    private WaveManager waveManager;
    private CameraController cc;

    public GameObject EquipedPrefab
    {
        get => equipedPrefab;
        set => equipedPrefab = value;
    }
    private TurretManager turretManager;

    private void Awake()
    {
        turretManager = GameObject.FindObjectOfType<TurretManager>().GetComponent<TurretManager>();
        coins = bCoins;
        coins_Text.text = " : " + coins;
        placingCor = null;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
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

    private IEnumerator ShopSequence()
    {
        UI_ShopSequence.SetActive(true);
        shopTimer = bShopTimer;
        
        while (shopTimer > 0f)
        {
            shopTimer -= Time.deltaTime;
            headerShopSequence_Text.text = "Phase de préparation, posez des tourelles\n"+(int)shopTimer;
            yield return null;
        }
        
        UI_ShopSequence.SetActive(false);

        if (placingCor != null)
            StopCoroutine(placingCor);
        cor = null;

        
        if (turretScript != null)
        {
            Debug.Log("passe muraille");
            RefundTurret();
        }
        yield return null;
        yield return new WaitForEndOfFrame();
        GameManager.Instance.ChangePhase(eGameState.Wave);
    }

    public void StartPlacingTurret(GameObject turretPrefab)
    {
        newTurret = Instantiate(turretPrefab);
        equipedPrefab = newTurret;
        turretScript = equipedPrefab.GetComponent<Turret>();
        Debug.Log("passe temps");
        if (placingCor ==  null && coins >= turretScript.SoTurret.Price)
        {
            placingCor = StartCoroutine(PlaceTurret());
        }
    }

    private void RefundTurret()
    {
        //coins += turretScript.SoTurret.Price;
        Destroy(equipedPrefab);
        equipedPrefab = null;
        turretScript = null;
    }
    
    
    // Start is called before the first frame update
    private IEnumerator PlaceTurret()
    {
        bool isTurretPlaced = false;
        MeshRenderer selected = null;
        Material oldMat = null;
        Color oldColor = new Color();
        
        
        Turret newTurretScript = newTurret.GetComponent<Turret>();
        newTurretScript.enabled = false;
        
        while (!isTurretPlaced)
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f);
            Ray castPoint = cc.CurrentCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
            {
                Node n = GameManager.Instance.ActualGrid.GetNodeWithPosition(hit.point);
                    newTurret.transform.position = n.position;

                    if (CheckIfTurretable(n))
                {
                    if (Input.GetAxisRaw("Fire1") == 1f)
                    {
                        newTurretScript.enabled = true;
                        
                        GameManager.Instance.P_TurretManager.AddItemToList(equipedPrefab);
                        UpdateCoins(turretScript.SoTurret.Price * -1);
                        
                        n.isWalkable = false;
                        n.isTurretable = false;
                        
                        equipedPrefab = null;
                        break;
                    }
                }
            }

            yield return null;
        }

        placingCor = null;
    }

    bool CheckIfTurretable(Node n)
    {
        return n.isTurretable;
    }
    
}
