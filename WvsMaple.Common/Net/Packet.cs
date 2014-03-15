using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Net
{
    public class Packet : IDisposable
    {
        private MemoryStream _memoryStream;
        private BinaryReader _binReader;
        private BinaryWriter _binWriter;

        public byte OperationCode { get; private set; }

        public int Remaining
        {
            get
            {
                return this.Length - this.Position;
            }
        }

        public Packet(byte[] pData, bool encrypted = false)
        {
            _memoryStream = new MemoryStream(pData);
            _binReader = new BinaryReader(_memoryStream);

            if (!encrypted)
            {
                this.OperationCode = this.ReadByte();
            }
        }

        public Packet()
        {
            _memoryStream = new MemoryStream();
            _binWriter = new BinaryWriter(_memoryStream);
        }

        public Packet(byte opcode)
        {
            _memoryStream = new MemoryStream();
            _binWriter = new BinaryWriter(_memoryStream);

            this.OperationCode = opcode;
            this.WriteByte(this.OperationCode);
        }

        public Packet(ServerMessages operationCode) : this((byte)operationCode) { }
        public Packet(InteroperabilityMessages operationCode) : this((byte)operationCode) { }

        public byte[] ToArray()
        {
            return _memoryStream.ToArray();
        }

        public int Length
        {
            get { return (int)_memoryStream.Length; }
        }

        public int Position
        {
            get { return (int)_memoryStream.Position; }
            set { _memoryStream.Position = value; }
        }

        public void Reset(int pPosition = 0)
        {
            _memoryStream.Position = pPosition;
        }

        public void Skip(int pAmount)
        {
            if (pAmount + _memoryStream.Position > Length)
                throw new Exception("!!! Cannot skip more bytes than there's inside the buffer!");
            _memoryStream.Position += pAmount;
        }

        public byte[] ReadLeftoverBytes()
        {
            return ReadBytes(Length - (int)_memoryStream.Position);
        }

        public override string ToString()
        {
            string ret = "";
            foreach (byte b in ToArray())
            {
                ret += string.Format("{0:X2} ", b);
            }
            return ret;
        }

        public void WriteBytes(params byte[] bytes)
        {
            foreach (byte b in bytes)
            {
                _binWriter.Write(b);
            }
        }
        public void WriteByte(byte val = 0) { _binWriter.Write(val); }
        public void WriteSByte(sbyte val = 0) { _binWriter.Write(val); }
        public void WriteBool(bool val) { WriteByte(val == true ? (byte)1 : (byte)0); }
        public void WriteShort(short val = 0) { _binWriter.Write(val); }
        public void WriteInt(int val = 0) { _binWriter.Write(val); }
        public void WriteLong(long val = 0) { _binWriter.Write(val); }
        public void WriteUShort(ushort val = 0) { _binWriter.Write(val); }
        public void WriteUInt(uint val = 0) { _binWriter.Write(val); }
        public void WriteULong(ulong val = 0) { _binWriter.Write(val); }
        public void WriteDouble(double val = 0) { _binWriter.Write(val); }
        public void WriteFloat(float val = 0) { _binWriter.Write(val); }
        public void WriteString(string val = "") { WriteShort((short)val.Length); _binWriter.Write(val.ToCharArray()); }
        public void WriteString(string val, int maxlen) { var i = 0; for (; i < val.Length & i < maxlen; i++) _binWriter.Write(val[i]); for (; i < maxlen; i++) WriteByte(0); }

        public void WriteHexString(string pInput)
        {
            pInput = pInput.Replace(" ", "");
            if (pInput.Length % 2 != 0) throw new Exception("Hex String is incorrect (size)");
            for (int i = 0; i < pInput.Length; i += 2)
            {
                WriteByte(byte.Parse(pInput.Substring(i, 2), System.Globalization.NumberStyles.HexNumber));
            }
        }

        public void WriteReversedLong(long value = 0)
        {
            WriteByte((byte)((value >> 32) & 0xFF));
            WriteByte((byte)((value >> 40) & 0xFF));
            WriteByte((byte)((value >> 48) & 0xFF));
            WriteByte((byte)((value >> 56) & 0xFF));
            WriteByte((byte)((value & 0xFF)));
            WriteByte((byte)((value >> 8) & 0xFF));
            WriteByte((byte)((value >> 16) & 0xFF));
            WriteByte((byte)((value >> 24) & 0xFF));
        }

        public void WriteDateTime(DateTime pDate)
        {
            this.WriteLong((long)((pDate.Millisecond * 10000) + 116444592000000000L));
            //this.Position += sizeof(long);
        }

        public void WriteZero(int length)
        {
            for (int i = 0; i < length; i++)
                WriteByte();
        }

        public byte[] ReadBytes(int pLen) { return _binReader.ReadBytes(pLen); }
        public bool ReadBool() { return _binReader.ReadByte() != 0; }
        public byte ReadByte() { return _binReader.ReadByte(); }
        public sbyte ReadSByte() { return _binReader.ReadSByte(); }
        public short ReadShort() { return _binReader.ReadInt16(); }
        public int ReadInt() { return _binReader.ReadInt32(); }
        public long ReadLong() { return _binReader.ReadInt64(); }
        public ushort ReadUShort() { return _binReader.ReadUInt16(); }
        public uint ReadUInt() { return _binReader.ReadUInt32(); }
        public ulong ReadULong() { return _binReader.ReadUInt64(); }
        public double ReadDouble() { return _binReader.ReadDouble(); }
        public float ReadFloat() { return _binReader.ReadSingle(); }
        public string ReadString(short pLen = -1) { short len = pLen == -1 ? _binReader.ReadInt16() : pLen; return new string(_binReader.ReadChars(len)); }

        public void SetBytes(int pPosition, byte[] val) { int tmp = (int)_memoryStream.Position; Reset(pPosition); _binWriter.Write(val); Reset(tmp); }
        public void SetByte(int pPosition, byte val) { int tmp = (int)_memoryStream.Position; Reset(pPosition); _binWriter.Write(val); Reset(tmp); }
        public void SetSByte(int pPosition, sbyte val) { int tmp = (int)_memoryStream.Position; Reset(pPosition); _binWriter.Write(val); Reset(tmp); }
        public void SetBool(int pPosition, bool val) { int tmp = (int)_memoryStream.Position; Reset(pPosition); WriteByte(val == true ? (byte)1 : (byte)0); Reset(tmp); }
        public void SetShort(int pPosition, short val) { int tmp = (int)_memoryStream.Position; Reset(pPosition); _binWriter.Write(val); Reset(tmp); }
        public void SetInt(int pPosition, int val) { int tmp = (int)_memoryStream.Position; Reset(pPosition); _binWriter.Write(val); Reset(tmp); }
        public void SetLong(int pPosition, long val) { int tmp = (int)_memoryStream.Position; Reset(pPosition); _binWriter.Write(val); Reset(tmp); }
        public void SetUShort(int pPosition, ushort val) { int tmp = (int)_memoryStream.Position; Reset(pPosition); _binWriter.Write(val); Reset(tmp); }
        public void SetUInt(int pPosition, uint val) { int tmp = (int)_memoryStream.Position; Reset(pPosition); _binWriter.Write(val); Reset(tmp); }
        public void SetULong(int pPosition, ulong val) { int tmp = (int)_memoryStream.Position; Reset(pPosition); _binWriter.Write(val); Reset(tmp); }

        ~Packet()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (this._binWriter != null)
                this._binWriter.Close();

            if (this._binReader != null)
                this._binReader.Close();

            this._memoryStream = null;
            this._binWriter = null;
            this._binReader = null;
        }
    }
}
