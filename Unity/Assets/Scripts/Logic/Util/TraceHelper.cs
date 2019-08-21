using System.Text;
using Lockstep.Logic;
using Lockstep.Logging;

namespace LockstepTutorial {
    public class TraceHelper {
        private static TraceHelper _instance = new TraceHelper();
        StringBuilder dumpSb = new StringBuilder();

        public static void TraceFrameState(){
            _instance._TraceFrameState();
        }

        private void _TraceFrameState(){
            dumpSb.AppendLine("Tick: " + GameManager.Instance.curFrameIdx);
            //trace input
            foreach (var input in GameManager.Instance.curFrameInput.inputs) {
                DumpInput(input);
            }

            foreach (var entity in GameManager.allPlayers) {
                DumpEntity(entity);
            }

            foreach (var entity in EnemyManager.Instance.allEnemy) {
                //dumpSb.Append(" " + entity.timer);
                DumpEntity(entity);
            }

            Debug.Trace(dumpSb.ToString(), true);
            dumpSb.Clear();
        }

        public void DumpInput(PlayerInput input){
            dumpSb.Append("    ");
            dumpSb.Append(" skillId:" + input.skillId);
            dumpSb.Append(" " + input.mousePos);
            dumpSb.Append(" " + input.inputUV);
            dumpSb.Append(" " + input.isInputFire);
            dumpSb.Append(" " + input.isSpeedUp);
            dumpSb.AppendLine();
        }


        public void DumpEntity(BaseEntity entity){
            dumpSb.Append("    ");
            dumpSb.Append(" " + entity.EntityId);
            dumpSb.Append(" " + entity.transform.Pos3);
            dumpSb.Append(" " + entity.transform.deg);
            dumpSb.AppendLine();
        }
    }
}