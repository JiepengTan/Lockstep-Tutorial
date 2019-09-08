
namespace NetMsg.Common {

    public enum EServerDetailPortType {
        ServerPort,
        TcpPort,
        UdpPort,
    }

    public enum ELoginResult {
        Success,
        Reconnect,
        PasswordMissMatch,
        ErrorHash,
        NotLogin,
    }

    public enum ERoomOperatorResult {
        Succ,
        Full,
        NotExist,
        AlreadyExist,
    }

    public enum EGameState {
        Idle,
        Loading,
        PartLoaded,
        Playing,
        PartFinished,
        FinishAll,
    }
    
    //msg between client RoomServer
    public enum EMsgSC : ushort {
        //GC 
        //!! please ensure udp msg's id < 255
        //Room msg
        C2G_UdpHello,
        G2C_UdpHello,
        C2G_UdpMessage,
        G2C_UdpMessage,
        C2G_TcpMessage,
        G2C_TcpMessage,
        //udp
        C2G_ReqMissFrame,
        C2G_RepMissFrameAck,
        G2C_RepMissFrame,
        C2G_HashCode,
        C2G_PlayerInput,
        G2C_FrameData,
        C2G_PlayerPing,
        G2C_PlayerPing,
        //end of Udp
        EnumEndOfUdpMessage,
        
        
        S2C_TickPlayer,

        //IC
        C2I_UserLogin,
        I2C_LoginResult,

        //LC
        //chat
        C2L_RoomChatInfo,
        L2C_RoomChatInfo,
        
        //Room Operator
        I2L_UserLogin,
        C2L_UserLogin,
        //Room Operator
        C2L_ReqRoomList,
        L2C_RoomList,
        L2C_RoomInfoUpdate,
        
        C2L_JoinRoom,
        L2C_JoinRoom,
        L2C_JoinRoomResult,
        C2L_ReadyInRoom,
        L2C_ReadyInRoom,
        C2L_LeaveRoom,
        L2C_LeaveRoom,
        C2L_CreateRoom,
        L2C_CreateRoomResult,
        C2L_StartGame,
        L2C_StartGame,

        //LG
        L2G_CreateGame,
        L2G_UserReconnect,

        //GC Tcp
        C2G_Hello,
        G2C_Hello,
        G2C_GameStartInfo,
        C2G_LoadingProgress,
        G2C_LoadingProgress,
        G2C_AllFinishedLoaded,
        C2G_FinishedLevel,
        /// <summary>
        ///  game state:Failed Success PartLoaded
        /// </summary>
        G2C_GameStatu,

        //
        C2G_GameEvent,
        G2C_GameEvent,


        EnumCount
    }
}