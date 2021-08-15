using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera shopCamera;

    private Vector3 playerCameraPos;
    private Vector3 shopCameraPos;
    

    private Vector3 baseOffset;
    private float cameraSpeed;
    [SerializeField] private float topOffset;
    [SerializeField] private float rightOffset;
    [SerializeField] private float botOffset;
    [SerializeField] private float leftOffset;

    private Camera currentCamera;
    public Camera CurrentCamera => currentCamera;
    
    
    [SerializeField] private bool debugEnabled = true;
    private List<GameObject> walls;

    private Transform minX;
    private Transform maxX;
    private Transform minZ;
    private Transform maxZ;

    private Transform playerTr;

    void Awake()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
       // playerCameraPos = new Vector3((playerTr.position + playerCamera.transform.position).x, playerCamera.transform.position.y,
      //      (playerTr.position + playerCamera.transform.position).z);
        playerCameraPos = playerCamera.transform.position;
        shopCameraPos = shopCamera.transform.localPosition;
    }
    void Start()
    {
        
    }

    void SetCameraLocalPos()
    {
        playerCamera.transform.localPosition = playerCameraPos;
        shopCamera.transform.localPosition = shopCameraPos;
    }

    public void Init()
    {
        GetWallsData();
        
        playerCamera.transform.localPosition = playerCameraPos;
        
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        cameraSpeed = playerTr.GetComponent<PlayerBehaviour>().Speed;
        baseOffset =  playerCamera.transform.position - playerTr.position;

        if (GameManager.Instance.EGameState == eGameState.Shop)
        {
            playerCamera.enabled = false;
            currentCamera = shopCamera;
        }

        
    }

    private void GetWallsData()
    {
        walls = new List<GameObject>(GameObject.FindGameObjectsWithTag("Boundaries"));
        if (walls.Count < 1) return;
        
        Vector3 center = GameManager.Instance.ActualGrid.CenterPosition;
        
        minX = walls[0].transform;
        maxX = walls[0].transform;
        minZ = walls[0].transform; 
        maxZ = walls[0].transform;

       float minusX =  minX.position.x - center.x;
       float plusX = maxX.position.x - center.x;
       float minusZ = minZ.position.z - center.z;
       float plusZ = maxZ.position.z - center.z;

       for (int i = 1; i < walls.Count; i++)
        {
            float newMinusX = walls[i].transform.position.x - center.x;
            float newPlusX = walls[i].transform.position.x - center.x;
            float newMinusZ = walls[i].transform.position.z - center.z;
            float newPlusZ = walls[i].transform.position.z - center.z;

            if (newMinusX <= minusX)
            {
                if (debugEnabled)
                {
                    Debug.Log("Old Minus X : " + minusX);
                    Debug.Log("New Minus X : " + newMinusX);
                    Debug.Log("-+-+-+-+-+-+-+-+-+-+-+-+-+-");
                }

                minusX = newMinusX;
                minX = walls[i].transform;
            }
            if(newMinusZ <= minusZ)
            {
                minusZ = newMinusZ;
                minZ = walls[i].transform;
            }
            if (newPlusX >= plusX)
            {
                plusX = newPlusX;
                maxX = walls[i].transform;
            }
            if(newPlusZ >= plusZ)
            {
                plusZ = newPlusZ;
                maxZ = walls[i].transform;
            }
        }

        if (debugEnabled)
        {
            minX.gameObject.GetComponent<Renderer>().material.color = Color.black;
            maxX.gameObject.GetComponent<Renderer>().material.color = Color.green;
            minZ.gameObject.GetComponent<Renderer>().material.color = Color.blue;
            maxZ.gameObject.GetComponent<Renderer>().material.color = Color.red;
            
            Debug.Log("minX, noir : " + minX.position);
            Debug.Log("maxX, vert : " + maxX.position);
            Debug.Log("minZ, bleu : " + minZ.position);
            Debug.Log("maxZ, rouge : " + maxZ.position);
            
            
        }
        
        
    }

    public void ChangeState(eGameState newState)
    {
        if (GameManager.Instance.EGameState == eGameState.Shop)
        {
            playerCamera.enabled = false;
            shopCamera.enabled = true;
            currentCamera = shopCamera;
        }
        else
        {
            playerCamera.enabled = true;
            shopCamera.enabled = false;
            currentCamera = playerCamera;
        }
    }
    
    void LateUpdate()
    {
        // calcul la taille de la base basse du tronc en fonction des paramètres de caméra et de la hauteur entre la caméra et la grid
        /*float height = Mathf.Abs(GameManager.Instance.ActualGrid.CenterPosition.y - camPos.y);
        float frustrumHalfHeight = height * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustrumHalfWidth = frustrumHalfHeight * cam.aspect;*/

        float frustrumHeight = GameManager.Instance.ActualGrid.P_GridWidth;
        //float frustrumHalfWidth = );
        float height = frustrumHeight * 0.5f / Mathf.Tan(shopCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float width = height * currentCamera.aspect;
        float cameraview = GameManager.Instance.ActualGrid.P_GridWidth * Mathf.Tan(shopCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        
        float newPosX = 0f;
        float newPosZ = 0f;
        //GameManager.Instance.ActualGrid.P_GridWidth
        Vector3 posClamped;
        Vector3 posModified;
        
        if (GameManager.Instance.EGameState == eGameState.Shop)
        {
            posModified = new Vector3(GameManager.Instance.ActualGrid.CenterPosition.x, GameManager.Instance.ActualGrid.P_GridWidth/2f + Mathf.PI,
                GameManager.Instance.ActualGrid.CenterPosition.z - GameManager.Instance.ActualGrid.P_GridLength/2f);
            shopCamera.transform.position = Vector3.MoveTowards(shopCamera.transform.position, posModified, 200f * Time.deltaTime);
        }
        else
        {
            posModified = playerTr.position ;

            float a = GameManager.Instance.ActualGrid.CenterPosition.z + height * 0.5f;
            float b = GameManager.Instance.ActualGrid.CenterPosition.x + width * 0.5f;
            
            if (playerTr.position.z >= maxZ.position.z - topOffset)
            {
                newPosZ = maxZ.position.z - topOffset;
                posModified = new Vector3(posModified.x, posModified.y, newPosZ);
                
                if(debugEnabled)
                    Debug.Log("Touché en haut");
            }
            if (playerTr.position.x >= maxX.position.x - rightOffset)
            {
                newPosX = maxX.position.x - rightOffset;
                posModified = new Vector3(newPosX, posModified.y, posModified.z);

                if(debugEnabled)
                    Debug.Log("Touché à droite");
            }
            if (playerTr.position.z  <= minZ.position.z + botOffset)
            {
                newPosZ = minZ.position.z + botOffset;
                posModified = new Vector3(posModified.x, posModified.y, newPosZ);
                
                if(debugEnabled)
                    Debug.Log("Touché en bas");
            }
            if (playerTr.position.x <= minX.position.x + leftOffset)
            {
                newPosX = minX.position.x + leftOffset;
                posModified = new Vector3(newPosX, posModified.y, posModified.z);
                
                if(debugEnabled)
                    Debug.Log("Touché à gauche");
            }
            
            posModified += baseOffset;
            
            playerCamera.transform.position = Vector3.MoveTowards(playerCamera.transform.position, posModified, 200f * Time.deltaTime);
        }
    }
}
