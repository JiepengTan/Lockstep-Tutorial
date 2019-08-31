namespace Lockstep.Game {

    public interface IGameAudioService :IService {
        void PlayClipBorn();
        void PlayClipDied();
        void PlayMusicBG();
        void PlayMusicStart();
        void PlayMusicGetItem();   
    }
}