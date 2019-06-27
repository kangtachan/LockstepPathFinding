using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using UnityEngine;

public class TriangleNavMesh : NavMesh {
    private TriangleGraph _graph; 
    private TriangleHeuristic _heuristic; 
    private IndexedAStarPathFinder<Triangle> _pathFinder; 

    public TriangleNavMesh(String navMeshStr) : this(navMeshStr, 1){ }

    public TriangleNavMesh(String navMeshStr, int scale){
        var data = JsonMapper.ToObject<TriangleData>(navMeshStr);
        _graph = new TriangleGraph(data,scale);
        _pathFinder = new IndexedAStarPathFinder<Triangle>(_graph);
        _heuristic = new TriangleHeuristic();
    }

    public TriangleGraph getGraph(){
        return _graph;
    }

    public TriangleHeuristic getHeuristic(){
        return _heuristic;
    }

    public IndexedAStarPathFinder<Triangle> getPathFinder(){
        return _pathFinder;
    }

   
    public Triangle GetTriangle(Vector3 point){
        return _graph.GetTriangle(point);
    }
   
    private bool FindPath(Vector3 fromPoint, Vector3 toPoint, TriangleGraphPath path){
        path.Clear();
        Triangle fromTriangle = GetTriangle(fromPoint);
        if (_pathFinder.SearchPath(fromTriangle, GetTriangle(toPoint), _heuristic, path)) {
            path.start = fromPoint;
            path.end = toPoint;
            path.startTri = fromTriangle;
            return true;
        }

        return false;
    }

  
    public List<Vector3> FindPath(Vector3 fromPoint, Vector3 toPoint, TrianglePointPath navMeshPointPath){
        TriangleGraphPath navMeshGraphPath = new TriangleGraphPath();
        bool find = FindPath(fromPoint, toPoint, navMeshGraphPath);
        if (!find) {
            return navMeshPointPath.getVectors();
        }

        navMeshPointPath.calculateForGraphPath(navMeshGraphPath, false);
        return navMeshPointPath.getVectors();
    }
}