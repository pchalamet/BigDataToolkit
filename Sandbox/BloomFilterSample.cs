namespace Sandbox
{
    using System;
    using System.Collections.Generic;
    using BigDataToolkit.Collections;
    using BigDataToolkit.Collections.Specialized;
    using BigDataToolkit.Cryptography;

    internal class BloomFilterSample
    {
        public static void Test()
        {
            BloomFilter bloomFilter = new BloomFilter(1000000, 0.1, new MurmurHash3());
            Console.WriteLine("Capacity = {0}", bloomFilter.ExpectedNumberOfElements);
            Console.WriteLine("FalsePositiveProbability = {0}", bloomFilter.FalsePositiveProbability);
            Console.WriteLine("BitPerElements = {0}", bloomFilter.BitPerElements);
            Console.WriteLine("K = {0}", bloomFilter.NumberOfHashFunctions);
            Console.WriteLine("Size = {0}", bloomFilter.Size);

            Random rnd = new Random(0);
            int max = 100000000;
            HashSet<int> elements = new HashSet<int>();
            for (int i = 0; i < 5000000; ++i)
            {
                int next = rnd.Next(max);
                if (!elements.Contains(next))
                {
                    elements.Add(next);
                }

                byte[] data = BitConverter.GetBytes(next);
                bloomFilter.Add(data);
            }
            Console.WriteLine("Count = {0}", bloomFilter.Count);

            int founds = 0;
            for (int i = 0; i < max; ++i)
            {
                if (0 == (i % 1000))
                {
                    Console.Write(i);
                    Console.Write("\r");
                }

                byte[] data = BitConverter.GetBytes(i);
                bool found = bloomFilter.Contains(data);
                if (found)
                {
                    ++founds;
                }
            }

            Console.WriteLine("Founds {0}", founds);
            Console.WriteLine("Elements = {0}", elements.Count);
            Console.WriteLine("Probability = {0}", founds / (double) max);
        }
    }
}