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
    public enum EItemType {
        Boom,
        AddLife,
        Upgrade,
    }
    [System.Serializable]
    public enum EAssetID : ushort {
        EnemyTank0 = 10,
        EnemyTank1,
        EnemyTank2,
        EnemyTank3,
        EnemyTank4,

        ItemAddLife = 20,
        ItemBoom,
        ItemUpgrade,

        Camp = 30,

        PlayerTank0 = 40,
        PlayerTank1,
        PlayerTank2,
        PlayerTank3,

        Bullet0 = 50,
        Bullet1,
        Bullet2,
    }
}