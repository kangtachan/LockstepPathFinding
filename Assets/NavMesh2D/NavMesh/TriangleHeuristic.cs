
///*******************************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;



// * Copyright 2015 See AUTHORS file.
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// * http://www.apache.org/licenses/LICENSE-2.0
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// ******************************************************************************/
//
//


/**navmesh 启发式消耗预估
 * <br>
 * @author jsjolund
 */
public class TriangleHeuristic :Heuristic<Triangle> {

	private static Vector3 A_AB = new Vector3();
	private static Vector3 A_BC = new Vector3();
	private static Vector3 A_CA = new Vector3();
	private static Vector3 B_AB = new Vector3();
	private static Vector3 B_BC = new Vector3();
	private static Vector3 B_CA = new Vector3();

	/**
	 * Estimates the distance between two triangles, by calculating the distance
	 * between their edge midpoints.
	 *
	 * @param node
	 * @param endNode
	 * @return
	 */
	public float estimate(Triangle node, Triangle endNode) {
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
