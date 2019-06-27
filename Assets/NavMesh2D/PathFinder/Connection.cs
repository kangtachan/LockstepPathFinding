using System;
using System.Collections.Generic;
using UnityEngine;

public interface Connection<N> {

	/** Returns the non-negative cost of this connection */
	float GetCost();

	/** Returns the node that this connection came from */
	N GetFromNode();

	/** Returns the node that this connection leads to */
	N GetToNode();
}
