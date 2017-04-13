using System;
using System.Collections.Generic;
using System.Linq;

namespace RoutingSimulator.Core
{
    public class Node<T> : IEquatable<Node<T>>, IComparable<Node<T>>
                 where T : IComparable<T>, IEquatable<T>
    {
        private ISet<Node<T>> _neighbours;
        private IDictionary<Node<T>, long> _distances;
        private IDictionary<Node<T>, DistanceVector<T>> _neighbourDistanceVectors;

        public IEnumerable<Node<T>> Neighbours
        {
            get
            {
                return _neighbours;
            }
        }


        private DistanceVector<T> _distanceVector;
        public T Value { get; private set; }
        public DistanceVector<T> DistanceVector
        {
            get
            {
                return _distanceVector;
            }
            set
            {
                _distanceVector = value;
                //DistanceVectorUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler DistanceVectorUpdated;

        public Node(T value)
        {
            Value = value;
            _distanceVector = new DistanceVector<T>(this);
            _neighbours = new HashSet<Node<T>>();
            _distances = new Dictionary<Node<T>, long>();
            _neighbourDistanceVectors = new Dictionary<Node<T>, DistanceVector<T>>();
        }

        public void AddNeighbour(Node<T> node, long distance)
        {
            _neighbours.Add(node);
            _distances.Add(node, distance);
            _neighbourDistanceVectors.Add(node, node.DistanceVector);
            node.DistanceVectorUpdated += (s, e) =>
            {
                _neighbourDistanceVectors[node] = node.DistanceVector;
                UpdateDistanceVector();
            };
            UpdateDistanceVector();
        }

        public void RemoveNeighbour(Node<T> node)
        {
            if (!_neighbours.Contains(node))
                return;
            _neighbours.Remove(node);
            _distances.Remove(node);
            _neighbourDistanceVectors.Remove(node);
            var badEntries = _distanceVector.Entries.Where(x => x.Next == node);
            if(badEntries.Count() != 0)
            {
                _distanceVector.RemoveNode(node);
                UpdateDistanceVector();
            }
        }

        private void UpdateDistanceVector()
        {
            var distanceVector = new DistanceVector<T>(this);
            var destinations = _neighbours.SelectMany(
                                x => x.DistanceVector
                                      .Entries
                                      .Select(y => y.Destination)
                                ).Where(x => x != this).Distinct().ToList();
            foreach(var destination in destinations)
            {
                long bestDist = long.MaxValue;
                Node<T> next = null;
                foreach(var neighbour in _neighbours)
                {
                    long distToNeigh = _distances[neighbour];
                    var distToDest = neighbour.DistanceVector
                                               .Entries
                                               .Where(x => x.Destination == destination)
                                               .Where(x => x.Next != this)
                                               .Select(x=>x.Distance);
                    if (distToDest.Count() == 0)
                        continue;
                    if (distToNeigh + distToDest.First() < bestDist)
                    {
                        bestDist = distToNeigh + distToDest.First();
                        next = neighbour;
                    }
                }
                if(next != null)
                    distanceVector.AddEntry(destination, bestDist, next);
            }
            if(!_distanceVector.Equals(distanceVector))
            {
                _distanceVector = distanceVector;
                DistanceVectorUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public void MergeDistanceVector(Node<T> node, long distance)
        {
            var mergedVector = new DistanceVector<T>(this);
            var destinations = DistanceVector.Entries
                                             .Select(x => x.Destination)
                                             .Union(node.DistanceVector
                                                        .Entries
                                                        .Select(x => x.Destination));
            bool isBetter = false;
            foreach(var d in destinations)
            {
                var local = DistanceVector.Entries.Where(x => x.Destination == d).FirstOrDefault();
                var other = node.DistanceVector.Entries.Where(x => x.Destination == d).FirstOrDefault();
                if(local == null) // add a completely new entry
                {
                    isBetter = true;
                    DistanceVector.AddEntry(d, distance + other.Distance, node);
                }
                else if(other == null)
                {
                    // nothing to update locally
                }
                else
                {
                    if(distance + other.Distance < local.Distance)
                    {
                        isBetter = true;
                        DistanceVector.AddEntry(d, distance + other.Distance, node);
                    }
                }
            }
            if (isBetter)
            {
                DistanceVector = mergedVector;
            }
        }

        public int CompareTo(Node<T> other)
        {
            return Value.CompareTo(other.Value);
        }

        public bool Equals(Node<T> other)
        {
            return Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
