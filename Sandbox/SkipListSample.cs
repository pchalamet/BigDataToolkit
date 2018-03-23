namespace Sandbox
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using BigDataToolkit.Collections;

    internal class SkipListSample
    {
        private const int MAX = 500000;

        private static void TestSkipList()
        {
            Console.WriteLine("=========== TestSkipList");

            SkipList<int, string> skipList = new SkipList<int, string>();
            Random rnd = new Random(0);

            Console.WriteLine("Add");
            Stopwatch swAdd = Stopwatch.StartNew();
            for (int i = 0; i < MAX; ++i)
            {
                Console.Write("{0}\r", i);

                int next = rnd.Next();
                skipList.Add(next, next.ToString());
            }
            swAdd.Stop();

            rnd = new Random(0);
            Console.WriteLine("Remove");
            Stopwatch swRemove = Stopwatch.StartNew();
            for (int i = 0; i < MAX; ++i)
            {
                Console.Write("{0}\r", i);

                int next = rnd.Next();
                skipList.Remove(next);
            }
            swRemove.Stop();

            rnd = new Random(0);
            Console.WriteLine("ContainsKey");
            Stopwatch swContainsKey = Stopwatch.StartNew();
            for (int i = 0; i < MAX; ++i)
            {
                Console.Write("{0}\r", i);

                int next = rnd.Next();
                if (skipList.TryGetValue(next, out var value)) {
                    Console.WriteLine("Bug");
                }
            }
            swContainsKey.Stop();

            Console.WriteLine("Add = {0}", swAdd.ElapsedMilliseconds);
            Console.WriteLine("Remove = {0}", swRemove.ElapsedMilliseconds);
            Console.WriteLine("ContainsKey = {0}", swContainsKey.ElapsedMilliseconds);

            //skipList.Add(10, "tralala");
            //skipList.Add(20, "tutu");
            //skipList.Add(20, "pouet");
            //Console.WriteLine("Contains 10 = {0}", skipList.ContainsKey(10));
            //Console.WriteLine("Contains 20 = {0}", skipList.ContainsKey(20));
            //Console.WriteLine("Remove 20 = {0}", skipList.Remove(20));
            //Console.WriteLine("Remove 10 = {0}", skipList.Remove(10));
            //Console.WriteLine("Remove 20 = {0}", skipList.Remove(20));
            //Console.WriteLine("Contains 20 = {0}", skipList.ContainsKey(20));
            //Console.WriteLine("Contains 10 = {0}", skipList.ContainsKey(10));
        }

        private static void TestSortedSet()
        {
            Console.WriteLine("=========== TestSortedSet");

            SortedSet<int> skipList = new SortedSet<int>();
            Random rnd = new Random(0);

            Console.WriteLine("Add");
            Stopwatch swAdd = Stopwatch.StartNew();
            for (int i = 0; i < MAX; ++i)
            {
                Console.Write("{0}\r", i);

                int next = rnd.Next();
                skipList.Add(next);
            }
            swAdd.Stop();

            rnd = new Random(0);
            Console.WriteLine("Remove");
            Stopwatch swRemove = Stopwatch.StartNew();
            for (int i = 0; i < MAX; ++i)
            {
                Console.Write("{0}\r", i);

                int next = rnd.Next();
                skipList.Remove(next);
            }
            swRemove.Stop();

            rnd = new Random(0);
            Console.WriteLine("ContainsKey");
            Stopwatch swContainsKey = Stopwatch.StartNew();
            for (int i = 0; i < MAX; ++i)
            {
                Console.Write("{0}\r", i);

                int next = rnd.Next();
                bool res = skipList.Contains(next);
                if (res)
                {
                    Console.WriteLine("Bug");
                }
            }
            swContainsKey.Stop();

            Console.WriteLine("Add = {0}", swAdd.ElapsedMilliseconds);
            Console.WriteLine("Remove = {0}", swRemove.ElapsedMilliseconds);
            Console.WriteLine("ContainsKey = {0}", swContainsKey.ElapsedMilliseconds);
        }

        public static void Test()
        {
            TestSkipList();
            TestSortedSet();
        }
    }
}