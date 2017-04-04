using System;

namespace RoutingSimulator.Core
{
    public class Edge<TNode> : IComparable<Edge<TNode>>, IEquatable<Edge<TNode>>
                 where TNode : IComparable<TNode>, IEquatable<TNode>
    {
        public Node<TNode> Node1 { get; private set; }
        public Node<TNode> Node2 { get; private set; }
        public long Weight { get; private set; }

        public Edge(Node<TNode> node1, Node<TNode> node2, long weight)
        {
            Node1 = node1;
            Node2 = node2;
            Weight = weight;
        }

        public int CompareTo(Edge<TNode> other)
        {
            return Weight.CompareTo(other.Weight);
        }

        public bool Equals(Edge<TNode> other)
        {
            return ((Node1.Equals(other.Node1) && Node2.Equals(other.Node2)) ||
                   ((Node1.Equals(other.Node2) && Node2.Equals(other.Node1)))) &&
                   Weight.Equals(other.Weight);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(Node1, Node2, Weight).GetHashCode();
        }
    }
}
