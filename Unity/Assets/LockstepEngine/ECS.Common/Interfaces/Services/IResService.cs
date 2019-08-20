namespace Lockstep.Game {
    public interface IResService : IService {
        string GetAssetPath(ushort assetId);
    }
}