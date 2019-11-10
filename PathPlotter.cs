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
    public PathRenderer staticLine;


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
        List<Vector3> keyPositions = new List<Vector3>();
        
        for (int inc = 1; inc<=path.Count-2;inc++) {
            currentDir = (path[anchorIndex + 1].worldPosition - path[anchorIndex].worldPosition).normalized;
            nextDir = (path[inc + 1].worldPosition- path[anchorIndex].worldPosition).normalized;
            if (currentDir == nextDir)
            {

               
                continue;
                


            }
            else if(currentDir!=nextDir) {
                //Debug.Log(currentDir);
                //Debug.Log(nextDir);
                keyPositions.Add(path[anchorIndex].worldPosition);
                anchorIndex = inc;
                
            
            }
            
        }
        keyPositions.Add(path[path.Count-1].worldPosition);
        keyPositions.Insert(0, path[0].worldPosition);

        
        return keyPositions;
        
    }

    // Update is called once per frame
    void Update()
    {

        //trace between current anchor and mouse node.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            cursorBeacon.transform.position = hit.point;
            cursorLocation = hit.point;
            pathfinder.FindPath(anchor,cursorLocation);
            Debug.Log(RemoveInlines(path).Count);
            dynamicLine.UpdateLine(RemoveInlines(path));
            

        }


        if (Input.GetMouseButtonDown(0))
        {
            //move anchor to new position. 

            //add path to lockedpath 

            for(int i = 1; i < keyPositions.Count; i++)
            {
                lockedPath.Add(keyPositions[i]);
            }

          

            
        //    staticLine.UpdateLine(lockedPath);
            anchor = grid.NodeFromWorldPosition(cursorLocation).worldPosition;
            AddBeacon(anchor);
        }


        
        //Click the right click to cancel a move
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Movement cleared");
            foreach(GameObject go in spawnedBeacons)
            {

                Destroy(go);
            
            }
            spawnedBeacons.Clear();
            waypointNodes.Clear();

        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(player.MoveAlongPath(lockedPath));
            
        }
    }

    void AddBeacon(Vector3 pos)
    {
        GameObject newBeacon = GameObject.Instantiate(waypointBeacon, pos, transform.rotation);
        spawnedBeacons.Add(newBeacon);
    }
}
