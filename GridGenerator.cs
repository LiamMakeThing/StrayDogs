using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    int gridNodeCountX;
    int gridNodeCountY;
    public Vector2 gridWordUnitSize;
    public float nodeRadius;
    public float nodeNavSearchRadius;
    float nodeDiameter;
    public LayerMask notNavigable;
    Node[,] grid;
    public bool drawNavDebug;
    public Transform player;
    public List<Node> path;
    public float nodeVisualizerSize = 0.25f;

    private void Start()//set up scale, node count and call the grid generate function
    {
        nodeDiameter = nodeRadius * 2;
        gridNodeCountX = Mathf.RoundToInt(gridWordUnitSize.x / nodeDiameter);
        gridNodeCountY = Mathf.RoundToInt(gridWordUnitSize.y / nodeDiameter);
        GenerateGrid();


    }
    void GenerateGrid()
    {
        grid = new Node[gridNodeCountX, gridNodeCountY]; //make new grid structure array to hold rows and columns
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWordUnitSize.x / 2 - Vector3.forward * gridWordUnitSize.y / 2;//find the anchor at the bottom left of the grid by subtracting half the height and width
        for (int x = 0; x <gridNodeCountX; x++)
        {
            for(int y = 0;y<gridNodeCountY; y++)
            {
                Vector3 worldPointToPlaceNode = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool navigable = !(Physics.CheckSphere(worldPointToPlaceNode, nodeNavSearchRadius,notNavigable));//do a sphere check to see if the node overlaps with anything in the nonNavigable layer. Set the local bool navigable to the opposite of what is returned.
                grid[x, y] = new Node(navigable, worldPointToPlaceNode,x,y);
            }
        }
    }

    public Node NodeFromWorldPosition(Vector3 worldPos)
    {
        float percentX = (worldPos.x + gridWordUnitSize.x / 2) / gridWordUnitSize.x;
        float percentY = (worldPos.z + gridWordUnitSize.y / 2) / gridWordUnitSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridNodeCountX - 1) * percentX);
        int y = Mathf.RoundToInt((gridNodeCountY - 1) * percentY);

        return grid[x, y];

    }

    public List<Node> GetNeighbours(Node node)//Take in a node, search in a 3x3 grid around that node and store those nodes in a list called "neighbours".
    {
        List<Node> neighbours = new List<Node>();//make new empty list to store nodes. Its a list cause we dont know how many nodes will be found and arrays need the absolute length set at creation where lists can be resized dynamically. Normally we would find 8 neighbours but if a node is on an edge, it will have fewer.

        for (int x = -1; x<=1;x++)
        {
            for (int y =-1; y<=1;y++)
            {
                if (x == 0 && y == 0)//if x and y are 0 it means we are in the centre node, which is the node we are checking. We dont want to add it since it is not a neighbour, so we skip that one.
                {
                    continue;
                }

                //add to the position in the search to identify the next node to be checked, then see if that node is within the bounds of the grid. I.E. More than 0 and less than the max in each axis.
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if(checkX >= 0 && checkX < gridNodeCountX && checkY >=0 && checkY < gridNodeCountY)
                {
                    neighbours.Add(grid[checkX, checkY]);//if all goes well, we have skipped the node we are checking, and made sure that we only add nodes that are within the bounds of the grid.
                }
            }
        }
        return neighbours;//send the list of neighbour nodes back to whatever called this function.

    }

    private void Update()
    {
        //Overlapping nodes with cursor location get a UI or ball visualizer
    }




    private void OnDrawGizmos()//DRAW DEBUG VISUALIZERS FOR NODES
    {
        if (drawNavDebug)
        {


            Gizmos.DrawWireCube(transform.position, new Vector3(gridWordUnitSize.x, 1.0f, gridWordUnitSize.y));//draw box around entire grid

            if (grid != null)
            {
                Node playernode = NodeFromWorldPosition(player.position);
                foreach (Node n in grid)
                {
                    if (n.isNavigable)
                    {
                        Gizmos.color = Color.white;
                        if (path.Contains(n))
                        {
                            Gizmos.color = Color.blue;
                        }
                        //Gizmos.DrawSphere(n.worldPosition,nodeVisualizerSize);
                    }
                    
                    else
                    {
                        Gizmos.color = Color.red;

                        //Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));

                    }
                    Gizmos.DrawSphere(n.worldPosition, nodeVisualizerSize);
                }
            }
        }
    }
}


