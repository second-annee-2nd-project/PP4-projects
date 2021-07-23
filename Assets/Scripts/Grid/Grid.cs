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
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridLength; j++)
            {
                float newPosX = i * gridSizeWidth - positionOffset.x;
                float newPosZ = j * gridSizeLength - positionOffset.z;
                Vector3 position = centerPosition + new Vector3(newPosX, 0, newPosZ);
                // Diamètre du cercle circonscrit
                //float diameter = Mathf.Sqrt(gridSizeWidth * gridSizeWidth + gridSizeLength * gridSizeLength);
                //Diamètre du cercle inscrit
                float diameter = gridSizeWidth;
                bool isWalkable = true;
                bool isTurretable = true;

                Collider[] hitColliders = new Collider[10];
                
                // On enlève 0.1 pour ne pas toucher les bords
                    int size = Physics.OverlapSphereNonAlloc(position, (diameter - 0.1f)/2f, hitColliders, unwalkableMask.value);

                if (size > 0)
                {
                    isWalkable = false;
                    isTurretable = false;
                }

                Vector3 newInternalPosition = new Vector3(Mathf.Abs(position.x), Mathf.Abs(position.y), Mathf.Abs(position.z));
                Vector3 internalPosition = new Vector3(i, 0, j);
                nodes[i, j] = new Node(position, internalPosition, diameter/2f, isWalkable, isTurretable);
            }
        }
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
                if (d < distance && nodes[i, j].isWalkable)
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
        float d = gridSizeWidth;
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridLength; j++)
            {
                if (nodes!= null)
                {
                    Gizmos.color = nodes[i, j].isWalkable ? Color.green : Color.red;
                    Gizmos.DrawWireSphere(nodes[i, j].position, gridSizeWidth/2f);
                    //Gizmos.DrawWireCube(nodes[i, j].position, new Vector3(gridSizeWidth, gridSizeHeight, gridSizeLength));
                }
            }
        }
    }
}
