using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshFromPositions : MonoBehaviour
{


    
    public Transform[] points;
    public List<Vector3> vertices;//draw these with Gizmos
    public float width = 5.0f;
    //public int numTris;
    public int numSegments;
    public Vector3[] pointDirections;
    public Vector3[] pointDirectionsRotated;
    public Vector3[] mVerts;
    public int[] mTris;
    List<int> tris;

    void Start()
    {
        
        /*
        mVerts = new Vector3[4];
        mUVs = new Vector2[4];
        mTris = new int[6];

        

        mVerts[0] = new Vector3(-1.0f,0.0f,1.0f);
        mVerts[1] = new Vector3(1.0f,0.0f,1.0f);
        mVerts[2] = new Vector3(-1.0f,0.0f,-1.0f);
        mVerts[3] = new Vector3(1.0f,0.0f,-1.0f);

        mUVs[0] = new Vector2(0.0f,0.0f);
        mUVs[1] = new Vector2(1.0f,0.0f);
        mUVs[2] = new Vector2(0.0f,1.0f);
        mUVs[3] = new Vector2(1.0f,1.0f);

        mTris[0] = 0;
        mTris[1] = 1;
        mTris[2] = 3;

        mTris[3] = 0;
        mTris[4] = 3;
        mTris[5] = 2;


        mesh.vertices = mVerts;
        mesh.uv = mUVs;
        mesh.triangles = mTris;
        
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        */

    }
    void Update()
    {

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        
        
        tris = new List<int>();
        pointDirections = new Vector3[points.Length];
        pointDirectionsRotated = new Vector3[points.Length];
        vertices = new List<Vector3>();


        numSegments = points.Length - 1;

        for(int i = 0; i <=numSegments; i++)
        {
            if (i==numSegments)
            {
                pointDirections[i] = pointDirections[i - 1];
                

            }
            else
            {
                pointDirections[i] = (points[i + 1].localPosition - points[i].localPosition).normalized;
            }
            pointDirectionsRotated[i] = Quaternion.Euler(0, 90, 0) * pointDirections[i];
        }

        for (int i = 0; i < points.Length; i++)
        {
            //add outside point
            vertices.Add(points[i].localPosition + (pointDirectionsRotated[i] * width));
            vertices.Add(points[i].localPosition - (pointDirectionsRotated[i] * width));
            //Gizmos.DrawSphere(points[i].position + (pointDirectionsRotated[i] * width), 0.25f);
            //add inside point
            //Gizmos.DrawSphere(points[i].position - (pointDirectionsRotated[i] * width), 0.25f);

        }
        for(int i = 0; i < numSegments; i++)
        {
            int index = i*2;
            tris.Add(index);
            tris.Add(index+1);
            tris.Add(index+3);

            tris.Add(index);
            tris.Add(index+3);
            tris.Add(index+2);
        }
        //convert lists back into arrays so the mesh component can read them
        mVerts = new Vector3[vertices.Count];
        mTris = new int[tris.Count];

        for (int i = 0;i<vertices.Count;i++)
        {
            mVerts[i] = vertices[i];
        }


        for (int i = 0; i < tris.Count; i++)
        {
            mTris[i] = tris[i];
        }
        mesh.vertices = mVerts;
        mesh.triangles = mTris;

    }
 



}