namespace BigDataToolkit.Collections
{
    using System;
    using System.Collections.Generic;

    public class SkipList<K, V> where K : IComparable<K>
    {
        private readonly IComparer<K> _keyComparer;

        private readonly Random _rnd = new Random(0);

        private Node _head;

        public SkipList()
                : this(Comparer<K>.Default)
        {
        }

        public SkipList(IComparer<K> comparer)
        {
            _keyComparer = comparer;
            _head = new Node(default(K), default(V), 1);
            _head.Levels[0] = null;
        }

        public int Level
        {
            get { return _head.Levels.Length; }
        }

        public void Add(K key, V value)
        {
            Node[] updates = FindUpdates(key);
            if (null != updates[0].Levels[0] && 0 == _keyComparer.Compare(updates[0].Levels[0].Key, key))
            {
                updates[0].Value = value;
                return;
            }

            // create and insert the node
            Node node = new Node(key, value, ChooseRandomHeight());
            int min = node.Levels.Length;
            if (min > updates.Length)
            {
                min = updates.Length;
            }

            for (int i = 0; i < min; ++i)
            {
                node.Levels[i] = updates[i].Levels[i];
                updates[i].Levels[i] = node;
            }

            // update head level
            if (node.Levels.Length > _head.Levels.Length)
            {
                Node newHead = new Node(_head.Key, _head.Value, node.Levels.Length);
                Array.Copy(_head.Levels, newHead.Levels, _head.Levels.Length);
                newHead.Levels[_head.Levels.Length] = node;
                _head = newHead;
            }
        }

        public bool Remove(K key)
        {
            Node[] updates = FindUpdates(key);
            Node node = updates[0].Levels[0];
            if (null == node || 0 != _keyComparer.Compare(node.Key, key))
            {
                return false;
            }

            for (int i = 0; i < updates.Length; ++i)
            {
                if (i < node.Levels.Length)
                {
                    updates[i].Levels[i] = node.Levels[i];
                }
            }

            if (1 < _head.Levels.Length && null == _head.Levels[_head.Levels.Length - 1])
            {
                Node newHead = new Node(_head.Key, _head.Value, _head.Levels.Length - 1);
                Array.Copy(_head.Levels, newHead.Levels, newHead.Levels.Length);
                _head = newHead;
            }

            return true;
        }

        public bool ContainsKey(K key)
        {
            Node current = _head;
            int level = current.Levels.Length - 1;
            while (0 <= level)
            {
                int cmp = -1;
                while (null != current.Levels[level] && (cmp = _keyComparer.Compare(current.Levels[level].Key, key)) < 0)
                {
                    current = current.Levels[level];
                }

                if (0 == cmp)
                {
                    return true;
                }

                --level;
            }

            return false;
        }

        private Node[] FindUpdates(K key)
        {
            Node[] updates = new Node[_head.Levels.Length];
            Node current = _head;
            int level = current.Levels.Length - 1;
            while (0 <= level)
            {
                while (null != current.Levels[level] && _keyComparer.Compare(current.Levels[level].Key, key) < 0)
                {
                    current = current.Levels[level];
                }

                updates[level] = current;
                --level;
            }
            return updates;
        }

        protected int ChooseRandomHeight()
        {
            int maxLevel = _head.Levels.Length;
            int level = 0;
            while (_rnd.NextDouble() < 0.5 && level < maxLevel)
            {
                level++;
            }

            return level + 1;
        }

        private class Node
        {
            public Node(K key, V value, int level)
            {
                Key = key;
                Value = value;
                Levels = new Node[level];
            }

            public K Key { get; private set; }

            public V Value { get; set; }

            public Node[] Levels { get; private set; }
        }
    }
}