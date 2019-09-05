using System;
using System.Collections;

namespace Lockstep.Util {
     public partial class Random {
        public ulong randSeed = 1;
        public Random(uint seed = 17){
            randSeed = seed;
        }

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

      
    }
    public class LRandom {
        private static Random _i = new Random(3274);

        public static void SetSeed(uint seed){
            _i = new Random(seed);
        }
        public static uint Next(){return _i.Next();}
        public static uint Next(uint max){return _i.Next(max);}
        public static int Next(int max){return _i.Next(max);}
        public static uint Range(uint min, uint max){return _i.Range(min, max);}
        public static int Range(int min, int max){return _i.Range(min, max);}
    }


    public class LTime {
        /// The total number of frames that have passed (Read Only).
        public static int frameCount { get; private set; }

        /// The time in seconds it took to complete the last frame (Read Only).
        public static float deltaTime { get; private set; }

        /// The time this frame has started (Read Only). This is the time in seconds since the last level has been loaded.
        public static float timeSinceLevelLoad { get; private set; }

        /// The real time in seconds since the game started (Read Only).
        public static float realtimeSinceStartup => (float) (DateTime.Now - _initTime).TotalSeconds;
        public static long realtimeSinceStartupMS =>  (long)(DateTime.Now - _initTime).TotalMilliseconds;

        private static DateTime _initTime;
        private static DateTime lastFrameTime;

        public static void DoStart(){
            _initTime = DateTime.Now;
        }

        public static void DoUpdate(){
            var now = DateTime.Now;
            deltaTime = (float) ((now - lastFrameTime).TotalSeconds);
            timeSinceLevelLoad = (float) ((now - _initTime).TotalSeconds);
            frameCount++;
            lastFrameTime = now;
        }
    }
}