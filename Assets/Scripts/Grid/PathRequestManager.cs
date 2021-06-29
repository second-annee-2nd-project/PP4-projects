using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    public List<PathRequest> pathRequests;

    public Vector3 target;
    public Vector3 startingPoint;
    
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
    public int j;
    private Coroutine cor;

    void Awake()
    {
        pathRequests = new List<PathRequest>();
        nodes = FindObjectOfType<Grid>().Nodes;
        j = 0;
    }

    public void AddPath(PathRequest newPathRequest)
    {
        pathRequests.Add(newPathRequest);
        if(cor != null) return;
        cor = StartCoroutine(Instantiate());
    }

    public IEnumerator Instantiate()
    {
        while (pathRequests.Count > 0)
        {
            PathFinder();
            yield return new WaitForSeconds(1f);
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
        Dictionary<Node, int>costs = new Dictionary<Node, int>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        List<Node> path = new List<Node>();
//        path = new List<Node>();
//
      //  targetNode = pathRequests[0].requestedFrom.target;
       // startingNode = pathRequests[0].requestedFrom.start;
        
        boundaries.Add(new PriorityQueue(startingNode, 0));
        current = boundaries[0].node;
        Node first = current;
        
        costs.Add(current, 0);
        cameFrom[current] = null;
        visited.Add(current);

        while (boundaries != null)
        {
            current = GetFirst(boundaries);
            if (current == null) break;

            if(current == targetNode)
            {
                break;
            }
            
            neighbours = GetNeighbours(current);
            
            for (int i = 0; i < neighbours.Count; i++)
            {
                int distance = GetDistance(targetNode.position, neighbours[i].position);
                int newCost = costs[current] + 1;

                if(!costs.ContainsKey(neighbours[i]) || newCost < costs[neighbours[i]])
                {
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
        pathRequests[0].path = RetracePath(cameFrom, first);
        pathRequests[0].ReturnPath();
        StartCoroutine(pathRequests[0].requestedFrom.Move());
        pathRequests.RemoveAt(0);
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

                if ((int) ( xOffset + x + node.position.x) >= 0 &&
                    (int) ( xOffset + x + node.position.x) < nodes.GetLength(0))
                {
                    if ((int) ( zOffset + z + node.position.z) >= 0 &&
                        (int) ( zOffset + z + node.position.z) < nodes.GetLength(1))
                    {
                        n.Add(nodes[(int) (xOffset + x + node.position.x), (int) (z + zOffset + node.position.z)]);
                    }
                }
            }
        }

        return n;
        
    }
}
