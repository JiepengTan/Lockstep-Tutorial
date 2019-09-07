using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Lockstep.Game;
using Lockstep.Math;

namespace Lockstep.Serialization {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Limited : Attribute {
        public bool le255;
        public bool le65535;

        public Limited(){
            le255 = false;
            le65535 = true;
        }

        public Limited(bool isLess255){
            le255 = isLess255;
            le65535 = !isLess255;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,Inherited = false)]
    public class SelfImplementAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class UdpAttribute : Attribute { }

    public interface ISerializablePacket {
        byte[] ToBytes();
        void FromBytes(byte[] bytes);
    }

    public interface ISerializable {
        void Serialize(Serializer writer);

        void Deserialize(Deserializer reader);
    }

    public partial class Serializer {
        //for ext
        protected int[] offsets;

        public void SetPosition(int pos){
            _position = pos;
        }

        public int[] GetOffsetTable(int len){
            if (offsets == null) {
                offsets = new int[len];
                return offsets;
            }
            else {
                var rawLen = offsets.Length;
                if (rawLen >= len) {
                    for (int i = 0; i < len; i++) {
                        offsets[i] = 0;
                    }
                    return offsets;
                }

                while (rawLen < len) {
                    rawLen *= 2;
                }
                offsets = new int[rawLen];
                return offsets;
            }
        }
        
        protected byte[] _data;
        protected int _position;
        private const int InitialSize = 64;
        private int _capacity;

        public int Position => _position;
        public int Capacity {
            get { return _capacity; }
        }

        public Serializer() : this(true, InitialSize){ }

        public Serializer(int initialSize) : this(true, initialSize){ }

        private Serializer(bool autoResize, int initialSize){
            _data = new byte[initialSize];
            _capacity = initialSize;
        }

        /// <summary>
        /// Creates NetDataWriter from existing ByteArray
        /// </summary>
        /// <param name="bytes">Source byte array</param>
        /// <param name="copy">Copy array to new location or use existing</param>
        public static Serializer FromBytes(byte[] bytes, bool copy){
            if (copy) {
                var netDataWriter = new Serializer(true, bytes.Length);
                netDataWriter._Put(bytes);
                return netDataWriter;
            }

            return new Serializer(true, 0) {_data = bytes, _capacity = bytes.Length};
        }

        /// <summary>
        /// Creates NetDataWriter from existing ByteArray (always copied data)
        /// </summary>
        /// <param name="bytes">Source byte array</param>
        /// <param name="offset">Offset of array</param>
        /// <param name="length">Length of array</param>
        public static Serializer FromBytes(byte[] bytes, int offset, int length){
            var netDataWriter = new Serializer(true, bytes.Length);
            netDataWriter._Put(bytes, offset, length);
            return netDataWriter;
        }

        public static Serializer FromString(string value){
            var netDataWriter = new Serializer();
            netDataWriter.Write(value);
            return netDataWriter;
        }

        public void ResizeIfNeed(int newSize){
            int len = _data.Length;
            if (len < newSize) {
                while (len < newSize)
                    len *= 2;
                Array.Resize(ref _data, len);
                _capacity = _data.Length;
            }
        }

        public void Reset(int size){
            ResizeIfNeed(size);
            _position = 0;
        }

        public void Reset(){
            _position = 0;
        }

        public byte[] CopyData(){
            byte[] resultData = new byte[_position];
            Buffer.BlockCopy(_data, 0, resultData, 0, _position);
            return resultData;
        }

        public byte[] Data {
            get { return _data; }
        }

        public int Length {
            get { return _position; }
        }

        public void Write(float value){
            ResizeIfNeed(_position + 4);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 4;
        }

        public void Write(double value){
            ResizeIfNeed(_position + 8);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 8;
        }

        public void Write(long value){
            ResizeIfNeed(_position + 8);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 8;
        }

        public void Write(ulong value){
            ResizeIfNeed(_position + 8);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 8;
        }

        public void Write(int value){
            ResizeIfNeed(_position + 4);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 4;
        }

        public void Write(uint value){
            ResizeIfNeed(_position + 4);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 4;
        }


        public void Write(ushort value){
            ResizeIfNeed(_position + 2);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 2;
        }

        public void Write(short value){
            ResizeIfNeed(_position + 2);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 2;
        }

        public void Write(sbyte value){
            ResizeIfNeed(_position + 1);
            _data[_position] = (byte) value;
            _position++;
        }

        public void Write(byte value){
            ResizeIfNeed(_position + 1);
            _data[_position] = value;
            _position++;
        }

        public void Write(bool value){
            ResizeIfNeed(_position + 1);
            _data[_position] = (byte) (value ? 1 : 0);
            _position++;
        }

        public void Write(char value){
            ResizeIfNeed(_position + 2);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 2;
        }

        public void Write(IPEndPoint endPoint){
            Write(endPoint.Address.ToString());
            Write(endPoint.Port);
        }

        public void Write(string value){
            if (string.IsNullOrEmpty(value)) {
                Write(0);
                return;
            }

            //put bytes count
            int bytesCount = Encoding.UTF8.GetByteCount(value);
            ResizeIfNeed(_position + bytesCount + 4);
            Write(bytesCount);

            //put string
            Encoding.UTF8.GetBytes(value, 0, value.Length, _data, _position);
            _position += bytesCount;
        }


        public void Write(BaseFormater value){
            Write(value == null);
            value?.Serialize(this);
        }

 
        public void Write(string[] value){
            ushort len = value == null ? (ushort) 0 : (ushort) value.Length;
            Write(len);
            for (int i = 0; i < len; i++)
                Write(value[i]);
        }

        /// len should less then ushort.MaxValue
        public void Write(byte[] value){
            ushort len = value == null ? (ushort) 0 : (ushort) value.Length;
            Write(len);
            if (len > 0) {
                _Put(value);
            }
        }
        public void WriteBytes_255(byte[] value){
            if (value == null) {
                Write((byte) 0);
                return;
            }
            if (value.Length > byte.MaxValue) {
                throw new ArgumentOutOfRangeException($"Input Cmd len should less then {byte.MaxValue}");
            }
            Write((byte) value.Length);
            _Put(value);
        }

        public void Write(float[] value){_PutArray(value, sizeof(float), FastBitConverter.GetBytes);}

        public void Write(double[] value){_PutArray(value, sizeof(double), FastBitConverter.GetBytes);}

        public void Write(long[] value){_PutArray(value, sizeof(long), FastBitConverter.GetBytes);}

        public void Write(ulong[] value){_PutArray(value, sizeof(ulong), FastBitConverter.GetBytes);}

        public void Write(int[] value){_PutArray(value, sizeof(int), FastBitConverter.GetBytes);}

        public void Write(uint[] value){_PutArray(value, sizeof(uint), FastBitConverter.GetBytes);}

        public void Write(ushort[] value){_PutArray(value, sizeof(ushort), FastBitConverter.GetBytes);}

        public void Write(short[] value){_PutArray(value, sizeof(short), FastBitConverter.GetBytes);}

        public void Write(bool[] value){_PutArray(value, sizeof(bool), FastBitConverter.GetBytes);}

        public void Write(LFloat val){Write(val._val);}

        public void Write(LVector2 val){Write(val._x);Write(val._y);}

        public void Write(LVector3 val){Write(val._x);Write(val._y);Write(val._z);}


        public void Write<T>(T[] value) where T : BaseFormater{
            ushort len = (ushort) (value?.Length ?? 0);
            Write(len);
            for (int i = 0; i < len; i++) {
                var val = value[i];
                Write(val == null);
                val?.Serialize(this);
            }
        }

        public void Write<T>(List<T> value) where T : BaseFormater{
            ushort len = (ushort) (value?.Count ?? 0);
            Write(len);
            for (int i = 0; i < len; i++) {
                var val = value[i];
                Write(val == null);
                val?.Serialize(this);
            }
        }


        private void _Put(byte[] data, int offset, int length){
            ResizeIfNeed(_position + length);
            Buffer.BlockCopy(data, offset, _data, _position, length);
            _position += length;
        }

        private void _Put(byte[] data){
            ResizeIfNeed(_position + data.Length);
            Buffer.BlockCopy(data, 0, _data, _position, data.Length);
            _position += data.Length;
        }


        private void _PutArray<T>(T[] value, int typeSize, Action<byte[], int, T> _func) where T : struct{
            if (BitConverter.IsLittleEndian) {
                __PutArrayFastLE(value, typeSize);
                return;
            }
            ushort len = value == null ? (ushort) 0 : (ushort) value.Length;
            ResizeIfNeed(_position + 2 + typeSize * len);
            Write(len);
            for (int i = 0; i < len; i++) {
                _func(_data, _position, value[i]);
                _position += typeSize;
            }
        }
        private void __PutArrayFastLE<T>(T[] x, int elemSize) where T : struct{
            ushort len = x == null ? (ushort) 0 : (ushort) x.Length;
            int bytesCount = elemSize * len;
            ResizeIfNeed(_position + 2 + bytesCount);
            FastBitConverter.GetBytes(_data, _position, len);
            _position += 2;
            if (len == 0) {
                return;
            }

            // if we are LE, just do a block copy
            Buffer.BlockCopy(x, 0, _data, _position, bytesCount);
            _position += bytesCount;
        }
    }
}