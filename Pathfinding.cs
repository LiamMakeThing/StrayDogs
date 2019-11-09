using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    GridGenerator grid;
    PathPlotter pathPlotter;
    //public Transform seeker;
    //public Transform destination;

    private void Awake()
    {
        grid = GetComponent<GridGenerator>();
        pathPlotter = GetComponent<PathPlotter>();
    }
    private void Update()
    {
       // FindPath(seeker.position, destination.position);

        
    }
    public void FindPath(Vector3 startPosition,Vector3 endPosition)
    {
        Node startNode = grid.NodeFromWorldPosition(startPosition);
        Node targetNode = grid.NodeFromWorldPosition(endPosition);


        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost<currentNode.hCost)//check to see if the current node has the lowest fcost or if its the same, see if it has the lower hcost (closer to target position)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach(Node neighbour in grid.GetNeighbours(currentNode))//for each neighbour node of the current node
            {
                if(!neighbour.isNavigable || closedSet.Contains(neighbour))//if the neighbour is not navigable or has already been evaluated, skip it
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);//new movement cost is the gcost (cost it took to get where we are, plus the distance to the potential neighbour node.)
                if(newMovementCostToNeighbour<neighbour.gCost || !openSet.Contains(neighbour))//if the cost to get to the nighbour from the current node is less than one already found (its gCost) or the neightbour is not in the openset (we havent visitid it yet ), set its gCost and hCost
                {
                    neighbour.gCost = newMovementCostToNeighbour; //gCost is the new, lower cost to get there from this node
                    neighbour.hCost = GetDistance(neighbour, targetNode);//hCost is the distance from neighbour to the end location
                    neighbour.parent = currentNode;//now that we found either a new or shorter path to get to this node, set its parent to the current node so we know where we came from in order to backtrack later.

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour); //if the node has not been visited before/this is the first time visiting it (not in the open set), add it to the open set so we can evaluate it as a current node in a future iteration.
                    }
                }
            }


        }
    }

    void RetracePath(Node startNode, Node endNode)//walk back through the nodes parental heirarchy to find the path. Reverse it to get the forward direction as we marched through it backwards to trace the path from the end to the start.
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        //grid.path = path;
        pathPlotter.path = path;
    }


    int GetDistance (Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if(distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distX + 10 * (distY - distX);
        }
    }
}
