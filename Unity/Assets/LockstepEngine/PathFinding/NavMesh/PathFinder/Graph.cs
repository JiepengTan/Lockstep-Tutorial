

using System;
using System.Collections.Generic;
using Lockstep.Math;



/**
 * 图接口 
 * <br>
 * A graph is a collection of nodes, each one having a collection of
 * outgoing {@link Connection connections}.
 * 
 * @param <N>
 *            Type of node
 * 
 * @author davebaol
 */

using System.Collections.Generic;

namespace Lockstep.PathFinding {
	public interface Graph<N> {

		/**和当前节点相连的连接关系
		 * Returns the connections outgoing from the given node.
		 * 
		 * @param fromNode
		 *            the node whose outgoing connections will be returned
		 * @return the array of connections outgoing from the given node.
		 */
		List<Connection<N>> GetConnections(N fromNode);
	}
}