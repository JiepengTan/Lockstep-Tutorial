using System;
using System.Collections.Generic;
using Lockstep.Math;

namespace Lockstep.PathFinding {
	public interface Connection<N> {

		/** Returns the non-negative cost of this connection */
		LFloat GetCost();

		/** Returns the node that this connection came from */
		N GetFromNode();

		/** Returns the node that this connection leads to */
		N GetToNode();
	}
}