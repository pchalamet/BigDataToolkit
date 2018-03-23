namespace BigDataToolkit.Collections.Specialized
{
    using System;
    using System.Collections;
    using System.Security.Cryptography;

    public class BloomFilter
    {
        private readonly HashAlgorithm _hashFunc;

        private readonly BitArray _hashbits;

        public BloomFilter(int expectedNumberOfElements, double p, HashAlgorithm hashfunc)
        {
            double logp = Math.Log(p);
            double log2 = Math.Log(2);
            double bitsPerElement = -logp / (log2 * log2);
            int bitSetSize = (int) Math.Ceiling(bitsPerElement * expectedNumberOfElements);
            int k = (int) Math.Ceiling(-logp / log2);

            ExpectedNumberOfElements = expectedNumberOfElements;
            FalsePositiveProbability = p;
            NumberOfHashFunctions = k;
            BitPerElements = bitsPerElement;
            Size = bitSetSize;
            _hashbits = new BitArray(bitSetSize);
            _hashFunc = hashfunc;
        }

        public double BitPerElements { get; private set; }

        public int NumberOfHashFunctions { get; private set; }

        public int ExpectedNumberOfElements { get; private set; }

        public double FalsePositiveProbability { get; private set; }

        public int Size { get; private set; }

        public int Count { get; private set; }

        public void Clear()
        {
            _hashbits.SetAll(false);
            Count = 0;
        }

        public bool Contains(byte[] buffer)
        {
            int[] hashKeys = CreateHashes(buffer);
            foreach (int hash in hashKeys)
            {
                int bitOffset = Math.Abs(hash % Size);
                if (!_hashbits.Get(bitOffset))
                {
                    return false;
                }
            }

            return true;
        }

        public bool Add(byte[] buffer)
        {
            bool exists = true;
            int[] hashKeys = CreateHashes(buffer);
            foreach (int hash in hashKeys)
            {
                int bitOffset = Math.Abs(hash % Size);
                exists &= _hashbits.Get(bitOffset);

                _hashbits.Set(bitOffset, true);
            }

            ++Count;

            return exists;
        }

        private int[] CreateHashes(byte[] data)
        {
            int[] result = new int[NumberOfHashFunctions];
            int k = 0;
            while (k < NumberOfHashFunctions)
            {
                // compute a new hash
                byte[] salt = BitConverter.GetBytes(k);
                _hashFunc.Initialize();
                _hashFunc.TransformBlock(salt, 0, salt.Length, null, 0);
                _hashFunc.TransformFinalBlock(data, 0, data.Length);
                byte[] digest = _hashFunc.Hash;

                for (int i = 0; i < digest.Length / 4 && k < NumberOfHashFunctions; i++)
                {
                    result[k] = BitConverter.ToInt32(digest, i * 4);
                    ++k;
                }
            }

            return result;
        }
    }
}