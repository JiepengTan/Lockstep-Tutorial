
using System;
using System.Collections.Generic;
using Lockstep.Math;

namespace Lockstep.PathFinding {

	public interface GraphPath<N> {

		/** Returns the number of items of this path. */
		int GetCount();

		/** Returns the item of this path at the given index. */
		N Get(int index);

		/** Adds an item at the end of this path. */
		void Add(N node);

		/** Clears this path. */
		void Clear();

		/** Reverses this path. */
		void reverse();

	}
}
