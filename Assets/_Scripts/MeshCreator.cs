/*
 * Taken from http://forum.unity3d.com/threads/54092-Draw-Polygon by AlexG
 * This creates a mesh and attaches it to two game objects. The mesh is stitched counter-clockwise in one object, and clockwise in another game object. 
 * Then both objects are drawn on top of each other. The reason I do this is so that way the object is visible on the front and back.
 * */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class MeshCreator {
    GameObject[] _myObject;
    public Material Mat;
    Mesh _mesh;

	// Use this for initialization
	public MeshCreator(string pathMaterial) {
        _myObject = new GameObject[2];
        for (int i = 0; i < 2; i++) {
            //New mesh and game object
            _myObject[i] = new GameObject();
            _myObject[i].name = "MousePolygon";
            _mesh = new Mesh();

            //Components
            _myObject[i].AddComponent<MeshFilter>();
            _myObject[i].AddComponent<MeshRenderer>();
            _myObject[i].GetComponent<MeshFilter>().mesh = _mesh;
        }
        Mat = new Material(Resources.Load(pathMaterial) as Material);


	}

    public void ClearMeshes() {
        for (int i = 0; i < 2; i++) {
            // GetComponent<MeshFilter>().mesh.Clear();
            _mesh.Clear();
            _myObject[i].GetComponent<MeshFilter>().mesh.Clear();

            MeshFilter MF = _myObject[i].GetComponent<MeshFilter>();
            MF.mesh.Clear();

        
        }
    }
    public void UpdatePolygon(List<Vector3> nodePositions) {
        
        for (int i = 0; i < 2; i++) {
           // GetComponent<MeshFilter>().mesh.Clear();
            _mesh.Clear();
            _myObject[i].GetComponent<MeshFilter>().mesh.Clear();
       
            MeshFilter MF = _myObject[i].GetComponent<MeshFilter>();
            MF.mesh.Clear();
            MeshRenderer MR = _myObject[i].GetComponent<MeshRenderer>();

            //Create mesh
            CreateMesh(_mesh, nodePositions, i);

            //Assign materials
            MR.material = Mat;
            //Assign mesh to game object

            MF.mesh = _mesh;
            
        }

    }

    Mesh CreateMesh(Mesh mesh, List<Vector3> nodePositions, int num) {

        int i; //Counter

        //Create a new mesh
        //Mesh mesh = new Mesh();

        //Vertices

        Vector3[] vertex = new Vector3[nodePositions.Count];
        for (i = 0; i < nodePositions.Count; i++) {
            vertex[i] = nodePositions[i];

        }

        //UVs

        var uvs = new Vector2[vertex.Length];
        for (i = 0; i < vertex.Length; i++) {

            if ((i % 2) == 0)
                uvs[i] = new Vector2(0, 0);

            else
                uvs[i] = new Vector2(1, 1);

        }

        //Triangles

        int[] tris = new int[3 * (vertex.Length - 2)];    //3 verts per triangle * num triangles

        int C1, C2, C3;

        if (num == 0) {

            C1 = 0;
            C2 = 1;
            C3 = 2;


            for (i = 0; i < tris.Length; i += 3) {
                tris[i] = C1;
                tris[i + 1] = C2;
                tris[i + 2] = C3;
                C2++;
                C3++;
            }
        }

        else {
            C1 = 0;
            C2 = vertex.Length - 1;
            C3 = vertex.Length - 2;

            for (i = 0; i < tris.Length; i += 3) {
                tris[i] = C1;
                tris[i + 1] = C2;
                tris[i + 2] = C3;

                C2--;
                C3--;

            }

        }

        //Assign data to mesh
        mesh.vertices = vertex;
        mesh.uv = uvs;
        mesh.triangles = tris;

        //Recalculations
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        //Name the mesh
        mesh.name = "MyMesh";

        //Return the mesh
        return mesh;

    }
}
