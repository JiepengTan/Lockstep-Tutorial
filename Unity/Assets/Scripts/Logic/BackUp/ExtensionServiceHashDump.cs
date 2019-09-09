using System.Text;
using Lockstep.Collision2D;
using Lockstep.Game;
using Lockstep.Math;
using Lockstep.Util;

namespace Lockstep.Game {
    public partial class GameStateService {
        public override int GetHash(ref int idx){
            int hash = 1;
            foreach (var entity in GetPlayers()) {
                hash += entity.curHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
                hash += entity.transform.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
                hash += entity.skillBox.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            }

            //_debugService.Trace($"GetPlayers hash {hash}",true);
            foreach (var entity in GetEnemies()) {
                hash += entity.curHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
                hash += entity.transform.GetHash(ref idx)  * PrimerLUT.GetPrimer(idx++);
            }
            //_debugService.Trace($"GetEnemies hash {hash}",true);

            foreach (var entity in GetSpawners()) {
                hash += entity.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            }
            //_debugService.Trace($"GetSpawners hash {hash}",true);

            hash += _curGameState.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //_debugService.Trace($"GetHash hash {hash}",true);
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix){
            sb.AppendLine("Hash ------: " +_commonStateService.Hash);
            BackUpUtil.DumpList("GetPlayers", GetPlayers(), sb, prefix);
            BackUpUtil.DumpList("GetEnemies", GetEnemies(), sb, prefix);
            BackUpUtil.DumpList("GetSpawners", GetSpawners(), sb, prefix);
            sb.AppendLine(prefix + "EntityId" + ":" + _curGameState.ToString());
        }
    }

    public partial class IdService : IHashCode, IDumpStr {
        public int GetHash(ref int idx){
            return Id * PrimerLUT.GetPrimer(idx++);
        }

        public void DumpStr(System.Text.StringBuilder sb, string prefix){
            sb.AppendLine(prefix + "Id" + ":" + Id.ToString());
        }
    }

    public partial class RandomService {
        public override int GetHash(ref int idx){
            return (int) _i.randSeed * PrimerLUT.GetPrimer(idx++);
        }
        public override void DumpStr(System.Text.StringBuilder sb, string prefix){
            sb.AppendLine(prefix + "randSeed" + ":" + _i.randSeed.ToString());
        }
    }
}