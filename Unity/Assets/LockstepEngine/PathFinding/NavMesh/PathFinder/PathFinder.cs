namespace Lockstep.PathFinding {
	public interface PathFinder<N> {

		bool SearchPath(N startNode, N endNode, Heuristic<N> heuristic, GraphPath<Connection<N>> outPath);

		bool SearchNodePath(N startNode, N endNode, Heuristic<N> heuristic, GraphPath<N> outPath);
	}
}