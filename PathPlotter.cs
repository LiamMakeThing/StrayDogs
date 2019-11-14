using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathPlotter : MonoBehaviour
{


    //public List<Vector3> waypoints;
    public List<Node> waypointNodes;
    GridGenerator grid;
    Pathfinding pathfinder;
    public PlayerController player;
    public Transform activeUnit;
    public GameObject waypointBeacon;
    public List<GameObject> spawnedBeacons;
    public GameObject cursorBeacon;
    public Vector3 anchor;
    public List<Node> path;
    public List<Vector3> lockedPath;

    Vector3 cursorLocation;
    List<Vector3> keyPositions;
    public PathRenderer dynamicLine;
    bool canMove;
    public bool optimizePath;


    /* Allow player to choose multiple waypoints.
    * Send to pathfinder to solve for each segment.
    * Combine all segments together.
    * 
    * position 0 is always the units position. Positions are added to the position list in the order they are clicked.
    * 
    */
    // Start is called before the first frame update
    void Start()
    {
        cursorBeacon = GameObject.Instantiate(waypointBeacon,Vector3.zero,transform.rotation);
        grid = GetComponent<GridGenerator>();
        pathfinder = GetComponent<Pathfinding>();
       // waypoints = new List<Vector3>();
        waypointNodes = new List<Node>();
        anchor = activeUnit.position;
        lockedPath = new List<Vector3>();
        canMove = true;
        


        //waypoints.Add(activeUnit.position);


    }
    
    public List<Vector3> RemoveInlines(List<Node> path)
    {
        /*Take in all the positions on the path.
         * Get the direction from the first to the second point. Store this as current vector.
         * Increment through the third, fourth and so on points, getting their direction in relation to the first point.
         * compare the two directions, if they are not the same, the first point in the second vector is a keypoint. Add it to the list and set that point as the new start point and continue
         */
        int anchorIndex = 0;
        Debug.Log("FunctionEntered");
        
        Vector3 currentDir;
        Vector3 nextDir;
        keyPositions = new List<Vector3>();
        if (optimizePath)
        {
            for (int inc = 1; inc <= path.Count - 2; inc++)
            {
                currentDir = (path[anchorIndex + 1].worldPosition - path[anchorIndex].worldPosition).normalized;
                nextDir = (path[inc + 1].worldPosition - path[anchorIndex].worldPosition).normalized;
                if (currentDir == nextDir)
                {


                    continue;



                }
                else if (currentDir != nextDir)
                {
                    //Debug.Log(currentDir);
                    //Debug.Log(nextDir);
                    keyPositions.Add(path[anchorIndex].worldPosition);
                    anchorIndex = inc;


                }

            }
            keyPositions.Add(path[path.Count - 1].worldPosition);
            keyPositions.Insert(0, path[0].worldPosition);
        }
        else
        {
            for (int i = 0; i<path.Count;i++) {
                keyPositions.Add(path[i].worldPosition);
            }
        }
        
        return keyPositions;
        
    }

    // Update is called once per frame
    void Update()
    {

        //trace between current anchor and mouse node.
        if (canMove)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))

            {
                //cursorBeacon.transform.position = grid.NodeFromWorldPosition(hit.point).worldPosition;
               
                cursorLocation = hit.point;
                pathfinder.FindPath(anchor, cursorLocation);
                Debug.Log(RemoveInlines(path).Count);
                keyPositions = RemoveInlines(path);
                cursorBeacon.transform.position = keyPositions[keyPositions.Count - 1];
                dynamicLine.UpdateLine(keyPositions);
               
              


            }
        }


        if (Input.GetMouseButtonDown(0))
        {


            anchor = keyPositions[keyPositions.Count - 1];
            StartCoroutine(player.MoveAlongPath(keyPositions));
            //canMove = false;

        }


    }

 
}
