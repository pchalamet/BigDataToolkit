namespace BigDataToolkit.Collections
{
    using System;

    public partial class SkipList<TK, TV> where TK : IComparable<TK>
    {
        private class Node
        {
            public TK Key { get; private set; }

            public TV Value { get; set; }

            public Node[] Next { get; private set; }

            private Node(TK key, TV value, int level)
            {
                Key = key;
                Value = value;
                Next = new Node[level];
            }

            public static Node Create(TK key, TV value, int level)
            {
                Node node = new Node(key, value, level);
                return node;
            }
        }
    }
}