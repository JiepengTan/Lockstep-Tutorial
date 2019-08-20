using System.Collections.Generic;
using System.IO;
using Lockstep.Util;

namespace Lockstep.Game {
    public class ResService : BaseService, IResService {
        private Dictionary<ushort, string> _id2Path = new Dictionary<ushort, string>();
        public override void DoStart(){
            base.DoStart();
            var path = _constStateService.ClientConfigPath + "AssetPath.json";
            var text = File.ReadAllText(path);
            //TODO 
            var content = JsonUtil.ToObject<Dictionary<string, string>>(text);
            foreach (var pair in content) {
                _id2Path[ushort.Parse(pair.Key)] = pair.Value;
            }
        }

        public string GetAssetPath(ushort assetId){
            if (_id2Path.TryGetValue(assetId, out string path)) {
                return path;
            }
            return null;
        }
    }
}