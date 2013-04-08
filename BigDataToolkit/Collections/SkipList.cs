namespace BigDataToolkit.Collections
{
    using System;
    using System.Collections.Generic;

    public partial class SkipList<TK, TV> where TK : IComparable<TK>
    {
        private readonly IComparer<TK> _keyComparer;

        private readonly Random _rnd = new Random(0);

        private Node _head;

        public SkipList()
                : this(Comparer<TK>.Default)
        {
        }

        public SkipList(IComparer<TK> comparer)
        {
            _keyComparer = comparer;
            _head = Node.Create(default(TK), default(TV), 1);
        }

        public void Add(TK key, TV value)
        {
            Node[] updates = FindUpdates(key);
            if (null != updates[0][0] && 0 == _keyComparer.Compare(updates[0][0].Key, key))
            {
                updates[0].Value = value;
                return;
            }

            // create and insert the node
            Node node = Node.Create(key, value, ChooseRandomHeight());
            int min = node.NbLevels;
            if (updates.Length < min)
            {
                min = updates.Length;
            }

            for (int i = 0; i < min; ++i)
            {
                node[i] = updates[i][i];
                updates[i][i] = node;
            }

            // update head level
            if (node.NbLevels > _head.NbLevels)
            {
                Node newHead = Node.Create(_head.Key, _head.Value, node.NbLevels);
                for (int i = 0; i < _head.NbLevels; ++i)
                {
                    newHead[i] = _head[i];
                }
                newHead[_head.NbLevels] = node;
                _head = newHead;
            }
        }

        public bool Remove(TK key)
        {
            Node[] updates = FindUpdates(key);
            Node node = updates[0][0];
            if (null == node || 0 != _keyComparer.Compare(node.Key, key))
            {
                return false;
            }

            for (int i = 0; i < updates.Length; ++i)
            {
                if (i < node.NbLevels)
                {
                    updates[i][i] = node[i];
                }
            }

            if (1 < _head.NbLevels && null == _head[_head.NbLevels - 1])
            {
                Node newHead = Node.Create(_head.Key, _head.Value, _head.NbLevels - 1);
                for (int i = 0; i < _head.NbLevels - 1; ++i)
                {
                    newHead[i] = _head[i];
                }
                _head = newHead;
            }

            return true;
        }

        public bool ContainsKey(TK key)
        {
            Node current = _head;
            int level = current.NbLevels - 1;
            while (0 <= level)
            {
                int cmp = -1;
                while (null != current[level] && (cmp = _keyComparer.Compare(current[level].Key, key)) < 0)
                {
                    current = current[level];
                }

                if (0 == cmp)
                {
                    return true;
                }

                --level;
            }

            return false;
        }

        private Node[] FindUpdates(TK key)
        {
            Node[] updates = new Node[_head.NbLevels];
            Node current = _head;
            int level = current.NbLevels - 1;
            while (0 <= level)
            {
                while (null != current[level] && _keyComparer.Compare(current[level].Key, key) < 0)
                {
                    current = current[level];
                }

                updates[level] = current;
                --level;
            }
            return updates;
        }

        protected int ChooseRandomHeight()
        {
            int maxLevel = _head.NbLevels;
            int level = 0;
            while (_rnd.NextDouble() < 0.5 && level < maxLevel)
            {
                level++;
            }

            return level + 1;
        }
    }
}