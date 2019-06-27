using System;
using System.Collections.Generic;
using UnityEngine;

public class TriangleHeuristic :Heuristic<Triangle> {

	private static Vector3 A_AB = new Vector3();
	private static Vector3 A_BC = new Vector3();
	private static Vector3 A_CA = new Vector3();
	private static Vector3 B_AB = new Vector3();
	private static Vector3 B_BC = new Vector3();
	private static Vector3 B_CA = new Vector3();

	public float Estimate(Triangle node, Triangle endNode) {
		float dst2;
		float minDst2 = float.MaxValue;
		A_AB.set(node.a).Add(node.b).scl(0.5f);
		A_BC.set(node.b).Add(node.c).scl(0.5f);
		A_CA.set(node.c).Add(node.a).scl(0.5f);

		B_AB.set(endNode.a).Add(endNode.b).scl(0.5f);
		B_BC.set(endNode.b).Add(endNode.c).scl(0.5f);
		B_CA.set(endNode.c).Add(endNode.a).scl(0.5f);

		if ((dst2 = A_AB.dst2(B_AB)) < minDst2)
			minDst2 = dst2;
		if ((dst2 = A_AB.dst2(B_BC)) < minDst2)
			minDst2 = dst2;
		if ((dst2 = A_AB.dst2(B_CA)) < minDst2)
			minDst2 = dst2;

		if ((dst2 = A_BC.dst2(B_AB)) < minDst2)
			minDst2 = dst2;
		if ((dst2 = A_BC.dst2(B_BC)) < minDst2)
			minDst2 = dst2;
		if ((dst2 = A_BC.dst2(B_CA)) < minDst2)
			minDst2 = dst2;

		if ((dst2 = A_CA.dst2(B_AB)) < minDst2)
			minDst2 = dst2;
		if ((dst2 = A_CA.dst2(B_BC)) < minDst2)
			minDst2 = dst2;
		if ((dst2 = A_CA.dst2(B_CA)) < minDst2)
			minDst2 = dst2;

		return (float) Math.Sqrt(minDst2);
	}

}
