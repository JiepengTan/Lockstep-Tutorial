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
        public string dumpPath => Path.Combine(UnityEngine.Application.dataPath, _serviceContainer.GetService<IGameConfigService>().DumpStrPath);
#endif
        private HashHelper _hashHelper;

        public void DumpFrame(bool isNewFrame){
            if (isNewFrame) { 
                _tick2RawFrameData[Tick] = DumpFrame();}
            else {
                
                _tick2OverrideFrameData[Tick] = DumpFrame();
            }
        }

        public void DumpToFile(){
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