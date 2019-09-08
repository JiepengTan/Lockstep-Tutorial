using System.Text;
using Lockstep.Game;
using Lockstep.Logging;
using Lockstep.Math;

namespace Lockstep.Game {
    public class TraceLogSystem : BaseSystem {
        StringBuilder _dumpSb = new StringBuilder();

        public override void DoUpdate(LFloat deltaTime){
            _dumpSb.AppendLine("Tick: " + World.Instance.Tick);
            //trace input
            foreach (var input in World.Instance.PlayerInputs) {
                DumpInput(input);
            }

            foreach (var entity in _gameStateService.GetPlayers()) {
                DumpEntity(entity);
            }

            foreach (var entity in _gameStateService.GetEnemies()) {
                //dumpSb.Append(" " + entity.timer);
                DumpEntity(entity);
            }

            //_debugService.Trace(_dumpSb.ToString(), true);
            _dumpSb.Clear();
        }

        private void DumpInput(PlayerInput input){
            _dumpSb.Append("    ");
            _dumpSb.Append(" skillId:" + input.skillId);
            _dumpSb.Append(" " + input.mousePos);
            _dumpSb.Append(" " + input.inputUV);
            _dumpSb.Append(" " + input.isInputFire);
            _dumpSb.Append(" " + input.isSpeedUp);
            _dumpSb.AppendLine();
        }


        private void DumpEntity(BaseEntity entity){
            _dumpSb.Append("    ");
            _dumpSb.Append(" " + entity.EntityId);
            _dumpSb.Append(" " + entity.transform.Pos3);
            _dumpSb.Append(" " + entity.transform.deg);
            _dumpSb.AppendLine();
        }
    }
}