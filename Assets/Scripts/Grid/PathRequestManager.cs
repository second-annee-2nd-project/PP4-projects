using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    public Dictionary<EnemyBehaviour, PathRequest> pathRequests;
    
    public Node current;
    public Node startingNode;
    public Node targetNode;
    
    public Node[,] nodes;
   /* public List<Node> neighbours;
    public List<Node> visited;
    public List<Node> taken;*/
    
    /*public Dictionary<Node, int> costs;
    public Dictionary<Node, Node> cameFrom;
    public Node first;
    public List<PriorityQueue> boundaries;*/

    //public List<Node> path;
    private int j;
    private Coroutine cor;

    public void Init()
    {
        pathRequests = new Dictionary<EnemyBehaviour, PathRequest>();
        nodes = FindObjectOfType<Grid>().Nodes;
        j = 0;
    }

    public void AddPath(PathRequest newPathRequest, EnemyBehaviour enemy)
    {
        if (pathRequests.ContainsKey(enemy))
        {
            pathRequests.Remove(enemy);
        }
        pathRequests.Add(enemy, newPathRequest);
        if(cor != null) return;
        cor = StartCoroutine(Instantiate());
    }

    public IEnumerator Instantiate()
    {
        while (pathRequests.Count > 0)
        {
            PathFinder();
            yield return null;
        }

        cor = null;
    }

    /*public int[,] GetNodeWithPosition(Vector3 position)
    {
        
    }*/
    
    int GetDistance(Vector3 pos, Vector3 targetPos)
    {
        int distance = (int) (Mathf.Abs(pos.x - targetPos.x) + Mathf.Abs(pos.z - targetPos.z));
        return distance;
    }
    
    public void PathFinder()
    {
        
        List<Node> visited = new List<Node>();
        List<Node> taken = new List<Node>();
        List<Node> neighbours = new List<Node>();
        
        List<PriorityQueue> boundaries = new List<PriorityQueue>();
        Dictionary<Node, int> costs = new Dictionary<Node, int>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        List<Node> path = new List<Node>();
        
        targetNode = pathRequests.First().Value.requestedFrom.TargetingNode;
        startingNode = pathRequests.First().Value.requestedFrom.StartingNode;
        EnemyBehaviour eb = pathRequests.First().Value.requestedFrom;
        
        boundaries.Add(new PriorityQueue(startingNode, 0));
        current = boundaries[0].node;
        Node first = current;
        
        costs.Add(current, 0);
        cameFrom[current] = null;
        visited.Add(current);

        while (boundaries != null && boundaries.Count > 0 && pathRequests.First().Key != null)
        {
            Debug.Log(pathRequests.First().Key.name+" demande un path.");
            current = GetFirst(boundaries);
            if (current == null) break;

            if(current == targetNode)
            {
                break;
            }
            
            neighbours = GetNeighbours(current);
            
            for (int i = 0; i < neighbours.Count; i++)
            {
                int distance = GetDistance(targetNode.internalPosition, neighbours[i].internalPosition);
                int newCost = costs[current] + 1;

                if(!costs.ContainsKey(neighbours[i]) || newCost < costs[neighbours[i]])
                {
                    
                    // 0 1 2
                    // 3 x 4
                    // 5 6 7
                    // diagonales
                    if (i == 0 || i == 2 || i == 5 || i == 7)
                    {
                        switch (i)
                        {
                            case 0:
                                //Calculer en fonction de 1 et 3
                                if (neighbours.Count > 1 && neighbours.Count > 3)
                                {
                                    // S'il n'y a pas de mur alors c'est bon
                                    if (neighbours[1].isWalkable && neighbours[3].isWalkable)
                                    {
                                        
                                    }
                                }
                                break;
                            
                            case 2:
                                //Calculer en fonction de 1 et 4
                                if (neighbours.Count > 1 && neighbours.Count > 4)
                                {
                                    // S'il n'y a pas de mur alors c'est bon
                                    if (neighbours[1].isWalkable && neighbours[4].isWalkable)
                                    {
                                        
                                    }
                                }
                                break;
                            
                            case 5:
                                //Calculer en fonction de 3 et 6
                                if (neighbours.Count > 3 && neighbours.Count > 6)
                                {
                                    // S'il n'y a pas de mur alors c'est bon
                                    if (neighbours[3].isWalkable && neighbours[6].isWalkable)
                                    {
                                        
                                    }
                                }
                                break;
                            case 7:
                                //Calculer en fonction de 4 et 6
                                if (neighbours.Count > 4 && neighbours.Count > 6)
                                {
                                    // S'il n'y a pas de mur alors c'est bon
                                    if (neighbours[4].isWalkable && neighbours[6].isWalkable)
                                    {
                                        
                                    }
                                }
                                break;
                        }
                    }
                    if(neighbours[i].isWalkable)
                    {
                        if (!costs.ContainsKey(neighbours[i]))
                        {
                                costs.Add(neighbours[i], newCost);
                                cameFrom[neighbours[i]] = current;
                        }
                        else
                        {
                                if (costs[neighbours[i]] > newCost)
                                {
                                    costs[neighbours[i]] = newCost;
                                    cameFrom[neighbours[i]] = current;
                                }
                        }
                        int prio = newCost + distance;
                        boundaries.Add(new PriorityQueue(neighbours[i], prio));
                    }
                }
            }
            boundaries.RemoveAt(0);
        }
        pathRequests.First().Value.path = RetracePath(cameFrom, first);
        pathRequests.First().Value.ReturnPath();
        pathRequests.Remove(pathRequests.First().Key);
    }

    public List<Node> RetracePath(Dictionary<Node, Node> cf, Node f)
    {
        List<Node> newPath = new List<Node>();
        if (cf.ContainsKey(targetNode))
        {
            newPath.Add(targetNode);
            Node nodeToTake = cf[targetNode];
            newPath.Add(nodeToTake);
            while (nodeToTake != f)
            {
                nodeToTake = cf[nodeToTake];

                newPath.Add(nodeToTake);
            }
        }

        newPath.Reverse();
        return newPath;
    }

    public Node GetFirst(List<PriorityQueue> priorityQueuesList)
    {
        bool sorted = false;
        while (sorted != true)
        {
            int count = 0;
            for (int i = 0; i < priorityQueuesList.Count-1; i++)
            {
                if (priorityQueuesList[i].priority > priorityQueuesList[i + 1].priority)
                {
                    PriorityQueue temp = priorityQueuesList[i];
                    priorityQueuesList[i] = priorityQueuesList[i + 1];
                    priorityQueuesList[i + 1] = temp;
                    count++;
                }
            }

            if (count == 0)
                sorted = true;
        }

        return priorityQueuesList?[0]?.node;
    }
    
    List<Node> GetNeighbours(Node node)
    {
//        Debug.Log("NodeX : "+node.position.x);
        List<Node> n = new List<Node>();

        int xOffset = -1; 
        int zOffset = -1;
        for (int z = 0; z < 3; z++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (x + xOffset == 0 && z + zOffset == 0)
                {
                    x++;
                }
                //Debug.Log("Nombre de neighbours : "+(xOffset + x + node.internalPosition.x));

                if ((int) ( xOffset + x + node.internalPosition.x) >= 0 &&
                    (int) ( xOffset + x + node.internalPosition.x) < nodes.GetLength(0))
                {
                    if ((int) ( zOffset + z + node.internalPosition.z) >= 0 &&
                        (int) ( zOffset + z + node.internalPosition.z) < nodes.GetLength(1))
                    {
                        n.Add(nodes[(int) (xOffset + x + node.internalPosition.x), (int) (z + zOffset + node.internalPosition.z)]);
                    }
                }
            }
        }
        
        return n;
        
    }
}
