using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*This is the class that gets instanced in the navigation grid. 
 * It contains the infor for pathfinding such as movement costs as well as whether the node is traversable.
 * It will later be extended to contain the behaviour for cover and any other per-tile characteristics such as unit perception/area awareness.
 */

public class Node
{
    public bool isNavigable;//whether or not the tile can be walked on.
    public int hCost; //cost/distance from end position when pathfinding.
    public int gCost; //cost/distance from start position when pathfinding.
    public Vector3 worldPosition; //location of node in world space.
    public int gridX;
    public int gridY;
    public Node parent;

    public int fCost
    {
        get { return hCost + gCost;
        }
    }

    public Node(bool _isNavigable,Vector3 _worldPosotion, int _gridX, int _gridY)
    {
        isNavigable = _isNavigable;
        worldPosition = _worldPosotion;
        gridX = _gridX;
        gridY = _gridY;
    }


}
