namespace Lockstep.FakeServer {
    public class HashCodeMatcher {
        public long hashCode;
        public bool[] sendResult;
        public int count;

        public HashCodeMatcher(int num){
            hashCode = 0;
            sendResult = new bool[num];
            count = 0;
        }

        public bool IsMatchered => count == sendResult.Length;
    }
}