//#define DEBUG_SIMPLE_CHECK  暴力检测 不同步

using System.Collections.Generic;
using Lockstep.Logging;

namespace Lockstep.Game {
    public interface ICommand {
        void Do(object param);
        void Undo(object param);
    }

    public class BaseCommand : ICommand {
        public virtual void Do(object param){ }
        public virtual void Undo(object param){ }
    }

    public class CommandNode {
        public CommandNode pre;
        public CommandNode next;
        public int Tick;
        public ICommand cmd;

        public CommandNode(int tick, ICommand cmd, CommandNode pre = null, CommandNode next = null){
            this.Tick = tick;
            this.cmd = cmd;
            this.pre = pre;
            this.next = next;
        }
    }
    public delegate  void FuncUndoCommands(CommandNode minTickNode, CommandNode maxTickNode, object param);

    public class CommandBuffer : ICommandBuffer {

        protected CommandNode _head;
        protected CommandNode _tail;
        protected object _param;
        protected FuncUndoCommands _funcUndoCommand;

        public void Init(object param, FuncUndoCommands funcUndoCommand){
            _param = param;
            var func = funcUndoCommand;
            _funcUndoCommand = func ?? UndoCommands;
        }

        public List<CommandNode> heads = new List<CommandNode>();

        ///RevertTo tick , so all cmd between [tick,~)(Include tick) should undo
        public void Jump(int curTick, int dstTick){
            //Debug.Assert(curTick > dstTick, $"Not video mode should not roll forward curTick{curTick} dstTick{dstTick}");
            if (_tail == null || _tail.Tick <= dstTick) {
                return;
            }

            var newTail = _tail;
            while (newTail.pre != null && newTail.pre.Tick >= dstTick) {
                newTail = newTail.pre;
            }

            Debug.Assert(newTail.Tick >= dstTick,
                $"newTail must be the first cmd executed after that tick : tick:{dstTick}  newTail.Tick:{newTail.Tick}");
            Debug.Assert(newTail.pre == null
                         || newTail.pre.Tick < dstTick,
                $"newTail must be the first cmd executed in that tick : tick:{dstTick}  " +
                $"newTail.pre.Tick:{newTail.pre?.Tick ?? dstTick}");

            var minTickNode = newTail;
            var maxTickNode = _tail;

            if (newTail.pre == null) {
                _head = null;
                _tail = null;
            }
            else {
                _tail = newTail.pre;
                //断开链接
                _tail.next = null;
                newTail.pre = null;
            }

            _funcUndoCommand(minTickNode, maxTickNode, _param);
        }

        ///Discard all cmd between [0,maxVerifiedTick] (Include maxVerifiedTick)
        public void Clean(int maxVerifiedTick){
#if DEBUG_SIMPLE_CHECK
#else
            return;
            if (_head == null || _head.Tick > maxVerifiedTick) {
                return;
            }

            var newHead = _head;
            while (newHead.next != null && newHead.next.Tick <= maxVerifiedTick) {
                newHead = newHead.next;
            }

            if (newHead.next == null) {
                _tail = null;
                _head = null;
            }
            else {
                _head = newHead.next;
                //断开链接
                _head.pre = null;
                newHead.next = null;
            }
#endif
        }

        public void Execute(int tick, ICommand cmd){
            if (cmd == null) return;
            cmd.Do(_param);
            var node = new CommandNode(tick, cmd, _tail, null);
            if (_head == null) {
                _head = node;
                _tail = node;
                return;
            }

            _tail.next = node;
            _tail = node;
        }

        /// 只需执行undo 不需要顾虑指针的维护  //如果有性能需要可以考虑合并Cmd 
        protected void UndoCommands(CommandNode minTickNode, CommandNode maxTickNode, object param){
            if (maxTickNode == null) return;
            while (maxTickNode != minTickNode) {
                maxTickNode.cmd.Undo(_param);
                maxTickNode = maxTickNode.pre;
            }

            maxTickNode.cmd.Undo(_param);
        }
    }

    public interface ICommandBuffer{
        void Init(object param, FuncUndoCommands funcUndoCommand);

        ///RevertTo tick , so all cmd between [tick,~)(Include tick) should undo
        void Jump(int curTick, int dstTick);

        ///Discard all cmd between [0,maxVerifiedTick] (Include maxVerifiedTick)
        void Clean(int maxVerifiedTick);

        void Execute(int tick, ICommand cmd);
    }

}