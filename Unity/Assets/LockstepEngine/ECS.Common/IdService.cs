using System.Collections.Generic;
using Lockstep.Math;

namespace Lockstep.Game {
    public class IdService : IIdService, IHashCode, ITimeMachine {
        public int CurTick { get; set; }

        private int Id;

        public int GenId(){
            return Id++;
        }

        Dictionary<int, int> _tick2Id = new Dictionary<int, int>();

        public void RollbackTo(int tick){
            Id = _tick2Id[tick];
        }

        public void Backup(int tick){
            _tick2Id[tick] = Id;
        }

        public void Clean(int maxVerifiedTick){ }

        public int GetHash(ref int idx){
            return Id * PrimerLUT.GetPrimer(idx++);
        }
        public void DumpStr(System.Text.StringBuilder sb,string prefix){                        
            sb.AppendLine(prefix + "Id"+":" + Id.ToString());
        }
    }
}