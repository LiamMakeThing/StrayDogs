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
    public List<Node> lockedPath;

    Vector3 cursorLocation;
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
        lockedPath = new List<Node>();
        


        //waypoints.Add(activeUnit.position);


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
            dynamicLine.UpdateLine(path);
            

        }

        if (Input.GetMouseButtonDown(0))
        {
            //move anchor to new position. 

            //add path to lockedpath 

            for(int i = 1; i < path.Count; i++)
            {
                lockedPath.Add(path[i]);
            }

          

            
            staticLine.UpdateLine(lockedPath);
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
