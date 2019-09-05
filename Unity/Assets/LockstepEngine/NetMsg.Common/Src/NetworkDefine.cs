namespace NetMsg.Common {

    public class NetworkDefine {
        /// <summary>
        /// 最大延迟时间 超过这个时间 依旧等不到玩家的输入包，默认玩家没有输入（输入丢失）
        /// </summary>
        public const int MAX_DELAY_TIME_MS = 300;

        /// 正常玩家的延迟
        public const int NORMAL_PLAYER_MAX_DELAY = 100;

        /// 正常玩家最大收到输入确认包的延迟 （有其中一个玩家输入延迟太大 且自身网络达到66%丢包率 情况下的时延）
        public const int MAX_FRAME_DATA_DELAY = MAX_DELAY_TIME_MS + NORMAL_PLAYER_MAX_DELAY + 2 * UPDATE_DELTATIME;

        /// 帧率
        public const int FRAME_RATE = 33;
        
        public const int UPDATE_DELTATIME = 30;

        public const string NetKey = "LockstepPlatform";
    }
    
  
}