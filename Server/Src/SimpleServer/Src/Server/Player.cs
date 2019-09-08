using Lockstep.Network;
using Lockstep.Util;
using NetMsg.Common;


namespace Lockstep.FakeServer {
    public class Player : BaseRecyclable {
        public long UserId;
        public string Account;
        public string LoginHash;
        public byte LocalId;
        public Session PeerTcp;
        public Session PeerUdp;
        public Game Game;
        public GameData GameData;
        public int GameId => Game?.GameId ?? -1;

        public void OnLeave(){
            PeerTcp = null;
            PeerUdp = null;
        }

        public void SendTcp(EMsgSC type, BaseMsg msg){
            PeerTcp?.Send(0x00, (ushort) type, msg.ToBytes());
        }

        public void SendTcp(EMsgSC type, byte[] data){
            PeerTcp?.Send(0x00, (ushort) type, data);
        }

        public void SendUdp(EMsgSC type, byte[] data){
            PeerUdp?.Send(0x00, (ushort) type, data);
        }     
        public void SendUdp(EMsgSC type, BaseMsg msg){
            PeerUdp?.Send(0x00, (ushort) type, msg.ToBytes());
        }

        public override void OnRecycle(){
            PeerTcp = null;
            PeerUdp = null;
        }
    }
}