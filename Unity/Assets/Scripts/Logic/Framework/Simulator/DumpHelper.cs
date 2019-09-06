using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lockstep.Game {
    public class DumpHelper : BaseSimulatorHelper {
        public DumpHelper(IServiceContainer serviceContainer, World world) : base(serviceContainer, world){ }

        public Dictionary<int, StringBuilder> _tick2RawFrameData = new Dictionary<int, StringBuilder>();
        public Dictionary<int, StringBuilder> _tick2OverrideFrameData = new Dictionary<int, StringBuilder>();


#if UNITY_EDITOR
        private string dumpPath => Path.Combine(UnityEngine.Application.dataPath, _serviceContainer.GetService<IGameConfigService>().DumpStrPath);
#endif
        
        private string dumpAllPath => "/Users/jiepengtan/Projects/Tutorial/LockstepTutorial/DumpLog";
        private HashHelper _hashHelper;
        public bool enable = false;
        public void DumpFrame(bool isNewFrame){
            if(!enable) return;
            var data = DumpFrame();
            if (isNewFrame) {
                _tick2RawFrameData[Tick] = data;
            }
            else {
                
                _tick2OverrideFrameData[Tick] = data;
            }
        }

        public void DumpToFile(){
            if(!enable) return;
#if UNITY_EDITOR
            var path = dumpPath + "/cur.txt";
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            var minTick = _tick2OverrideFrameData.Keys.Min();
            StringBuilder sbResume = new StringBuilder();
            StringBuilder sbRaw = new StringBuilder();
            for (int i = minTick; i <= Tick; i++) {
                sbRaw.AppendLine(_tick2RawFrameData[i].ToString());
                sbResume.AppendLine(_tick2OverrideFrameData[i].ToString());
            }

            File.WriteAllText(dumpPath + "/resume.txt", sbResume.ToString());
            File.WriteAllText(dumpPath + "/raw.txt", sbRaw.ToString());
            UnityEngine.Debug.Break();
#endif 
        }

        public void DumpAll(){
            if(!enable) return;
            var path = dumpAllPath + "/cur.txt";
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            StringBuilder sbRaw = new StringBuilder();
            for (int i = 0; i <= Tick; i++) {
                if (_tick2RawFrameData.TryGetValue(i, out var data)) {
                    sbRaw.AppendLine(data.ToString());
                }
            }
            File.WriteAllText(dumpAllPath + $"/All_{_serviceContainer.GetService<IConstStateService>().LocalActorId}.txt", sbRaw.ToString());
        }

        public StringBuilder DumpFrame(){
            var sb = new StringBuilder();
            sb.AppendLine("Tick : " + Tick + "--------------------");
            _DumpStr(sb, "");
            return sb;
        }

        private void _DumpStr(System.Text.StringBuilder sb, string prefix){
            foreach (var svc in _serviceContainer.GetAllServices()) {
                if (svc is IDumpStr hashSvc) {
                    sb.AppendLine(svc.GetType() + " --------------------");
                    hashSvc.DumpStr(sb, "\t" + prefix);
                }
            }
        }
    }
}