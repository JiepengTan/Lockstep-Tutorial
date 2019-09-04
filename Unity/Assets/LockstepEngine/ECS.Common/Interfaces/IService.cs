namespace Lockstep.Game
{
    public interface IService {
    }

    public interface IHashCode {
        int GetHash(ref int idx);
    }
}