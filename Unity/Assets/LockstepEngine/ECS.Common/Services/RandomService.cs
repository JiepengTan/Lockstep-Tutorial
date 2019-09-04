using System;
using Lockstep.Logging;
using Lockstep.Math;
using Random = Lockstep.Math.Random;

namespace Lockstep.Game {
    public partial class RandomService : BaseService, IRandomService {
        Random _i = new Math.Random();
        public LFloat value => _i.value;

        public uint Next(){
            return _i.Next();
        }

        public uint Next(uint max){
            return _i.Next(max);
        }

        public int Next(int max){
            return _i.Next(max);
        }

        public uint Range(uint min, uint max){
            return _i.Range(min, max);
        }

        public int Range(int min, int max){
            return _i.Range(min, max);
        }

        public LFloat Range(LFloat min, LFloat max){
            return _i.Range(min, max);
        }


        public class RandomCmd : BaseCommand {
            public ulong randSeed;

            public override void Do(object param){
                randSeed = ((RandomService) param)._i.randSeed;
            }

            public override void Undo(object param){
                ((RandomService) param)._i.randSeed = randSeed;
            }
        }

        protected override FuncUndoCommands GetRollbackFunc(){
            return (minTickNode, maxTickNode, param) => { minTickNode.cmd.Undo(param); };
        }

        public override void Backup(int tick){
            Debug.Assert(tick == CurTick, $"CurTick{CurTick} tick {tick}");
            cmdBuffer.Execute(tick, new RandomCmd());
        }
    }
}