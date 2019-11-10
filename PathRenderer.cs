using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRenderer : MonoBehaviour
{
    LineRenderer line;
    public float lineWidth = 0.25f;
    public Material mat;
    List<Vector3> anchorPoints;
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.startWidth=lineWidth;
        line.endWidth=lineWidth;
        line.material = mat;


            /*
            foreach (Node n in path)//this is the list of nodes returned from the retrace function in the pathfinder. This will render the dynamic path between the latest anchor and the cursor
            {

            }
     */
    }
    public void UpdateLine(List<Vector3> keyPoints)
    {
        anchorPoints = new List<Vector3>();
        anchorPoints = keyPoints;
        line.positionCount = anchorPoints.Count;
        //Vector3[] pos = new Vector3[path.Count];
        for (int i = 0; i< anchorPoints.Count;i++)
        {
            line.SetPosition(i, anchorPoints[i]);
        }
        
        
    }
    private void OnDrawGizmos()
    {
        foreach (Vector3 pos in anchorPoints)
        {
            Gizmos.DrawSphere(pos,0.25f);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
