using Lockstep.Game;
using Lockstep.Math;
using LockstepTutorial;
using UnityEngine;


public class MainScript : MonoBehaviour {
    private Launcher launcher;
    public bool IsClientMode = false;
    public int MaxEnemyCount = 10;
    [Header("Recorder")] public bool IsReplay = false;
    public string recordFilePath;

    private ServiceContainer _serviceContainer;

    private void Awake(){
        launcher = new Launcher();
        gameObject.AddComponent<PingMono>();
        gameObject.AddComponent<InputMono>();
        EnemyManager.maxCount = MaxEnemyCount;
        _serviceContainer = new UnityServiceContainer();
        _serviceContainer.GetService<IConstStateService>().GameName = "ARPGDemo";
        _serviceContainer.GetService<IConstStateService>().IsClientMode = IsClientMode;
        _serviceContainer.GetService<IConstStateService>().IsReplay = IsReplay;
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
    }

    private void Update(){
        launcher.DoUpdate(Time.deltaTime);
    }

    private void OnDestroy(){
        launcher.DoDestroy();
    }

    private void OnApplicationQuit(){
        launcher.OnApplicationQuit();
    }

    private T GetService<T>() where T : IService{
        return _serviceContainer.GetService<T>();
    }
}