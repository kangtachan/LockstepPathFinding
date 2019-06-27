
using System;
using System.Collections.Generic;
using UnityEngine;


/**图路径
 * <br>
 *  A {@code GraphPath} represents a path in a {@link Graph}. Note that a path can be defined in terms of nodes or
 * {@link Connection connections} so that multiple edges between the same pair of nodes can be discriminated.
 * 
 * @param <N> Type of node
 * 
 * @author davebaol */
public interface GraphPath<N>  {

	/** Returns the number of items of this path. */
	int GetCount ();

	/** Returns the item of this path at the given index. */
	N Get (int index);

	/** Adds an item at the end of this path. */
	void Add (N node);

	/** Clears this path. */
	void Clear ();

	/** Reverses this path. */
	void reverse ();

}
