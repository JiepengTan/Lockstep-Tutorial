namespace Lockstep {
    public enum EEvent {
        TryLogin,
        OnTickPlayer,
        OnConnLogin,
        OnLoginFailed,
        OnLoginResult,
        OnLeaveRoom,
        OnJoinRoomResult,
        OnPlayerJoinRoom,
        OnPlayerLeaveRoom,
        OnPlayerReadyInRoom,
        OnRoomChatInfo,
        OnRoomInfoUpdate,
        OnConnectToGameServer,
        
        VideoLoadProgress,
        VideoLoadDone,
        
        OnPlayerPing,
        OnServerHello,//连接GameServer 成功
        OnGameCreate,//连接GameServer 成功
        SimulationInit,//初始化Simulation
        LevelLoadProgress,//开始加载地图 本地load progress
        LevelLoadDone,//地图加载完成
        OnLoadingProgress,//Server notify load progress
        OnAllPlayerFinishedLoad,//Server notify all user finished load
        SimulationStart,//client start game lifecycle
        OnGameStartInfo,
        
        PursueFrameProcess,//追帧进度
        PursueFrameDone,//追帧进度
        
        ReconnectLoadProgress,//重连进度
        ReconnectLoadDone,//重连完成
        
        OnServerMissFrame,
        OnServerFrame,
        BorderVideoFrame,
        OnCreateRoom,
    }
}