#define DEBUG_FRAME_DELAY
using System;
using System.Collections.Generic;
using Lockstep.Util;
using NetMsg.Common;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
    /// <summary>
    /// frame buffer
    /// </summary>
    public class FrameBuffer {
        /// for debug
        public static byte DebugMainActorID;
        /// 客户端最大可以超前的frame 数量
        public const int MaxClientPredictFrameCount =
            NetworkDefine.MAX_FRAME_DATA_DELAY / NetworkDefine.UPDATE_DELTATIME;
        /// 客户度FrameBuffer Size 
        public static int BufferSize = NetworkDefine.FRAME_RATE * 30 + MaxClientPredictFrameCount;

        /// 进行备份的帧间隔
        public static int SnapshotFrameInterval = 2;
        /// 回滚需要的空间
        public static int RollbackNeedSpace = SnapshotFrameInterval * 2;
        /// 最大的可以超前的Frame数量
        public static int MaxServerOverrideFrameCount = BufferSize - RollbackNeedSpace;

        public ServerFrame[] serverBuffer;
        public ServerFrame[] clientBuffer;

        /// 下一个需要验证的tick
        public int nextTickToCheck;

        /// 下一步需要执行的客户端tick
        public int nextClientTick;

        /// 当前服务器的Tick (从接收到的消息中分析出来的)
        public int curServerTick;

        /// 当前Buffer中最大的服务器Tick
        public int maxServerTickInBuffer = -1;


        public bool IsNeedRevert = false;
        private int firstMissFrameTick;

        public FrameBuffer(){
            RollbackNeedSpace = SnapshotFrameInterval * 2;
            MaxServerOverrideFrameCount = BufferSize - RollbackNeedSpace;
            serverBuffer = new ServerFrame[BufferSize];
            clientBuffer = new ServerFrame[BufferSize];
        }

        public void SetClientTick(int tick){
            nextClientTick = tick + 1;
        }

        public void PushLocalFrame(ServerFrame frame){
            var sIdx = frame.tick % BufferSize;
            Debug.Assert(clientBuffer[sIdx] == null || clientBuffer[sIdx].tick <= frame.tick,
                "Push local frame error!");
            clientBuffer[sIdx] = frame;
        }

        ///1.push server frames
        public void PushServerFrames(ServerFrame[] frames, bool isNeedDebugCheck = true){
            var count = frames.Length;
            for (int i = 0; i < count; i++) {
                var data = frames[i];

                if (data == null || data.tick < nextTickToCheck) {
                    //the frame is already checked
                    return;
                }

                if (data.tick > curServerTick) {
                    curServerTick = data.tick;
                }

                if (data.tick >= nextTickToCheck + MaxServerOverrideFrameCount - 1) {
                    //本地服务器落后太多 需要本地验证完成后再统一叫服务器下发
                    return;
                }

                if (data.tick > maxServerTickInBuffer) { //记录最大服务帧
                    maxServerTickInBuffer = data.tick;
                }

                var targetIdx = data.tick % BufferSize;
                if (serverBuffer[targetIdx] == null || serverBuffer[targetIdx].tick != data.tick) {
                    serverBuffer[targetIdx] = data;
                }
            }
        }

        ///2.confirm frames=> (nextTickToCheck,hasMissedFrame,CanClientSimulate)
        public void UpdateFramesInfo(){
            //不考虑追帧
            //UnityEngine.Debug.Assert(nextTickToCheck <= nextClientTick, "localServerTick <= localClientTick ");
            //Confirm frames
            IsNeedRevert = false;
            while (nextTickToCheck <= maxServerTickInBuffer) {
                var sIdx = nextTickToCheck % BufferSize;
                var cFrame = clientBuffer[sIdx];
                var sFrame = serverBuffer[sIdx];
                //服务器帧 或者客户端帧 还没到
                if (cFrame == null || cFrame.tick != nextTickToCheck || sFrame == null ||
                    sFrame.tick != nextTickToCheck)
                    break;
                //Check client guess input match the real input
                if (object.ReferenceEquals(sFrame, cFrame) || sFrame.Equals(cFrame)) {
                    nextTickToCheck++;
                }
                else {
                    IsNeedRevert = true;
                    break;
                }
            }
        }


        public int GetMissServerFrameTick(){
            UpdateMissServerFrameTick();
            return firstMissFrameTick;
        }

        private void UpdateMissServerFrameTick(){
            int tick = nextTickToCheck;
            for (; tick <= maxServerTickInBuffer; tick++) {
                var idx = tick % BufferSize;
                if (serverBuffer[idx] == null || serverBuffer[idx].tick != tick) {
                    break;
                }
            }

            firstMissFrameTick = tick;
        }

        public bool CanExecuteNextFrame(){
            return (nextClientTick - firstMissFrameTick) < MaxClientPredictFrameCount;
        }

        public bool IsNeedReqMissFrame(){
            return (curServerTick > nextClientTick);
        }

        public int Ping = 50;

        public int GetTargetTick(){
            return curServerTick + (Ping * 2) / NetworkDefine.UPDATE_DELTATIME + 2;
        }

        public ServerFrame GetFrame(int tick){
            var sFrame = GetServerFrame(tick);
            if (sFrame != null) {
                return sFrame;
            }

            return GetLocalFrame(tick);
        }

        public ServerFrame GetServerFrame(int tick){
            if (tick > maxServerTickInBuffer) {
                return null;
            }

            var idx = tick % BufferSize;
            var frame = serverBuffer[idx];
            if (frame == null) return null;
            if (frame.tick != tick) return null;
            return frame;
        }

        public ServerFrame GetLocalFrame(int tick){
            lock (this) {
                if (tick >= nextClientTick) {
                    return null;
                }

                var idx = tick % BufferSize;
                return clientBuffer[idx];
            }
        }

        public int[] GetMissFrames(){ //check miss frame msg
            lock (this) {
                int missCount = 0;
                for (int tick = nextTickToCheck; tick < maxServerTickInBuffer; tick++) {
                    var idx = tick % BufferSize;
                    if (serverBuffer[idx] == null) { //有空窗口
                        ++missCount;
                    }
                }

                if (missCount > 0) {
                    var missFrames = new int[missCount];
                    int missFrameIdx = 0;
                    for (int tick = nextTickToCheck; tick < maxServerTickInBuffer; tick++) {
                        var idx = tick % BufferSize;
                        if (serverBuffer[idx] == null) { //有空窗口
                            missFrames[missFrameIdx++] = tick;
                        }
                    }

                    return missFrames;
                }

                return null;
            }
        }
    }
}