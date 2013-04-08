namespace BigDataToolkit.Collections
{
    using System;

    public partial class SkipList<TK, TV> where TK : IComparable<TK>
    {
        private class Node
        {
            private readonly Node[] _nodes;

            private Node(TK key, TV value, int level)
            {
                Key = key;
                Value = value;
                _nodes = new Node[level];
            }

            public int NbLevels
            {
                get { return _nodes.Length; }
            }

            public TK Key { get; private set; }

            public TV Value { get; set; }

            public Node this[int level]
            {
                get { return _nodes[level]; }
                set { _nodes[level] = value; }
            }

            public static Node Create(TK key, TV value, int level)
            {
                Node node = new Node(key, value, level);
                return node;
            }
        }
    }
}