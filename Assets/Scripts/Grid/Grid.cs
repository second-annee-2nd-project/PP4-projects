using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private Node[,] nodes;

    public Node[,] Nodes
    {
        get => Nodes = nodes;
        set => nodes = value;
    }

    [SerializeField] private int gridWidth;
    [SerializeField] private int gridLength;
    [SerializeField] private int gridHeight;
    public int P_GridWidth => gridWidth;
    public int P_GridLength => gridLength;

    [SerializeField] private float gridSizeWidth;
    [SerializeField] private float gridSizeLength;
    [SerializeField] private float gridSizeHeight;

    [SerializeField] private Vector3 centerPosition;
    public Vector3 CenterPosition => centerPosition;
    public LayerMask enemyMask;
    public LayerMask unwalkableMask;

    private GameObject ground;

    void Awake()
    {
        nodes = new Node[gridWidth, gridLength];
        unwalkableMask = LayerMask.GetMask("Unwalkable");
        Instantiate();
    }

    public void Instantiate()
    {
        Vector3 positionOffset = new Vector3((gridWidth * gridSizeWidth) / 2f - gridSizeWidth/ 2f, 0f, (gridLength * gridSizeLength) / 2f - gridSizeLength/ 2f);
        Debug.Log(positionOffset);
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridLength; j++)
            {
                float newPosX = i * gridSizeWidth - positionOffset.x;
                float newPosZ = j * gridSizeLength - positionOffset.z;
                Vector3 position = centerPosition + new Vector3(newPosX, 0, newPosZ);
                float radius = Mathf.Sqrt(gridSizeWidth * gridSizeWidth + gridSizeLength * gridSizeLength);
                bool isWalkable = true;
                bool isTurretable = true;

                Collider[] hitColliders = new Collider[10];
                int size = Physics.OverlapSphereNonAlloc(position, radius / 2f - 0.3f, hitColliders, unwalkableMask.value);

                if (size > 0)
                {
                    isWalkable = false;
                    isTurretable = false;
                }

                Vector3 newInternalPosition = new Vector3(Mathf.Abs(position.x), Mathf.Abs(position.y), Mathf.Abs(position.z));
                Vector3 internalPosition = new Vector3(i, 0, j);
                nodes[i, j] = new Node(position, internalPosition, radius / 2f, isWalkable, isTurretable);
            }
        }
    }
    
    public Node GetNode(Vector3 pos)
    {
        float totalWidth = gridWidth / gridSizeWidth;
        float totalLength = gridLength / gridSizeLength;
        int realI = 0;
        int realJ = 0;
        for (int i = 0; i < gridWidth; i++)
        {
            realJ = 0;
            for (int j = 0; j < gridLength; j++)
            {
                //
                Vector3 nodePos = nodes[realI, realJ].position;
                if (pos.x >= nodePos.x  && pos.x <= nodePos.x + gridSizeWidth && pos.z >= nodePos.z && pos.z <= nodePos.z + gridSizeLength)
                {
                    return nodes[realI, realJ];
                }

                realJ++;
            }

            realI++;
        }

        return nodes[0, 0];
    }
    
    public Node TestWorldToScreenPoint(Vector3 _position)
    {
        int x = 0;
        int z = 0;
        float distance = 10000000000f;

        //return GetNode(_position);
        
        for (int i = 0; i < nodes.GetLength(0); i++)
        {
            for (int j = 0; j < nodes.GetLength(1); j++)
            {
                Vector2 a = new Vector3(Camera.main.WorldToScreenPoint(_position).x,0f ,Camera.main.WorldToScreenPoint(_position).z);
                Vector2 b = new Vector3(Camera.main.WorldToScreenPoint(nodes[i, j].position).x,0f ,Camera.main.WorldToScreenPoint(nodes[i, j].position).z);
                
                float d = Vector3.Distance(a, b);
                if (d < distance)
                {
                  //  Debug.Log("Smallest distance is now : "+d +" from "+distance);
                    distance = d;
                    x = i;
                    z = j;
                 //   Debug.Log("Found smaller distance on "+x+", "+z+"]");
                    
                }
            }
        }

        Debug.Log("Position de l'objet sélectionné : " + _position);
        Debug.Log("Position de la node : " + nodes[x, z].position);
        Debug.Log("Coordonnées : ["+x+", "+z+"]");
        return nodes[x, z];
    }
    
    public Node GetNodeWithPosition(Vector3 _position)
    {
        int x = 0;
        int z = 0;
        float distance = 10000000000f;
        
        for (int i = 0; i < nodes.GetLength(0); i++)
        {
            for (int j = 0; j < nodes.GetLength(1); j++)
            {
                float d = Vector3.Distance(_position, nodes[i, j].position);
                if (d < distance)
                {
                    distance = d;
                    x = i;
                    z = j;
                }
            }
        }

        return nodes[x, z];
    }
    
    void OnDrawGizmos()
    {
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridLength; j++)
            {
                if (nodes!= null)
                {
                    Gizmos.color = nodes[i, j].isWalkable ? Color.green : Color.red;

                    Gizmos.DrawWireCube(nodes[i, j].position, new Vector3(gridSizeWidth, gridSizeHeight, gridSizeLength));
                }
            }
        }
    }
}
