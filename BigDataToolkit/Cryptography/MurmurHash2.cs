namespace BigDataToolkit.Cryptography
{
    //
    // adaptated from http://landman-code.blogspot.fr/2009/02/c-superfasthash-and-murmurhash2.html
    //
    using System;
    using System.Security.Cryptography;

    public class MurmurHash2 : HashAlgorithm
    {
        private const uint M = 0x5bd1e995;

        private const int R = 24;

        private const uint SEED = 0xc58f1a7b;

        private uint _h;

        public override void Initialize()
        {
            _h = SEED;
        }

        protected override unsafe void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (0 == cbSize)
            {
                return;
            }

            uint h = _h ^ (uint) cbSize;
            int remainingBytes = cbSize & 3; // mod 4
            int numberOfLoops = cbSize >> 2; // div 4
            fixed (byte* firstByte = & (array[ibStart]))
            {
                uint* realData = (uint*) firstByte;
                while (numberOfLoops != 0)
                {
                    uint k = *realData;
                    k *= M;
                    k ^= k >> R;
                    k *= M;

                    h *= M;
                    h ^= k;
                    numberOfLoops--;
                    realData++;
                }
                switch (remainingBytes)
                {
                    case 3:
                        h ^= (ushort) (*realData);
                        h ^= ((uint) (*(((byte*) (realData)) + 2))) << 16;
                        h *= M;
                        break;
                    case 2:
                        h ^= (ushort) (*realData);
                        h *= M;
                        break;
                    case 1:
                        h ^= *((byte*) realData);
                        h *= M;
                        break;
                }
            }

            // Do a few final mixes of the hash to ensure the last few
            // bytes are well-incorporated.

            h ^= h >> 13;
            h *= M;
            _h ^= h >> 15;
        }

        protected override byte[] HashFinal()
        {
            return BitConverter.GetBytes(_h);
        }
    }
}