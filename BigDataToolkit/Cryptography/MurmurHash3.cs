namespace BigDataToolkit.Cryptography
{
    //
    // adapted from http://blog.teamleadnet.com/2012/08/murmurhash3-ultra-fast-hash-algorithm.html
    //
    using System;
    using System.Security.Cryptography;

    public class MurmurHash3 : HashAlgorithm
    {
        // 128 bit output, 64 bit platform version

        public const int READ_SIZE = 16;

        private const ulong C1 = 0x87c37b91114253d5L;

        private const ulong C2 = 0x4cf5ad432745937fL;

        private const uint SEED = 0; // if want to start with a seed, create a constructor

        private ulong _h1;

        private ulong _h2;

        private ulong _length;

        private static ulong RotateLeft(ulong original, int bits)
        {
            return (original << bits) | (original >> (64 - bits));
        }

        private static unsafe ulong GetUInt64(byte[] bb, int pos)
        {
            // we only read aligned longs, so a simple casting is enough
            fixed (byte* pbyte = & bb[pos])
            {
                return *((ulong*) pbyte);
            }
        }

        private void MixBody(ulong k1, ulong k2)
        {
            _h1 ^= MixKey1(k1);

            _h1 = RotateLeft(_h1, 27);
            _h1 += _h2;
            _h1 = _h1 * 5 + 0x52dce729;

            _h2 ^= MixKey2(k2);

            _h2 = RotateLeft(_h2, 31);
            _h2 += _h1;
            _h2 = _h2 * 5 + 0x38495ab5;
        }

        private static ulong MixKey1(ulong k1)
        {
            k1 *= C1;
            k1 = RotateLeft(k1, 31);
            k1 *= C2;
            return k1;
        }

        private static ulong MixKey2(ulong k2)
        {
            k2 *= C2;
            k2 = RotateLeft(k2, 33);
            k2 *= C1;
            return k2;
        }

        private static ulong MixFinal(ulong k)
        {
            // avalanche bits
            k ^= k >> 33;
            k *= 0xff51afd7ed558ccdL;
            k ^= k >> 33;
            k *= 0xc4ceb9fe1a85ec53L;
            k ^= k >> 33;
            return k;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            int pos = ibStart;
            int remaining = cbSize;

            // read 128 bits, 16 bytes, 2 longs in each cycle
            while (remaining >= READ_SIZE)
            {
                ulong k1 = GetUInt64(array, pos);
                pos += 8;

                ulong k2 = GetUInt64(array, pos);
                pos += 8;

                _length += READ_SIZE;
                remaining -= READ_SIZE;

                MixBody(k1, k2);
            }

            // if the input MOD 16 != 0
            if (remaining > 0)
            {
                ProcessBytesRemaining(array, remaining, pos);
            }
        }

        private void ProcessBytesRemaining(byte[] bb, int remaining, int pos)
        {
            ulong k1 = 0;
            ulong k2 = 0;
            _length += (ulong) remaining;

            // little endian (x86) processing
            switch (remaining)
            {
                case 15:
                    k2 ^= (ulong) bb[pos + 14] << 48; // fall through
                    goto case 14;
                case 14:
                    k2 ^= (ulong) bb[pos + 13] << 40; // fall through
                    goto case 13;
                case 13:
                    k2 ^= (ulong) bb[pos + 12] << 32; // fall through
                    goto case 12;
                case 12:
                    k2 ^= (ulong) bb[pos + 11] << 24; // fall through
                    goto case 11;
                case 11:
                    k2 ^= (ulong) bb[pos + 10] << 16; // fall through
                    goto case 10;
                case 10:
                    k2 ^= (ulong) bb[pos + 9] << 8; // fall through
                    goto case 9;
                case 9:
                    k2 ^= bb[pos + 8]; // fall through
                    goto case 8;
                case 8:
                    k1 ^= GetUInt64(bb, pos);
                    break;
                case 7:
                    k1 ^= (ulong) bb[pos + 6] << 48; // fall through
                    goto case 6;
                case 6:
                    k1 ^= (ulong) bb[pos + 5] << 40; // fall through
                    goto case 5;
                case 5:
                    k1 ^= (ulong) bb[pos + 4] << 32; // fall through
                    goto case 4;
                case 4:
                    k1 ^= (ulong) bb[pos + 3] << 24; // fall through
                    goto case 3;
                case 3:
                    k1 ^= (ulong) bb[pos + 2] << 16; // fall through
                    goto case 2;
                case 2:
                    k1 ^= (ulong) bb[pos + 1] << 8; // fall through
                    goto case 1;
                case 1:
                    k1 ^= bb[pos]; // fall through
                    break;
                default:
                    throw new Exception("Something went wrong with remaining bytes calculation.");
            }

            _h1 ^= MixKey1(k1);
            _h2 ^= MixKey2(k2);
        }

        public override void Initialize()
        {
            _h1 = SEED;
            _h2 = 0;
            _length = 0;
        }

        protected override byte[] HashFinal()
        {
            _h1 ^= _length;
            _h2 ^= _length;

            _h1 += _h2;
            _h2 += _h1;

            _h1 = MixFinal(_h1);
            _h2 = MixFinal(_h2);

            _h1 += _h2;
            _h2 += _h1;

            var hash = new byte[READ_SIZE];

            Array.Copy(BitConverter.GetBytes(_h1), 0, hash, 0, 8);
            Array.Copy(BitConverter.GetBytes(_h2), 0, hash, 8, 8);

            return hash;
        }
    }
}