using System;
using Lockstep.Math;

namespace Lockstep.Math {
    public partial class Random {
        public ulong randSeed = 1;
        public Random(uint seed = 17){
            randSeed = seed;
        }
        public LFloat value => new LFloat(true, Range(0, 1000));

        public uint Next(){
            randSeed = randSeed * 1103515245 + 36153;
            return (uint) (randSeed / 65536);
        }
        // range:[0 ~(max-1)]
        public uint Next(uint max){
            return Next() % max;
        }
        public int Next(int max){
            return (int) (Next() % max);
        }
        // range:[min~(max-1)]
        public uint Range(uint min, uint max){
            if (min > max)
                throw new ArgumentOutOfRangeException("minValue",
                    string.Format("'{0}' cannot be greater than {1}.", min, max));

            uint num = max - min;
            return this.Next(num) + min;
        }
        public int Range(int min, int max){
            if (min >= max - 1)
                return min;
            int num = max - min;

            return this.Next(num) + min;
        }

        public LFloat Range(LFloat min, LFloat max){
            if (min > max)
                throw new ArgumentOutOfRangeException("minValue",
                    string.Format("'{0}' cannot be greater than {1}.", min, max));

            uint num = (uint) (max._val - min._val);
            return new LFloat(true, Next(num) + min._val);
        }
    }
#if false
    public class LRandom {
        private static Random _i = new Random(3274);
        public static LFloat value => _i.value;
        public static uint Next(){return _i.Next();}
        public static uint Next(uint max){return _i.Next(max);}
        public static int Next(int max){return _i.Next(max);}
        public static uint Range(uint min, uint max){return _i.Range(min, max);}
        public static int Range(int min, int max){return _i.Range(min, max);}
        public static LFloat Range(LFloat min, LFloat max){return _i.Range(min, max);}
    }
#endif
}