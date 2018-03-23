namespace BigDataToolkit.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class SkipList<TK, TV> where TK : IComparable<TK>
    {
        private readonly IComparer<TK> _keyComparer;

        private readonly Random _rnd = new Random(0);

        private int _levels;

        private Node _head;

        public SkipList()
                : this(Comparer<TK>.Default)
        {
        }

        public SkipList(IComparer<TK> comparer)
        {
            _keyComparer = comparer;
            _head = Node.Create(default(TK), default(TV), 33);
            _levels = 1;
        }

        public void Add(TK key, TV value)
        {
            var level = 0;
            var r = _rnd.Next();
            while (0 != (r & 1))
            {
                ++level;
                r >>= 1;
            }
            if(level == _levels) 
                _levels++;

            // Insert this node into the skip list
            var newNode = Node.Create(key, value, level + 1);
            var cur = _head;
            for (var i = _levels - 1; 0 <= i; --i)
            {
                while (null != cur.Next[i])
                {
                    var cmp = _keyComparer.Compare(cur.Next[i].Key, key);
                    if (0 < cmp)
                    {
                        break;
                    }

                    cur = cur.Next[i];
                }

                if (i <= level) 
                { 
                    newNode.Next[i] = cur.Next[i]; 
                    cur.Next[i] = newNode; 
                }
            }
        }

        public bool Remove(TK key)
        {
            var cur = _head;
            var found = false;
            for (var i = _levels - 1; 0 <= i; --i)
            {
                while (null != cur.Next[i])
                {
                    var cmp = _keyComparer.Compare(cur.Next[i].Key, key);
                    if (0 < cmp)
                    {
                        break;
                    }

                    if (0 == cmp)
                    {
                        found = true;
                        cur.Next[i] = cur.Next[i].Next[i];
                        break;
                    }

                    cur = cur.Next[i];
                }
            }

            return found;
        }

        public bool TryGetValue(TK key, out TV value)
        {
            var cur = _head;
            for (var i = _levels - 1; 0 <= i; --i)
            {
                while (null != cur.Next[i])
                {
                    var cmp = _keyComparer.Compare(cur.Next[i].Key, key);
                    if (0 < cmp) 
                    {
                        break;
                    }

                    if (0 == cmp)
                    {
                        value = cur.Next[i].Value; 
                        return true;
                    }

                    cur = cur.Next[i];
                }
            }

            value = default(TV);
            return false;
        }
    }
}