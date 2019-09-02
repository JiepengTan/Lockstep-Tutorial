using System.Collections.Generic;
using System.IO;
using Lockstep.Game;
using Lockstep.Serialization;
using Lockstep.Util;

namespace Lockstep.Game {
    public class RecordHelper {
        private const int RECODER_FILE_VERSION = 0;

        public static void Serialize(string recordFilePath, World mgr){
           // var writer = new Serializer();
           // writer.Write(RECODER_FILE_VERSION);
           // writer.Write(mgr.playerCount);
           // writer.Write(mgr.localPlayerId);
           // writer.Write(mgr.playerServerInfos);
//
           // var count = mgr.frames.Count;
           // writer.Write(count);
           // for (int i = 0; i < count; i++) {
           //     mgr.frames[i].Serialize(writer);
           // }
//
           // var bytes = writer.CopyData();
//
           // var relPath = PathUtil.GetUnityPath(recordFilePath);
           // var dir = Path.GetDirectoryName(relPath);
           // if (!Directory.Exists(dir)) {
           //     Directory.CreateDirectory(dir);
           // }
//
           // File.WriteAllBytes(relPath, bytes);
        }

        public static void Deserialize(string recordFilePath, World mgr){
#if !UNITY_EDITOR
        return;
#endif
          // var relPath = PathUtil.GetUnityPath(recordFilePath);
          // var bytes = File.ReadAllBytes(relPath);
          // var reader = new Deserializer(bytes);
          // var recoderFileVersion = reader.ReadInt32();
          // mgr.playerCount = reader.ReadInt32();
          // mgr.localPlayerId = reader.ReadInt32();
          // mgr.playerServerInfos = reader.ReadArray(mgr.playerServerInfos);

          // var count = reader.ReadInt32();
          // mgr.frames = new List<FrameInput>();
          // for (int i = 0; i < count; i++) {
          //     var frame = new FrameInput();
          //     frame.Deserialize(reader);
          //     frame.tick = i;
          //     mgr.frames.Add(frame);
          // }
        }
    }
}