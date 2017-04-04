using RoutingSimulator.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoutingSimulator.Core
{
    public class UndirectedWeightedGraph<TNode> where TNode : IComparable<TNode>, IEquatable<TNode>
    {
        private ISet<Node<TNode>> _nodes;
        private IDictionary<Node<TNode>, ISet<Edge<TNode>>> _adjacencyList;

        public UndirectedWeightedGraph()
        {
            _nodes = new HashSet<Node<TNode>>();
            _adjacencyList = new Dictionary<Node<TNode>, ISet<Edge<TNode>>>();
        }

        public bool AddNode(Node<TNode> node)
        {
            if (_nodes.Contains(node))
                return false;
            _nodes.Add(node);
            _adjacencyList.Add(node, new HashSet<Edge<TNode>>());
            return true;
        }

        public bool RemoveNode(Node<TNode> node)
        {
            if (!_nodes.Contains(node))
                return false;
            _nodes.Remove(node);
            _adjacencyList.Remove(node);
            foreach(var neigh in _adjacencyList.Values)
            {
                neigh.RemoveWhere(x => node == x.Node1 || node == x.Node2);
            }
                
            return true;
        }

        /// <summary>
        /// Adds a bidirectional edge between node1 and node2
        /// </summary>
        public void AddEdge(Edge<TNode> edge)
        {
            if (!_nodes.Contains(edge.Node1) || !_nodes.Contains(edge.Node2))
                throw new NodeDoesNotExistException("The specified node does not exist in this graph.");
            _adjacencyList[edge.Node1].Add(edge);
            _adjacencyList[edge.Node2].Add(edge);
            edge.Node1.DistanceVectorUpdated += (s, e) =>
            {
                MergeDistanceVectors(s as Node<TNode>, edge.Node2, edge.Weight);
            };
            edge.Node2.DistanceVectorUpdated += (s, e) =>
            {
                MergeDistanceVectors(s as Node<TNode>, edge.Node1, edge.Weight);
            };
            MergeDistanceVectors(edge.Node1, edge.Node2, edge.Weight);
        }

        public void RemoveEdge(Edge<TNode> edge)
        {
            if(_adjacencyList.ContainsKey(edge.Node1))
            {
                _adjacencyList[edge.Node1].Remove(edge);
            }
            if(_adjacencyList.ContainsKey(edge.Node2))
            {
                _adjacencyList[edge.Node2].Remove(edge);
            }
        }

        private void MergeDistanceVectors(Node<TNode> node1, Node<TNode> node2, long distance)
        {
            var mergedVector1 = new DistanceVector<TNode>(node1);
            var mergedVector2 = new DistanceVector<TNode>(node2);
            var destinations = node1.DistanceVector.Entries
                                                   .Select(x => x.Destination)
                                                   .Union(node2.DistanceVector
                                                           .Entries
                                                           .Select(x => x.Destination));
            bool isBetter1 = false, isBetter2 = false;
            foreach (var d in destinations)
            {
                var entry1 = node1.DistanceVector.Entries.Where(x => x.Destination == d).FirstOrDefault();
                var entry2 = node2.DistanceVector.Entries.Where(x => x.Destination == d).FirstOrDefault();
                if (entry1 == null)
                {
                    isBetter1 = true;
                    mergedVector1.AddEntry(d, distance + entry2.Distance, node2);
                    mergedVector2.AddEntry(entry2);
                }
                else if (entry2 == null)
                {
                    isBetter2 = true;
                    mergedVector1.AddEntry(entry1);
                    mergedVector2.AddEntry(d, distance + entry1.Distance, node1);
                }
                else
                {
                    if (distance + entry2.Distance < entry1.Distance)
                    {
                        isBetter1 = true;
                        mergedVector1.AddEntry(d, distance + entry2.Distance, node2);
                    }
                    else
                    {
                        mergedVector1.AddEntry(entry1);
                    }
                    if(distance + entry1.Distance < entry2.Distance)
                    {
                        isBetter2 = true;
                        mergedVector2.AddEntry(d, distance + entry1.Distance, node1);
                    }
                    else
                    {
                        mergedVector2.AddEntry(entry2);
                    }
                }
            }
            if (isBetter1)
            {
                node1.DistanceVector = mergedVector1;
            }
            if(isBetter2)
            {
                node2.DistanceVector = mergedVector2;
            }
        }

        public IEnumerable<Node<TNode>> FindShortestPath(Node<TNode> from, Node<TNode> to)
        {
            var path = new List<Node<TNode>>();
            while(from != to)
            {
                path.Add(from);
                from = from.DistanceVector.Entries.Where(x => x.Destination == to).Select(x => x.Next).FirstOrDefault();
                if (from == null)
                    throw new PathDoesNotExistException("The specified path does not exist");
            }
            path.Add(to);
            return path;
        }
    }
}
