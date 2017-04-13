using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoutingSimulator.Core
{
    public class DistanceVector<TNode> : IEquatable<DistanceVector<TNode>>
                           where TNode : IComparable<TNode>, IEquatable<TNode>
    {
        private ISet<DistanceVectorEntry<TNode>> _entries;
        public Node<TNode> Source { get; private set; }
        public IEnumerable<DistanceVectorEntry<TNode>> Entries
        {
            get
            {
                return _entries;
            }
        }

        public DistanceVector(Node<TNode> source)
        {
            Source = source;
            _entries = new HashSet<DistanceVectorEntry<TNode>>();
            _entries.Add(new DistanceVectorEntry<TNode>(source, 0, source));
        }

        public void AddEntry(Node<TNode> dst, long dist, Node<TNode> next)
        {
            AddEntry(new DistanceVectorEntry<TNode>(dst, dist, next));
        }

        public void AddEntry(DistanceVectorEntry<TNode> entry)
        {
            var e = _entries.Where(x => x.Destination == entry.Destination).FirstOrDefault();
            if (e != null)
                _entries.Remove(e);
            _entries.Add(entry);
        }

        public void RemoveNode(Node<TNode> dst)
        {
            _entries.RemoveWhere(x => x.Destination == dst || x.Next == dst);
        }

        public bool Equals(DistanceVector<TNode> other)
        {
            var entries = other.Entries.ToList();
            if (_entries.Count != entries.Count)
                return false;
            foreach(var entry in _entries)
            {
                var row = entries.Where(x => x.Destination == entry.Destination).FirstOrDefault();
                if (row == null)
                    return false;
                if (row.Distance != entry.Distance)
                    return false;
                if (row.Next != entry.Next)
                    return false;
            }
            return true;
        }
    }

    public class DistanceVectorEntry<TNode> : IEquatable<DistanceVectorEntry<TNode>>
                                 where TNode: IComparable<TNode>, IEquatable<TNode>
    {
        public Node<TNode> Destination { get; private set; }
        public long Distance { get; private set; }
        public Node<TNode> Next { get; private set; }

        public DistanceVectorEntry(Node<TNode> dst, long dist, Node<TNode> next)
        {
            Destination = dst;
            Distance = dist;
            Next = next;
        }

        public bool Equals(DistanceVectorEntry<TNode> other)
        {
            return Destination == other.Destination;
        }

        public override string ToString()
        {
            return Destination + "<--" + Next + ":" + Distance;
        }
    }
}
