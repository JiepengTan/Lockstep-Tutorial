using System;
using Lockstep.Serialization;

namespace NetMsg.Common {

    public interface IBaseMsg { }
    [System.Serializable]
    [SelfImplement]
    public partial class BaseMsg : BaseFormater ,IBaseMsg{}
    
    #region UDP

    [Serializable]
    [SelfImplement]
    [Udp]
    public partial class Msg_RepMissFrame : MutilFrames { }

    [Serializable]
    [SelfImplement]
    [Udp]
    public partial class Msg_ServerFrames : MutilFrames { }

    [Udp]
    public partial class Msg_HashCode : BaseMsg {
        public int StartTick;
        public int[] HashCodes;
    }

    [Udp]
    public partial class Msg_RepMissFrameAck : BaseMsg {
        public int MissFrameTick;
    }

    [Udp]
    public partial class Msg_ReqMissFrame : BaseMsg {
        public int StartTick;
    }

    #endregion

    #region TCP

    public partial class IPEndInfo : BaseMsg {
        public string Ip;
        public ushort Port;
    }

    public partial class RoomChangedInfo : BaseMsg {
        public int RoomId;
        public byte CurPlayerCount;
    }

    public partial class RoomInfo : BaseMsg {
        public int GameType;
        public int MapId;
        public string Name;
        public byte MaxPlayerCount;

        public int RoomId;
        public byte State;
        public string OwnerName;
        public byte CurPlayerCount;
    }

    public partial class UserGameInfo : BaseMsg {
        public string Name;
        public byte[] Data;
    }

    public partial class GamePlayerInfo : BaseMsg {
        public long UserId;
        public string Account;
        public string LoginHash;
    }

    public partial class RoomPlayerInfo : BaseMsg {
        public long UserId;
        public string Name;
        public byte Status;
    }

    public partial class RoomChatInfo : BaseMsg {
        public byte Channel;
        public long SrcUserId;
        public long DstUserId;
        public byte[] Message;
    }

//IC
    public partial class Msg_C2I_UserLogin : BaseMsg {
        public string Account;
        public string Password;
        public string EncryptHash;
        public int GameType;
    }

    public partial class Msg_I2C_LoginResult : BaseMsg {
        public byte LoginResult;
        public string LoginHash;
        public long UserId;
        public IPEndInfo LobbyEnd;
    }


//LC
    public partial class Msg_S2C_TickPlayer : BaseMsg {
        public byte Reason;
    }

    public partial class Msg_C2L_UserLogin : BaseMsg {
        public long UserId;
        public string LoginHash;
    }


    public partial class Msg_C2L_ReqRoomList : BaseMsg {
        public short StartIdx;
    }

    public partial class Msg_L2C_RoomList : BaseMsg {
        public int GameType;
        public RoomInfo[] Rooms;
    }

    public partial class Msg_L2C_RoomInfoUpdate : BaseMsg {
        public RoomInfo[] AddInfo;
        public int[] DeleteInfo;
        public RoomChangedInfo[] ChangedInfo;
    }


    public partial class Msg_C2L_JoinRoom : BaseMsg {
        public int RoomId;
    }

    public partial class Msg_L2C_JoinRoomResult : BaseMsg {
        public RoomPlayerInfo[] PlayerInfos;
    }

    public partial class Msg_C2L_ReadyInRoom : BaseMsg {
        public byte State;
    }

    public partial class Msg_L2C_ReadyInRoom : BaseMsg {
        public long UserId;
        public byte State;
    }

    public partial class Msg_L2C_JoinRoom : BaseMsg {
        public RoomPlayerInfo PlayerInfo;
    }

    public partial class Msg_C2L_LeaveRoom : BaseMsg {
        public byte Reason;
    }

    public partial class Msg_L2C_LeaveRoom : BaseMsg {
        public long UserId;
    }

    public partial class Msg_C2L_RoomChatInfo : BaseMsg {
        public RoomChatInfo ChatInfo;
    }

    public partial class Msg_L2C_RoomChatInfo : BaseMsg {
        public RoomChatInfo ChatInfo;
    }

    public partial class Msg_C2L_CreateRoom : BaseMsg {
        public int GameType;
        public int MapId;
        public string Name;
        public byte MaxPlayerCount;
    }

    public partial class Msg_L2C_CreateRoom : BaseMsg {
        public RoomInfo Info;
        public RoomPlayerInfo[] PlayerInfos;
    }

    public partial class Msg_C2L_StartGame : BaseMsg {
        public byte Reason;
    }

    public partial class Msg_L2C_StartGame : BaseMsg {
        public byte Result;
        public IPEndInfo GameServerEnd;
        public string GameHash;
        public int GameId;
        public int RoomId;
        public bool IsReconnect;
    }

//LG
    public partial class Msg_L2G_UserReconnect : BaseMsg {
        public GamePlayerInfo PlayerInfo;
    }

    public partial class Msg_L2G_CreateGame : BaseMsg {
        public int GameType;
        public int MapId;
        public int RoomId;
        public GamePlayerInfo[] Players;
        public string GameHash;
    }

    public partial class Msg_L2G_UserLeave : BaseMsg {
        public long UserId;
        public int GameId;
    }

    public partial class Msg_G2L_OnGameFinished : BaseMsg {
        public int GameId;
        public int RoomId;
    }

//GC
    public partial class MessageHello : BaseMsg {
        public GamePlayerInfo UserInfo;
        public int GameType;
        public string GameHash;
        public int GameId;
        public bool IsReconnect;
    }

    public partial class Msg_C2G_Hello : BaseMsg {
        public MessageHello Hello;
    }

    public partial class Msg_G2C_Hello : BaseMsg {
        public byte LocalId;
        public byte UserCount;
        public int MapId;
        public int GameId;
        public int Seed;
        public IPEndInfo UdpEnd;
    }

    public partial class Msg_G2C_GameStartInfo : BaseMsg {
        public byte LocalId;
        public byte UserCount;
        public int MapId;
        public int RoomId;
        public int Seed;
        public GameData[] UserInfos;
        public IPEndInfo UdpEnd;
        public IPEndInfo TcpEnd;
        public int SimulationSpeed;
    }

    public partial class Msg_C2G_UdpHello : BaseMsg {
        public MessageHello Hello;
    }

    public partial class Msg_G2C_GameStatu : BaseMsg {
        public byte Status;
    }

    public partial class Msg_C2G_LoadingProgress : BaseMsg {
        /// [进度百分比 1表示1% 100表示已经加载完成]
        public byte Progress;
    }

    public partial class Msg_G2C_LoadingProgress : BaseMsg {
        /// [进度百分比 1表示1% 100表示已经加载完成]
        public byte[] Progress;
    }

    public partial class Msg_G2C_AllFinishedLoaded : BaseMsg {
        public short Level;
    }

    public partial class Msg_C2G_GameEvent : BaseMsg {
        public byte[] Data;
    }

    public partial class Msg_G2C_GameEvent : BaseMsg {
        public byte[] Data;
    }

    #endregion
}