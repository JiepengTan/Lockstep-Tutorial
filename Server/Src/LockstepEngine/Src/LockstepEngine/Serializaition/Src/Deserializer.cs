using System;
using System.Collections.Generic;
using System.Text;
using Lockstep.Math;

namespace Lockstep.Serialization {
    public delegate uint FuncReadSlot(uint vTblOffset, int idx);

    public class Deserializer {
        protected byte[] _data;
        protected int _position;
        protected int _dataSize;
        private int _offset;

        public byte[] RawData {
            get { return _data; }
        }

        public int RawDataSize {
            get { return _dataSize; }
        }

        public int UserDataOffset {
            get { return _offset; }
        }

        public int UserDataSize {
            get { return _dataSize - _offset; }
        }

        public bool IsNull {
            get { return _data == null; }
        }

        public int Position {
            get { return _position; }
        }

        public void SetPosition(int pos){
            _position = pos;
        }

        public bool SkipLen(long len){
            var dst = _position + len;
            if (dst > _dataSize) {
                throw new Exception(
                    $"Skip len is out of range _dataSize:{_dataSize} _position:{_position}  skipLen:{len}");
                return false;
            }

            _position += (int) len;
            return true;
        }

        public bool EndOfData {
            get { return _position == _dataSize; }
        }

        public int AvailableBytes {
            get { return _dataSize - _position; }
        }

        public bool IsEnd {
            get { return _dataSize == _position; }
        }

        public void SetSource(Serializer dataWriter){
            _data = dataWriter.Data;
            _position = 0;
            _offset = 0;
            _dataSize = dataWriter.Length;
        }

        public void SetSource(byte[] source){
            _data = source;
            _position = 0;
            _offset = 0;
            _dataSize = source.Length;
        }

        public void SetSource(byte[] source, int offset){
            _data = source;
            _position = offset;
            _offset = offset;
            _dataSize = source.Length;
        }

        public void SetSource(byte[] source, int offset, int maxSize){
            _data = source;
            _position = offset;
            _offset = offset;
            _dataSize = maxSize;
        }

        public Deserializer(){ }

        public Deserializer(byte[] source){
            SetSource(source);
        }

        public Deserializer(byte[] source, int offset){
            SetSource(source, offset);
        }

        public Deserializer(byte[] source, int offset, int maxSize){
            SetSource(source, offset, maxSize);
        }

        #region GetMethods    

        public bool SetSlotOffset(int vTblOffset, int slotSize, int idx){
            int offset = (int) (vTblOffset + idx * slotSize);
            int dataOffset = _data[offset];
            if (slotSize == 4) {
                dataOffset = (int) FastBitConverter.ToUInt32(_data, offset);
            }
            else if (slotSize == 2) {
                dataOffset = FastBitConverter.ToUInt16(_data, offset);
            }
            _position = dataOffset;
            return dataOffset != 0;
        }

        public uint ReadSlotInt16(uint offset, int idx){
            return FastBitConverter.ToUInt16(_data, (int) (offset + idx * 2));
        }

        public uint ReadSlotInt32(uint offset, int idx){
            return FastBitConverter.ToUInt32(_data, (int) (offset + idx * 2));
        }

        public byte ReadByte(){
            byte res = _data[_position];
            _position += 1;
            return res;
        }

        public sbyte ReadSByte(){
            var b = (sbyte) _data[_position];
            _position++;
            return b;
        }

        public bool ReadBoolean(){
            bool res = _data[_position] > 0;
            _position += 1;
            return res;
        }

        public char ReadChar(){
            char result = (char) FastBitConverter.ToInt16(_data, _position);
            _position += 2;
            return result;
        }

        public ushort ReadUInt16(){
            ushort result = FastBitConverter.ToUInt16(_data, _position);
            _position += 2;
            return result;
        }

        public short ReadInt16(){
            short result = FastBitConverter.ToInt16(_data, _position);
            _position += 2;
            return result;
        }

        public long ReadInt64(){
            long result = FastBitConverter.ToInt64(_data, _position);
            _position += 8;
            return result;
        }

        public ulong ReadUInt64(){
            ulong result = FastBitConverter.ToUInt64(_data, _position);
            _position += 8;
            return result;
        }

        public int ReadInt32(){
            int result = FastBitConverter.ToInt32(_data, _position);
            _position += 4;
            return result;
        }

        public uint ReadUInt32(){
            uint result = FastBitConverter.ToUInt32(_data, _position);
            _position += 4;
            return result;
        }

        public float ReadSingle(){
            float result = FastBitConverter.ToSingle(_data, _position);
            _position += 4;
            return result;
        }

        public double ReadDouble(){
            double result = FastBitConverter.ToDouble(_data, _position);
            _position += 8;
            return result;
        }

        public LFloat ReadLFloat(){
            var x = ReadInt32();
            return new LFloat(true, x);
        }

        public LVector2 ReadLVector2(){
            var x = ReadInt32();
            var y = ReadInt32();
            return new LVector2(true, x, y);
        }

        public LVector3 ReadLVector3(){
            var x = ReadInt32();
            var y = ReadInt32();
            var z = ReadInt32();
            return new LVector3(true, x, y, z);
        }


        public T ReadRef<T>(ref T _) where T : BaseFormater, new(){
            if (ReadBoolean())
                return null;
            var val = new T();
            val.Deserialize(this);
            return val;
        }

        public string ReadString(int maxLength){
            int bytesCount = ReadInt32();
            if (bytesCount <= 0 || bytesCount > maxLength * 2) {
                return string.Empty;
            }

            int charCount = Encoding.UTF8.GetCharCount(_data, _position, bytesCount);
            if (charCount > maxLength) {
                return string.Empty;
            }

            string result = Encoding.UTF8.GetString(_data, _position, bytesCount);
            _position += bytesCount;
            return result;
        }

        public string ReadString(){
            int bytesCount = ReadInt32();
            if (bytesCount <= 0) {
                return string.Empty;
            }

            string result = Encoding.UTF8.GetString(_data, _position, bytesCount);
            _position += bytesCount;
            return result;
        }

        public byte[] ReadArray(byte[] _){
            return ReadBytes();
        }

        public short[] ReadArray(short[] _){
            return _ReadArray(ReadInt16);
        }

        public ushort[] ReadArray(ushort[] _){
            return _ReadArray(ReadUInt16);
        }

        public int[] ReadArray(int[] _){
            return _ReadArray(ReadInt32);
        }

        public uint[] ReadArray(uint[] _){
            return _ReadArray(ReadUInt32);
        }

        public long[] ReadArray(long[] _){
            return _ReadArray(ReadInt64);
        }

        public ulong[] ReadArray(ulong[] _){
            return _ReadArray(ReadUInt64);
        }

        public float[] ReadArray(float[] _){
            return _ReadArray(ReadSingle);
        }

        public double[] ReadArray(double[] _){
            return _ReadArray(ReadDouble);
        }

        public bool[] ReadArray(bool[] _){
            return _ReadArray(ReadBoolean);
        }

        public string[] ReadArray(string[] _){
            return _ReadArray(ReadString);
        }

        private T[] _ReadArray<T>(Func<T> _func){
            ushort size = FastBitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new T[size];
            for (int i = 0; i < size; i++) {
                arr[i] = _func();
            }

            return arr;
        }

        public T[] ReadArray<T>(T[] _) where T : BaseFormater, new(){
            ushort len = ReadUInt16();
            if (len == 0)
                return null;
            var formatters = new T[len];
            for (int i = 0; i < len; i++) {
                if (ReadBoolean())
                    formatters[i] = null;
                else {
                    var val = new T();
                    val.Deserialize(this);
                    formatters[i] = val;
                }
            }

            return formatters;
        }

        public List<T> ReadList<T>(List<T> _) where T : BaseFormater, new(){
            ushort len = ReadUInt16();
            if (len == 0)
                return null;
            var formatters = new List<T>(len);
            for (int i = 0; i < len; i++) {
                if (ReadBoolean())
                    formatters[i] = null;
                else {
                    var val = new T();
                    val.Deserialize(this);
                    formatters.Add(val);
                }
            }

            return formatters;
        }

        public byte[] GetRemainingBytes(){
            byte[] outgoingData = new byte[AvailableBytes];
            Buffer.BlockCopy(_data, _position, outgoingData, 0, AvailableBytes);
            _position = _data.Length;
            return outgoingData;
        }

        public void GetBytes(byte[] destination, int start, int count){
            Buffer.BlockCopy(_data, _position, destination, start, count);
            _position += count;
        }

        public void GetBytes(byte[] destination, int count){
            Buffer.BlockCopy(_data, _position, destination, 0, count);
            _position += count;
        }

        public byte[] ReadBytes(){
            ushort size = ReadUInt16();
            if (size == 0) return null;
            var outgoingData = new byte[size];
            Buffer.BlockCopy(_data, _position, outgoingData, 0, size);
            _position += size;
            return outgoingData;
        }

        public byte[] ReadBytes_255(){
            ushort size = ReadByte();
            if (size == 0) return null;
            var outgoingData = new byte[size];
            Buffer.BlockCopy(_data, _position, outgoingData, 0, size);
            _position += size;
            return outgoingData;
        }

        #endregion

        #region PeekMethods

        public byte PeekByte(){
            return _data[_position];
        }

        public sbyte PeekSByte(){
            return (sbyte) _data[_position];
        }

        public bool PeekBool(){
            return _data[_position] > 0;
        }

        public char PeekChar(){
            return (char) FastBitConverter.ToInt16(_data, _position);
        }

        public ushort PeekUShort(){
            return FastBitConverter.ToUInt16(_data, _position);
        }

        public short PeekShort(){
            return FastBitConverter.ToInt16(_data, _position);
        }

        public long PeekLong(){
            return FastBitConverter.ToInt64(_data, _position);
        }

        public ulong PeekULong(){
            return FastBitConverter.ToUInt64(_data, _position);
        }

        public int PeekInt(){
            return FastBitConverter.ToInt32(_data, _position);
        }

        public uint PeekUInt(){
            return FastBitConverter.ToUInt32(_data, _position);
        }

        public float PeekFloat(){
            return FastBitConverter.ToSingle(_data, _position);
        }

        public double PeekDouble(){
            return FastBitConverter.ToDouble(_data, _position);
        }

        public string PeekString(int maxLength){
            int bytesCount = BitConverter.ToInt32(_data, _position);
            if (bytesCount <= 0 || bytesCount > maxLength * 2) {
                return string.Empty;
            }

            int charCount = Encoding.UTF8.GetCharCount(_data, _position + 4, bytesCount);
            if (charCount > maxLength) {
                return string.Empty;
            }

            string result = Encoding.UTF8.GetString(_data, _position + 4, bytesCount);
            return result;
        }

        public string PeekString(){
            int bytesCount = BitConverter.ToInt32(_data, _position);
            if (bytesCount <= 0) {
                return string.Empty;
            }

            string result = Encoding.UTF8.GetString(_data, _position + 4, bytesCount);
            return result;
        }

        #endregion

        #region TryReadMethods

        public bool TryReadByte(out byte result){
            if (AvailableBytes >= 1) {
                result = ReadByte();
                return true;
            }

            result = 0;
            return false;
        }

        public bool TryReadSByte(out sbyte result){
            if (AvailableBytes >= 1) {
                result = ReadSByte();
                return true;
            }

            result = 0;
            return false;
        }

        public bool TryReadBool(out bool result){
            if (AvailableBytes >= 1) {
                result = ReadBoolean();
                return true;
            }

            result = false;
            return false;
        }

        public bool TryReadChar(out char result){
            if (AvailableBytes >= 2) {
                result = ReadChar();
                return true;
            }

            result = '\0';
            return false;
        }

        public bool TryReadShort(out short result){
            if (AvailableBytes >= 2) {
                result = ReadInt16();
                return true;
            }

            result = 0;
            return false;
        }

        public bool TryReadUShort(out ushort result){
            if (AvailableBytes >= 2) {
                result = ReadUInt16();
                return true;
            }

            result = 0;
            return false;
        }

        public bool TryReadInt(out int result){
            if (AvailableBytes >= 4) {
                result = ReadInt32();
                return true;
            }

            result = 0;
            return false;
        }

        public bool TryReadUInt(out uint result){
            if (AvailableBytes >= 4) {
                result = ReadUInt32();
                return true;
            }

            result = 0;
            return false;
        }

        public bool TryReadLong(out long result){
            if (AvailableBytes >= 8) {
                result = ReadInt64();
                return true;
            }

            result = 0;
            return false;
        }

        public bool TryReadULong(out ulong result){
            if (AvailableBytes >= 8) {
                result = ReadUInt64();
                return true;
            }

            result = 0;
            return false;
        }

        public bool TryReadFloat(out float result){
            if (AvailableBytes >= 4) {
                result = ReadSingle();
                return true;
            }

            result = 0;
            return false;
        }

        public bool TryReadDouble(out double result){
            if (AvailableBytes >= 8) {
                result = ReadDouble();
                return true;
            }

            result = 0;
            return false;
        }

        public bool TryReadString(out string result){
            if (AvailableBytes >= 4) {
                var bytesCount = PeekInt();
                if (AvailableBytes >= bytesCount + 4) {
                    result = ReadString();
                    return true;
                }
            }

            result = null;
            return false;
        }

        public bool TryReadStringArray(out string[] result){
            ushort size;
            if (!TryReadUShort(out size)) {
                result = null;
                return false;
            }

            result = new string[size];
            for (int i = 0; i < size; i++) {
                if (!TryReadString(out result[i])) {
                    result = null;
                    return false;
                }
            }

            return true;
        }

        #endregion

        public void Clear(){
            _position = 0;
            _dataSize = 0;
            _data = null;
        }
    }
}