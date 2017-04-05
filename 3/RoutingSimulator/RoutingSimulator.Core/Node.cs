using System;
using System.Linq;

namespace RoutingSimulator.Core
{
    public class Node<T> : IEquatable<Node<T>>, IComparable<Node<T>>
                 where T : IComparable<T>, IEquatable<T>
    {
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
                DistanceVectorUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler DistanceVectorUpdated;

        public Node(T value)
        {
            Value = value;
            _distanceVector = new DistanceVector<T>(this);
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
