using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/**
 * 寻路网格
 * <br>
 * TODO 获取不在寻路中，离当前点最近且在寻路层中的点；随机或者指定点的周围坐标点
 * @note 限制：三角形顶点需要在寻路层边缘，不能存在共边不共顶点
 * @author JiangZhiYong
 * @QQ 359135103 2017年11月7日 下午4:40:36
 */
public class TriangleNavMesh : NavMesh {
    private TriangleGraph graph; // 导航数据图
    private TriangleHeuristic heuristic; // 寻路消耗计算
    private IndexedAStarPathFinder<Triangle> pathFinder; // A*寻路算法


    public TriangleNavMesh(String navMeshStr) : this(navMeshStr, 1){ }

    /**
     * 
     * @param navMeshStr
     *            导航网格数据
     *            @param scale 放大倍数
     */
    public TriangleNavMesh(String navMeshStr, int scale){
        //graph = new TriangleGraph(JSON.parseObject(navMeshStr, TriangleData.class),scale);
        //TODO
        pathFinder = new IndexedAStarPathFinder<Triangle>(graph);
        heuristic = new TriangleHeuristic();
    }

    public TriangleGraph getGraph(){
        return graph;
    }

    public TriangleHeuristic getHeuristic(){
        return heuristic;
    }

    public IndexedAStarPathFinder<Triangle> getPathFinder(){
        return pathFinder;
    }

    /**
     * 查询路径
     * 
     * @param fromPoint
     * @param toPoint
     * @param path
     */
    private bool findPath(Vector3 fromPoint, Vector3 toPoint, TriangleGraphPath path){
        path.clear();
        Triangle fromTriangle = getTriangle(fromPoint);
        if (pathFinder.searchConnectionPath(fromTriangle, getTriangle(toPoint), heuristic, path)) {
            path.start = fromPoint;
            path.end = toPoint;
            path.startTri = fromTriangle;
            return true;
        }

        return false;
    }

    /**
     * 获取路径
     * 
     * @param fromPoint
     * @param toPoint
     * @param navMeshPointPath
     * @return
     */
    public List<Vector3> findPath(Vector3 fromPoint, Vector3 toPoint, TrianglePointPath navMeshPointPath){
        TriangleGraphPath navMeshGraphPath = new TriangleGraphPath();
        bool find = findPath(fromPoint, toPoint, navMeshGraphPath);
        if (!find) {
            return navMeshPointPath.getVectors();
        }

        navMeshPointPath.calculateForGraphPath(navMeshGraphPath, false);
        return navMeshPointPath.getVectors();
    }


    /**
     * 获取坐标点所在的三角形
     * 
     * @note 很耗时，迭代所有三角形寻找
     * @param point
     * @return
     */
    public Triangle getTriangle(Vector3 point){
        //TODO 高度判断，有可能有分层重叠多边形
        var optional = graph.getTriangles().Where(t => t.isInnerPoint(point)).First();
        return optional;
    }

}