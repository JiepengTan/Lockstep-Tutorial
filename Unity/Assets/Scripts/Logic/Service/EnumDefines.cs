namespace Lockstep.Game {
    [System.Serializable]
    public enum ECampType : byte {
        Player,
        Enemy,
        Other,
    }
    public enum EDir {
        Up,
        Left,
        Down,
        Right,
        EnumCount,
    }
}