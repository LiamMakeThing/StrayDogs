using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathRenderer : MonoBehaviour
{


    public float lineWidth = 0.25f;
    public Material mat;
    List<Vector3> anchorPoints;
    
    public GameObject debugAsset;



    public float searchDist;
    Vector3[] pointDir;
    List<Vector3> newPoints;
    public float cornerBias;
    public float debugSphereSize;
    List<Vector3> newPointsDir;
    public Vector3[] meshVertices;
    Mesh mesh;
    MeshFilter meshFilter;
    GameObject[] debugAssets;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        GetComponent<MeshRenderer>().material = mat;


    }

    void Update()
    {
    }


    

    public void UpdateLine(List<Vector3> keyPoints,int pathLength, int lastIndex)
    {


        /*PSEUDO
         * Find the direction of each point and assign it to a vector
         * make a new point array equal to the current point (count -2)*2.
         * Get each point, not first, not last.
         * For each point, find the location at a set distance ahead and behind the current point based on the point direction vectors.
         * Insert these points into the point array ahead of and after the current point index.
         * Find the average location between these two new points, and lerp it with the current point with a custom weight to give rounding.
         * draw all the points to the screen
         * 
         * we want each 'segment' to have the same direction, and have the elbow averaged between them.
         */
        mesh.Clear();
        int segments = keyPoints.Count - 1;
        
        //FIND POINT DIRECTIONS
        pointDir = new Vector3[keyPoints.Count];
        
            //Debug.Log(keyPoints.Count);
            for (int i = 0; i < pointDir.Length; i++)
            {
                //if its the last point, we cant project forward to the next point to get the direction so just use the previous points direction to continue the direction forward past the last point.
                if (i == segments)
                {
                    pointDir[i] = pointDir[i - 1];
                }//otherwise, get the direction between the current point and the next one.
                else
                {
                    pointDir[i] = (keyPoints[i + 1] - keyPoints[i]).normalized;
                }

            }
          
            newPoints = new List<Vector3>();
            newPointsDir = new List<Vector3>();
            for (int i = 0; i < keyPoints.Count; i++)

                //middle points (search forward and backward)
          
                //first point (only search forward)
                if (i == 0)
                {
                    Vector3 newPoint = keyPoints[i] + (pointDir[i] * searchDist);

                    newPoints.Add(keyPoints[i]);
                    newPointsDir.Add(pointDir[i]);
                    newPoints.Add(newPoint);
                    newPointsDir.Add(pointDir[i]);
                }
            else if (i > 0 && i < keyPoints.Count - 1)
            {
                Vector3 newPointA = keyPoints[i] - (pointDir[i - 1] * searchDist);
                Vector3 newPointC = keyPoints[i] + (pointDir[i] * searchDist);
                Vector3 newPointB = Vector3.Lerp(keyPoints[i], (newPointA + newPointC) / 2, cornerBias);
                newPoints.Add(newPointA);
                newPointsDir.Add(pointDir[i - 1]);
                newPoints.Add(newPointB);
                newPointsDir.Add((pointDir[i] + pointDir[i - 1]) / 2);
                newPoints.Add(newPointC);
                newPointsDir.Add(pointDir[i]);

            }
            //last point (only search backward)
            else if (i == keyPoints.Count - 1)
                {
                    Vector3 newPoint = keyPoints[i] - (pointDir[i] * searchDist);

                    newPoints.Add(newPoint);
                    newPointsDir.Add(pointDir[i]);
                    newPoints.Add(keyPoints[i]);
                    newPointsDir.Add(pointDir[i]);

                }

            /* rotate all point directions by 90
             *if point is an anchor point, average it out based on the previous and next point directions.
             *add flanking vertex point locations based on line width
             * 
             */


            //mTris
            int numberOfTriangles = (newPoints.Count - 1) * 2;
            int numberOfSegments = newPoints.Count - 1;
            int numberofVertices = newPoints.Count * 2;

            //Debug.Log(keyPoints.Count - 1 + " spans, " + numberOfTriangles + " triangles, " + numberofVertices + " vertices");
            List<int> triangles = new List<int>();

           // Debug.Log("NewPoints " + newPoints.Count + "NewPointsDir " + newPointsDir.Count);
            

            for(int i = 0; i < numberOfSegments; i++)
            {
                triangles.Add(i * 2);
                triangles.Add(i * 2+1);
                triangles.Add(i * 2+3);

                triangles.Add(i * 2);
                triangles.Add(i * 2+3);
                triangles.Add(i * 2+2);

                
            }
            int[] meshTriangles = new int[triangles.Count];
            meshVertices = new Vector3[numberofVertices];



            for (int i = 0; i < newPoints.Count; i++)
            {
                meshVertices[i * 2] = newPoints[i] + (Quaternion.Euler(0, 90, 0) * newPointsDir[i] * lineWidth);
                meshVertices[(i * 2) + 1] = newPoints[i] - (Quaternion.Euler(0, 90, 0) * newPointsDir[i] * lineWidth);

            }
            for (int i =0;i<meshTriangles.Length;i++)
            {
                meshTriangles[i] = triangles[i];
            }
            //SetUpUvs
            /*U is width
             * V is length
             * U is always 0 or 1
             * V starts at 0, ends at 1 and uses normalized values for each other point. the number of points/2
             * 
             */ 
            mesh.vertices = meshVertices;
            mesh.triangles = meshTriangles;
                    
            
        


        


    }
  

    private void OnDrawGizmos()
    {
        foreach(Vector3 pos in meshVertices)
        {
            Gizmos.DrawSphere(pos, debugSphereSize);
        }
    }

}
