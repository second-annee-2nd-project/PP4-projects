using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    
    private float camHalfWidth;
    private float camHalfHeight;

    private Vector3 baseOffset;

    [SerializeField] private float cameraOffset;
    private float cameraSpeed;
    [SerializeField] private float topOffset;
    [SerializeField] private float rightOffset;
    [SerializeField] private float botOffset;
    [SerializeField] private float leftOffset;
    
    
    [SerializeField] private bool debugEnabled = true;
    private List<GameObject> walls;

    private Transform minX;
    private Transform maxX;
    private Transform minZ;
    private Transform maxZ;

    private Transform playerTr;

    public void Init()
    {
        GetWallsData();
        cam = Camera.main;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        cameraSpeed = playerTr.GetComponent<PlayerBehaviour>().Speed;
        baseOffset = cam.transform.position - playerTr.position;
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
    
    void LateUpdate()
    {
        
        // calcul la taille de la base basse du tronc en fonction des paramètres de caméra et de la hauteur entre la caméra et la grid
        /*float height = Mathf.Abs(GameManager.Instance.ActualGrid.CenterPosition.y - camPos.y);
        float frustrumHalfHeight = height * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustrumHalfWidth = frustrumHalfHeight * cam.aspect;*/
        
        float newPosX = 0f;
        float newPosZ = 0f;
        
        Vector3 posClamped;
        Vector3 posModified = playerTr.position;
        
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
        cam.transform.position = Vector3.MoveTowards(cam.transform.position, posModified, 200f * Time.deltaTime);
    }
}
