using System;
using System.Collections.Generic;
using Lockstep.Math;

namespace Lockstep.Game {
    public class DelayCallService : BaseService {
        private int _startIdx;
        private readonly List<Action> actions = new List<Action>();
        private int _actionId =0;
        public int RegisterFunc(int delayMs, Action action){
            
            var id = _actionId++;
            var idx = id - _startIdx;
            if (idx >= actions.Count) {
                actions.Add(action);
            }
            else {
                actions[idx] = action;
            }
            return id;
        }

        public void Call(int id){
            var func =  actions[id - _startIdx];
            func.Invoke();
        }

        /// <summary>
        /// 移除掉 id <= leId 的所有 action
        /// </summary>
        /// <param name="leId"></param>
        public void RemoveFunc(int leId){ }
        
        public class FuncCmd : BaseCommand {
            public int randSeed;

            public override void Do(object param){
                randSeed = ((DelayCallService) param)._actionId;
            }

            public override void Undo(object param){
                ((DelayCallService) param)._actionId = randSeed;
            }
        }

        public override void Backup(int tick){
            cmdBuffer.Execute(tick, new FuncCmd());
        }
    }

    public static class FuncUtil {
        private static int _startIdx;
        private static readonly List<Action> actions = new List<Action>();
        private static int _actionId =0;
        
        public static int RegisterFunc( Action action){
            var id = _actionId++;
            var idx = id - _startIdx;
            if (idx >= actions.Count) {
                actions.Add(action);
            }
            else {
                actions[idx] = action;
            }
            return id;
        }
        public static void Call(Action func){
            func?.Invoke();
        }

        public static void Call(int id){
            var func =  actions[id - _startIdx];
            func.Invoke();
        }

        /// <summary>
        /// 移除掉 id <= leId 的所有 action
        /// </summary>
        /// <param name="leId"></param>
        public static void RemoveFunc(int leId){
            var len = leId - _startIdx +1;
            if(len <=0) return;
            actions.RemoveRange(0,len);
            _startIdx += len;
        }
    }
}