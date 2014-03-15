using Common.Net;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WvsGame.Maple.Characters
{
    public class CharacterRandom
    {

        [NonSerialized]
        private long seed1, seed2, seed3;

        public CharacterRandom()
        {
            const int v4 = 5;
            this.CRand32__Seed(Randomizer.NextLong(), unchecked(1170746341 * v4 - 755606699), unchecked(1170746341 * v4 - 755606699));
        }

        public void CRand32__Seed(long s1, long s2, long s3)
        {
            seed1 = s1 | 0x100000;
            seed2 = s2 | 0x1000;
            seed3 = s3 | 0x10;
        }

        public long CRand32__Random()
        {
            long v8 = ((this.seed1 & 0xFFFFFFFE) << 12) ^ ((this.seed1 & 0x7FFC0 ^ (this.seed1 >> 13)) >> 6);
            long v9 = 16 * (this.seed2 & 0xFFFFFFF8) ^ (((this.seed2 >> 2) ^ this.seed2 & 0x3F800000) >> 23);
            long v10 = ((this.seed3 & 0xFFFFFFF0) << 17) ^ (((this.seed3 >> 3) ^ this.seed3 & 0x1FFFFF00) >> 8);
            return (v8 ^ v9 ^ v10) & 0xffffffffL;
        }

        public void Append(Packet p)
        {
            long v5 = CRand32__Random();
            long s2 = CRand32__Random();
            long v6 = CRand32__Random();

            CRand32__Seed(v5, s2, v6);

            p.WriteInt((int)v5);
            p.WriteInt((int)s2);
            p.WriteInt((int)v6);
        }
    }
}
