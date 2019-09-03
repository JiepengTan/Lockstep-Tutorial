using Lockstep.Game;
using Lockstep.Math;
using Lockstep.Game;
using UnityEngine;


public class MainScript : MonoBehaviour {
    public Launcher launcher = new Launcher();
    public int MaxEnemyCount = 10;
    public bool IsClientMode = false;
    public bool IsRunVideo;
    public bool IsVideoMode = false;
    public string RecordFilePath;
    public bool HasInit = false;

    private ServiceContainer _serviceContainer;

    private void Awake(){
        gameObject.AddComponent<PingMono>();
        gameObject.AddComponent<InputMono>();
        _serviceContainer = new UnityServiceContainer();
        _serviceContainer.GetService<IConstStateService>().GameName = "ARPGDemo";
        _serviceContainer.GetService<IConstStateService>().IsClientMode = IsClientMode;
        _serviceContainer.GetService<IConstStateService>().IsVideoMode = IsVideoMode;
        _serviceContainer.GetService<IGameStateService>().MaxEnemyCount = MaxEnemyCount;
        Lockstep.Logging.Logger.OnMessage += UnityLogHandler.OnLog;
        Screen.SetResolution(1024, 768, false);

        launcher.DoAwake(_serviceContainer);
    }


    private void Start(){
        var stateService = GetService<IConstStateService>();
        string path = Application.dataPath;
#if UNITY_EDITOR
        path = Application.dataPath + "/../../../";
#elif UNITY_STANDALONE_OSX
        path = Application.dataPath + "/../../../../../";
#elif UNITY_STANDALONE_WIN
        path = Application.dataPath + "/../../../";
#endif
        Debug.Log(path);
        stateService.RelPath = path;
        launcher.DoStart();
        HasInit = true;
    }

    private void Update(){
        _serviceContainer.GetService<IConstStateService>().IsRunVideo = IsVideoMode;
        launcher.DoUpdate(Time.deltaTime);
    }

    private void OnDestroy(){
        launcher.DoDestroy();
    }

    private void OnApplicationQuit(){
        launcher.OnApplicationQuit();
    }

    public T GetService<T>() where T : IService{
        return _serviceContainer.GetService<T>();
    }
}