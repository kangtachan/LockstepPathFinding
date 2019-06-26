
//













using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;





/**
 * 导航网格图像数据 <br>
 * 数据对象预处理
 * 
 * @author JiangZhiYong
 * @QQ 359135103 2017年11月7日 下午4:43:33
 */
public class TriangleGraph :IndexedGraph<Triangle> {

	private NavMeshData navMeshData;
	private List<Triangle> triangles = new List<Triangle>();

	/** 寻路三角形对应的共享边 */
	private Dictionary<Triangle, List<TriangleEdge>> sharedEdges;
	/** 独立边 */
	private Dictionary<Triangle, List<TriangleEdge>> isolatedEdgesMap;

	private int numDisconnectedEdges; // 不相连边的个数
	private int numConnectedEdges; // 相互连接边的数目
	private int numTotalEdges; // 三角形总边数

    public TriangleGraph(NavMeshData navMeshData,int scale) {
		this.navMeshData = navMeshData;
		navMeshData.check(scale);
		// 寻路三角形
		List<Triangle> pathTriangles = createTriangles(scale);
		// 共享的连接边
		HashSet<IndexConnection> pathIndexConnections = getIndexConnections(navMeshData.getPathTriangles());
		// 三角形共享连接边
		sharedEdges = createSharedEdgesMap(pathIndexConnections, pathTriangles,navMeshData.getPathVertices().ToList());
		isolatedEdgesMap = createIsolatedEdgesMap(sharedEdges);

		// Count edges of different types
		foreach(List<TriangleEdge> edges  in  isolatedEdgesMap.Values) {
			numDisconnectedEdges += edges.Count;
		}
		foreach(List<TriangleEdge> edges  in  sharedEdges.Values) {
			numConnectedEdges += edges.Count;
		}
		numConnectedEdges /= 2;
		numTotalEdges = numConnectedEdges + numDisconnectedEdges;
		//LOGGER.debug("地图{} 三角形{} 总共边{} 共享边{} 独立边{}", navMeshData.getMapID(),getTriangleCont(), numTotalEdges, numConnectedEdges,
		//		numDisconnectedEdges);
	}

	public List<Connection<Triangle>> getConnections(Triangle fromNode){
		var lst = new List<Connection<Triangle>>();
		var raw = sharedEdges[fromNode];
		foreach (var edge in raw) {
			lst.Add(edge);
		}
		return lst ;
		// return (Array<Connection<Triangle>>) (Array<?>)
		// sharedEdges.getValueAt(fromNode.index);
	}

	public int getIndex(Triangle node) {
		return node.getIndex();
	}

	public int getNodeCount() {
		return sharedEdges.Count;
	}

	public NavMeshData getNavMeshData() {
		return navMeshData;
	}

	/**
	 * 创建三角形列表
	 * 
	 * @author JiangZhiYong
	 * @QQ 359135103 2017年11月7日 下午5:58:20
	 * @return
	 */
	private List<Triangle> createTriangles(int scale) {
		int[] vertexIndexs; // 顶点
		Vector3[] vertices; // 坐标
		vertexIndexs = navMeshData.getPathTriangles();
		vertices = navMeshData.getPathVertices();
		int triangleIndex = 0; // 三角形下标
		int length=vertexIndexs.Length-3;
		for (int i = 0; i <=length ;) {
			int aIndex = vertexIndexs[i++];
			int bIndex = vertexIndexs[i++];
			int cIndex = vertexIndexs[i++];
			Triangle triangle=null;
			if(scale!=1) {
				triangle = new Triangle(vertices[aIndex], vertices[bIndex],vertices[cIndex], triangleIndex++,aIndex,bIndex,cIndex);
			}else {
				triangle = new Triangle(vertices[aIndex], vertices[bIndex],vertices[cIndex], triangleIndex++);
			}
			
			triangles.Add(triangle);
		}

		return triangles;
	}

	/**
	 * 获得三角形顶点坐标的共享边
	 * 
	 * @author JiangZhiYong
	 * @QQ 359135103 2017年11月8日 下午4:12:33
	 * @param indices
	 *            顶点下标列表
	 * @return
	 */
	private static HashSet<IndexConnection> getIndexConnections(int[] indices) {
		HashSet<IndexConnection> indexConnections = new HashSet<IndexConnection>();
		int[] edge = { -1, -1 };
		short i = 0;
		int j, a0, a1, a2, b0, b1, b2, triAIndex, triBIndex;
		while (i < indices.Length) {
			triAIndex = (short) (i / 3); // A三角形编号
			a0 = indices[i++];
			a1 = indices[i++];
			a2 = indices[i++];
			j = i;
			while (j < indices.Length) {

				triBIndex = (short) (j / 3); // B三角形编号
				b0 = indices[j++];
				b1 = indices[j++];
				b2 = indices[j++];
				if (triAIndex == triBIndex) {
					// j+=3;
					continue;
				}
				if (hasSharedEdgeIndices(a0, a1, a2, b0, b1, b2, edge)) {
					IndexConnection indexConnection1 = new IndexConnection(edge[0], edge[1], triAIndex, triBIndex);
					IndexConnection indexConnection2 = new IndexConnection(edge[1], edge[0], triBIndex, triAIndex);
					indexConnections.Add(indexConnection1);
					indexConnections.Add(indexConnection2);
					edge[0] = -1;
					edge[1] = -1;
					// LOGGER.debug("共享边：{} ->
					// {}",indexConnection1.ToString(),indexConnection2.ToString());
				}
			}
		}
		//LOGGER.debug("连接个数：{}", indexConnections.Count);
		return indexConnections;
	}

	/**
	 * 检测是否有共享边 Checks if the two triangles have shared vertex indices. The edge
	 * will always follow the vertex winding order of the triangle A. Since all
	 * triangles have the same winding order, triangle A should have the opposite
	 * edge direction to triangle B.
	 *
	 * @param a0
	 *            Vertex index on triangle A
	 * @param a1
	 * @param a2
	 * @param b0
	 *            Vertex index on triangle B
	 * @param b1
	 * @param b2
	 * @param edge
	 *            Output, the indices of the shared vertices in the winding order of
	 *            triangle A.
	 * @return True if the triangles share an edge.
	 */
	private static bool hasSharedEdgeIndices(int a0, int a1, int a2, int b0, int b1, int b2, int[] edge) {
		bool match0 = (a0 == b0 || a0 == b1 || a0 == b2);
		bool match1 = (a1 == b0 || a1 == b1 || a1 == b2);
		if (!match0 && !match1) { // 无两个共享点
			return false;
		} else if (match0 && match1) {
			edge[0] = a0;
			edge[1] = a1;
			return true;
		}
		bool match2 = (a2 == b0 || a2 == b1 || a2 == b2);
		if (match0 && match2) {
			edge[0] = a2;
			edge[1] = a0;
			return true;
		} else if (match1 && match2) {
			edge[0] = a1;
			edge[1] = a2;
			return true;
		}
		return false;
	}

	/**
	 * 创建每个三角形的共享边列表 Creates a map over each triangle and its Edge connections to
	 * other triangles. Each edge must follow the vertex winding order of the
	 * triangle associated with it. Since all triangles are assumed to have the same
	 * winding order, this means if two triangles connect, each must have its own
	 * edge connection data, where the edge follows the same winding order as the
	 * triangle which owns the edge data.
	 *
	 * @param indexConnections
	 * @param triangles
	 * @param vertexVectors
	 * @return
	 */
	private static Dictionary<Triangle, List<TriangleEdge>> createSharedEdgesMap(HashSet<IndexConnection> indexConnections,
			List<Triangle> triangles, List<Vector3> vertexVectors) {

		Dictionary<Triangle, List<TriangleEdge>> connectionMap = new Dictionary<Triangle, List<TriangleEdge>>();
		// connectionMap.ordered = true;

		foreach(Triangle tri  in  triangles) {
			connectionMap.Add(tri, new List<TriangleEdge>());
		}

		foreach(IndexConnection indexConnection  in  indexConnections) {
			Triangle fromNode = triangles.get(indexConnection.fromTriIndex);
			Triangle toNode = triangles.get(indexConnection.toTriIndex);
			Vector3 edgeVertexA = vertexVectors.get(indexConnection.edgeVertexIndex1);
			Vector3 edgeVertexB = vertexVectors.get(indexConnection.edgeVertexIndex2);

			TriangleEdge edge = new TriangleEdge(fromNode, toNode, edgeVertexA, edgeVertexB);
			connectionMap.get(fromNode).Add(edge);
			fromNode.connections.Add(edge);
			// LOGGER.debug("三角形：{} -->{}
			// {}-->{}",fromNode.getIndex(),toNode.getIndex(),fromNode.ToString(),toNode.ToString());
		}
		return connectionMap;
	}


	/**
	 * 存储相互连接三角形的关系 Class for storing the edge connection data between two adjacent
	 * triangles.
	 */
	public class IndexConnection {
		// The vertex indices which makes up the edge shared between two triangles.
		public int edgeVertexIndex1;
		public int edgeVertexIndex2;
		 // The indices of the two triangles sharing this edge.
		public int fromTriIndex;
		public int toTriIndex;

		public IndexConnection(int sharedEdgeVertex1Index, int edgeVertexIndex2, int fromTriIndex, int toTriIndex) {
			this.edgeVertexIndex1 = sharedEdgeVertex1Index;
			this.edgeVertexIndex2 = edgeVertexIndex2;
			this.fromTriIndex = fromTriIndex;
			this.toTriIndex = toTriIndex;
		}

		public override String ToString() {
			return "IndexConnection [edgeVertexIndex1=" + edgeVertexIndex1 + ", edgeVertexIndex2=" + edgeVertexIndex2
					+ ", fromTriIndex=" + fromTriIndex + ", toTriIndex=" + toTriIndex + "]";
		}

		public override int GetHashCode() {
			int prime = 31;
			int result = 1;
			result = prime * result + edgeVertexIndex1;
			result = prime * result + edgeVertexIndex2;
			result = prime * result + fromTriIndex;
			result = prime * result + toTriIndex;
			return result;
		}

		public override bool Equals(object obj) {
			if (this == obj)
				return true;
			if (obj == null)
				return false;
			if (GetType() != obj.GetType())
				return false;
			IndexConnection other = (IndexConnection) obj;
			if (edgeVertexIndex1 != other.edgeVertexIndex1)
				return false;
			if (edgeVertexIndex2 != other.edgeVertexIndex2)
				return false;
			if (fromTriIndex != other.fromTriIndex)
				return false;
			if (toTriIndex != other.toTriIndex)
				return false;
			return true;
		}

	}

	public Dictionary<Triangle, List<TriangleEdge>> getPathSharedEdges() {
		return sharedEdges;
	}

	/**
	 * 获取所有三角形列表
	 * 
	 * @return
	 */
	public List<Triangle> getTriangles() {
		return triangles;
	}

	/**
	 * 创建有一条边与其他三角形无连接的边关系 Map the isolated edges for each triangle which does not
	 * have all three edges connected to other triangles.
	 *
	 * @param connectionMap
	 * @return
	 */
	private static Dictionary<Triangle, List<TriangleEdge>> createIsolatedEdgesMap(Dictionary<Triangle, List<TriangleEdge>> connectionMap) {

		Dictionary<Triangle, List<TriangleEdge>> disconnectionMap = new Dictionary<Triangle, List<TriangleEdge>>();

		foreach(Triangle tri  in  connectionMap.Keys) {
			List<TriangleEdge> connectedEdges = connectionMap.get(tri);

			List<TriangleEdge> disconnectedEdges = new List<TriangleEdge>();
			disconnectionMap.Add(tri, disconnectedEdges);

			if (connectedEdges.Count < 3) {
				// This triangle does not have all edges connected to other triangles
				bool ab = true;
				bool bc = true;
				bool ca = true;
				foreach(TriangleEdge edge  in  connectedEdges) {
					if (edge.rightVertex == tri.a && edge.leftVertex == tri.b)
						ab = false;
					else if (edge.rightVertex == tri.b && edge.leftVertex == tri.c)
						bc = false;
					else if (edge.rightVertex == tri.c && edge.leftVertex == tri.a)
						ca = false;
				}
				if (ab)
					disconnectedEdges.Add(new TriangleEdge(tri, null, tri.a, tri.b));
				if (bc)
					disconnectedEdges.Add(new TriangleEdge(tri, null, tri.b, tri.c));
				if (ca)
					disconnectedEdges.Add(new TriangleEdge(tri, null, tri.c, tri.a));
			}
			int totalEdges = (connectedEdges.Count + disconnectedEdges.Count);
			if (totalEdges != 3) {
				//LOGGER.debug("Wrong number of edges (" + totalEdges + ") in triangle " + tri.getIndex());
			}
		}
		return disconnectionMap;
	}

	public int getNumDisconnectedEdges() {
		return numDisconnectedEdges;
	}

	public int getNumConnectedEdges() {
		return numConnectedEdges;
	}

	public int getNumTotalEdges() {
		return numTotalEdges;
	}

	public int getTriangleCont() {
		return triangles.Count;
	}
}
