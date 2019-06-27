using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Lockstep.Math;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using Debug = Lockstep.Logging.Debug;

public class Test : MonoBehaviour {
    public Transform srcPoint;
    public Transform dstPoint;

    public LineRenderer lineRenderer;

    public List<Vector3> pathPoints = new List<Vector3>();
    public bool isDrawLine = false;

    public TriangleNavMesh NavMesh;

    void CheckInit(){
        if (NavMesh != null) {
            return;
        }

        var _mapId = int.Parse(SceneManager.GetActiveScene().name.Replace("map", ""));
        var txt = Resources.Load<TextAsset>("Maps/" + _mapId + ".navmesh");
        NavMesh = new TriangleNavMesh(txt.text);
        if (lineRenderer == null)
            lineRenderer = GetComponentInChildren<LineRenderer>();
        debugGo = new GameObject("PathMesh");
        debugMesh = new Mesh();
        debugGo.AddComponent<MeshRenderer>().material = debugMeshMat;
        debugGo.AddComponent<MeshFilter>().mesh = debugMesh;
        debugGo.transform.position = Vector3.up;
    }

    private void Update(){
        CheckInit();
        DrawLine();
    }

    public TrianglePointPath path = new TrianglePointPath();
    public float widthMultiplier = 2;
    public List<Triangle> DebugTriangles = new List<Triangle>();
    private Mesh debugMesh;
    private GameObject debugGo;
    public Material debugMeshMat;
    public float useTime;

    void DrawLine(){
        if (isDrawLine) {
            Profiler.BeginSample(" FindPath");
            var time = DateTime.Now;
            pathPoints = NavMesh.FindPath(srcPoint.position, dstPoint.position, path);
            useTime = (float) (DateTime.Now - time).TotalMilliseconds;
            Profiler.EndSample();
            //isDrawLine = false;
        }

        var graph = NavMesh.navMeshGraphPath;
        if (graph != null) {
            DebugTriangles.Clear();
            foreach (var node in graph.nodes) {
                DebugTriangles.Add(node.GetFromNode());
            }

            DebugTriangles.Add(graph.GetEndTriangle());
            var triCount = DebugTriangles.Count;
            if (DebugTriangles.Count <= 1 || DebugTriangles[0] == null) {
                triCount = 0;
                debugMesh.Clear();
            }
            else {
                var colors = new Color[triCount * 3];
                var vecs = new Vector3[triCount * 3];
                var idxs = new int[triCount * 3];
                for (int i = 0; i < triCount; i++) {
                    var tri = DebugTriangles[i];
                    vecs[i * 3 + 0] = tri.a;
                    vecs[i * 3 + 1] = tri.b;
                    vecs[i * 3 + 2] = tri.c;
                    idxs[i * 3 + 0] = i * 3 + 0;
                    idxs[i * 3 + 1] = i * 3 + 1;
                    idxs[i * 3 + 2] = i * 3 + 2;

                    colors[i * 3 + 0] = new Color(0, i * 1.0f / triCount, 0, 1);
                    colors[i * 3 + 1] = new Color(0, i * 1.0f / triCount, 0, 1);
                    colors[i * 3 + 2] = new Color(0, i * 1.0f / triCount, 0, 1);
                }

                try { 
                
                    debugMesh.vertices = vecs;
                    debugMesh.colors = colors;
                    debugMesh.triangles = idxs;
                    debugMesh.RecalculateBounds();
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        lineRenderer.positionCount = pathPoints.Count;
        lineRenderer.widthMultiplier = widthMultiplier;
        lineRenderer.SetPositions(pathPoints.ToArray());
    }
}