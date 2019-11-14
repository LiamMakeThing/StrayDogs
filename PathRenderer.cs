using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class PathRenderer : MonoBehaviour
{


    public float lineWidth = 0.25f;
    public Material mat;
    List<Vector3> anchorPoints;
    
    List<Vector3> outputPoints;
    public float searchDist;
    Vector3[] pointDir;
    List<Vector3> newPoints;
    public float cornerBias;
    public float debugSphereSize;
    List<Vector3> newPointsDir;
    Mesh mesh;
    MeshFilter meshFilter;
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
   



    public void UpdateLine(List<Vector3> keyPoints)
    {
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
           
            int segments = keyPoints.Count - 1;
            //FIND POINT DIRECTIONS
            pointDir = new Vector3[keyPoints.Count];
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

            int pointsToProcess = (keyPoints.Count - 2);
            if (pointsToProcess > 0)
            {
                newPoints = new List<Vector3>();
                newPointsDir = new List<Vector3>();
                for (int i = 0; i < keyPoints.Count; i++)

                    //middle points (search forward and backward)
                    if (i > 0 && i < keyPoints.Count - 1)
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
                    //first point (only search forward)
                    else if (i == 0)
                    {
                        Vector3 newPoint = keyPoints[i] + (pointDir[i] * searchDist);

                        newPoints.Add(keyPoints[i]);
                        newPointsDir.Add(pointDir[i]);
                        newPoints.Add(newPoint);
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
            }


            /* rotate all point directions by 90
             *if point is an anchor point, average it out based on the previous and next point directions.
             *add flanking vertex point locations based on line width
             * 
             */
            List<Vector3> vertices = new List<Vector3>();
            for (int i = 0; i < newPoints.Count; i++)
            {


                Vector3 rotated = Quaternion.Euler(0, 90, 0) * newPointsDir[i];
                vertices.Add(newPoints[i] + (rotated * lineWidth));
                vertices.Add(newPoints[i] - (rotated * lineWidth));
            }
            List<int> tris = new List<int>();
            for (int i = 0; i < newPoints.Count - 1; i++)
            {
                int index = i * 2;
                tris.Add(index);
                tris.Add(index + 1);
                tris.Add(index + 3);

                tris.Add(index);
                tris.Add(index + 3);
                tris.Add(index + 2);
            }


            Vector3[] mVerts = new Vector3[vertices.Count];
            int[] mTris = new int[tris.Count];

            for (int i = 0; i < vertices.Count; i++)
            {
                mVerts[i] = vertices[i];
            }


            for (int i = 0; i < mTris.Length; i++)
            {
                mTris[i] = tris[i];
            }
            mesh.vertices = mVerts;
            mesh.triangles = mTris;

            outputPoints = vertices;

        }


    }
   }
