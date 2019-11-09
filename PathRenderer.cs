using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRenderer : MonoBehaviour
{
    LineRenderer line;
    public float lineWidth = 0.25f;
    public Material mat;
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.SetWidth(lineWidth,lineWidth);
        line.material = mat;


            /*
            foreach (Node n in path)//this is the list of nodes returned from the retrace function in the pathfinder. This will render the dynamic path between the latest anchor and the cursor
            {

            }
     */
    }
    public void UpdateLine(List<Node> path)
    {
        line.positionCount = path.Count;
        //Vector3[] pos = new Vector3[path.Count];
        for (int i = 0; i< path.Count;i++)
        {
            line.SetPosition(i, path[i].worldPosition);
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
