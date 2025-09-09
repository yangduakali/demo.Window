using System;
using System.Collections.Generic;
using System.Text;
using network.client.ultis;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace network.client.message {

    public class Message : IMessage {
        public Message(SendMode sendMode = SendMode.Reliable, int bufferSize = 1230) {
            Bytes = new byte[bufferSize];
            SendMode = sendMode;
            instanceCount++;
        }

        public static Queue<IMessage> pool = new();
        public static int instanceCount;

        public static IMessage Create(SendMode sendMode = SendMode.Reliable, int bufferSize = 1230, byte[] bytes = null) {
            if (pool.Count == 0) {
                var nn =  new Message(sendMode, bufferSize);
                nn.Clear(bytes);
                if (!pool.Contains(nn)) {
                    pool.Enqueue(nn);
                }
            }

            var newMessage = pool.Dequeue();
            newMessage.Clear(bytes);
            return newMessage;
        }
        public void Release() {
            if (pool.Contains(this)) return;
            pool.Enqueue(this);
        }

        public SendMode SendMode { get; }
        public byte[] WritedBytes => Bytes[..WritePos];
        public int UnreadLength => WritePos - ReadPos;
        public int WrittenLength => WritePos;
        public int UnwrittenLength => Bytes.Length - WritePos;
        public byte[] Bytes { get; private set; }

        private ushort WritePos { get; set; }
        private ushort ReadPos { get; set; }

        public IMessage Add(byte value) {
            if (UnwrittenLength < 1) throw new InsufficientCapacityException(this, "byte", 1);

            Bytes[WritePos++] = value;
            return this;
        }
        public IMessage Add(sbyte value) {
            if (UnwrittenLength < 1) throw new InsufficientCapacityException(this, "sbyte", 1);

            Bytes[WritePos++] = (byte)value;
            return this;
        }
        public IMessage Add(bool value) {
            if (UnwrittenLength < 1) throw new InsufficientCapacityException(this, "bool", 1);

            Bytes[WritePos++] = (byte)(value ? 1u : 0u);
            return this;
        }
        public IMessage Add(short value) {
            if (UnwrittenLength < 2) throw new InsufficientCapacityException(this, "short", 2);

            Converter.FromShort(value, Bytes, WritePos);
            WritePos += 2;
            return this;
        }
        public IMessage Add(ushort value) {
            if (UnwrittenLength < 2) throw new InsufficientCapacityException(this, "ushort", 2);

            Converter.FromUShort(value, Bytes, WritePos);
            WritePos += 2;
            return this;
        }
        public IMessage Add(int value) {
            if (UnwrittenLength < 4) throw new InsufficientCapacityException(this, "int", 4);

            Converter.FromInt(value, Bytes, WritePos);
            WritePos += 4;
            return this;
        }
        public IMessage Add(uint value) {
            if (UnwrittenLength < 4) throw new InsufficientCapacityException(this, "uint", 4);

            Converter.FromUInt(value, Bytes, WritePos);
            WritePos += 4;
            return this;
        }
        public IMessage Add(long value) {
            if (UnwrittenLength < 8) throw new InsufficientCapacityException(this, "long", 8);

            Converter.FromLong(value, Bytes, WritePos);
            WritePos += 8;
            return this;
        }
        public IMessage Add(ulong value) {
            if (UnwrittenLength < 8) throw new InsufficientCapacityException(this, "ulong", 8);

            Converter.FromULong(value, Bytes, WritePos);
            WritePos += 8;
            return this;
        }
        public IMessage Add(float value) {
            if (UnwrittenLength < 4) throw new InsufficientCapacityException(this, "float", 4);

            Converter.FromFloat(value, Bytes, WritePos);
            WritePos += 4;
            return this;
        }
        public IMessage Add(double value) {
            if (UnwrittenLength < 8) throw new InsufficientCapacityException(this, "double", 8);

            Converter.FromDouble(value, Bytes, WritePos);
            WritePos += 8;
            return this;
        }
        public IMessage Add(string value) {
            if (value == null) {
                Add(false);
                return this;
            }
            Add(true);
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            int num = bytes.Length + (bytes.Length <= 127 ? 1 : 2);
            if (UnwrittenLength < num) throw new InsufficientCapacityException(this, "string", num);

            Add(bytes);
            return this;
        }
        public IMessage Add(byte[] array, bool includeLength = true) {
            if (includeLength) AddArrayLength(array.Length);

            if (UnwrittenLength < array.Length) throw new InsufficientCapacityException(this, array.Length, "byte", 1);

            Array.Copy(array, 0, Bytes, WritePos, array.Length);
            WritePos += (ushort)array.Length;
            return this;
        }
        public IMessage Add(sbyte[] array, bool includeLength = true) {
            if (includeLength) AddArrayLength(array.Length);

            if (UnwrittenLength < array.Length) throw new InsufficientCapacityException(this, array.Length, "sbyte", 1);

            for (int i = 0; i < array.Length; i++) {
                Bytes[WritePos++] = (byte)array[i];
            }

            return this;
        }
        public IMessage Add(bool[] array, bool includeLength = true) {
            if (includeLength) AddArrayLength(array.Length);

            ushort num = (ushort)(array.Length / 8 + (array.Length % 8 != 0 ? 1 : 0));
            if (UnwrittenLength < num) throw new InsufficientCapacityException(this, array.Length, "bool", 1, num);

            bool flag = array.Length % 8 == 0;
            for (int i = 0; i < num; i++) {
                byte b = 0;
                int num2 = 8;
                if (i + 1 == num && !flag) num2 = array.Length % 8;

                for (int j = 0; j < num2; j++) {
                    b = (byte)(b | (byte)((array[i * 8 + j] ? 1 : 0) << j));
                }

                Bytes[WritePos + i] = b;
            }

            WritePos += num;
            return this;
        }
        public IMessage Add(short[] array, bool includeLength = true) {
            if (includeLength) AddArrayLength(array.Length);

            if (UnwrittenLength < array.Length * 2) throw new InsufficientCapacityException(this, array.Length, "short", 2);

            for (int i = 0; i < array.Length; i++) {
                Add(array[i]);
            }

            return this;
        }
        public IMessage Add(ushort[] array, bool includeLength = true) {
            if (includeLength) AddArrayLength(array.Length);

            if (UnwrittenLength < array.Length * 2) throw new InsufficientCapacityException(this, array.Length, "ushort", 2);

            for (int i = 0; i < array.Length; i++) {
                Add(array[i]);
            }

            return this;
        }
        public IMessage Add(int[] array, bool includeLength = true) {
            if (includeLength) AddArrayLength(array.Length);

            if (UnwrittenLength < array.Length * 4) throw new InsufficientCapacityException(this, array.Length, "int", 4);

            for (int i = 0; i < array.Length; i++) {
                Add(array[i]);
            }

            return this;
        }
        public IMessage Add(uint[] array, bool includeLength = true) {
            if (includeLength) AddArrayLength(array.Length);

            if (UnwrittenLength < array.Length * 4) throw new InsufficientCapacityException(this, array.Length, "uint", 4);

            for (int i = 0; i < array.Length; i++) {
                Add(array[i]);
            }

            return this;
        }
        public IMessage Add(long[] array, bool includeLength = true) {
            if (includeLength) AddArrayLength(array.Length);

            if (UnwrittenLength < array.Length * 8) throw new InsufficientCapacityException(this, array.Length, "long", 8);

            for (int i = 0; i < array.Length; i++) {
                Add(array[i]);
            }

            return this;
        }
        public IMessage Add(ulong[] array, bool includeLength = true) {
            if (includeLength) AddArrayLength(array.Length);

            if (UnwrittenLength < array.Length * 8) throw new InsufficientCapacityException(this, array.Length, "ulong", 8);

            for (int i = 0; i < array.Length; i++) {
                Add(array[i]);
            }

            return this;
        }
        public IMessage Add(float[] array, bool includeLength = true) {
            if (includeLength) AddArrayLength(array.Length);

            if (UnwrittenLength < array.Length * 4) throw new InsufficientCapacityException(this, array.Length, "float", 4);

            for (int i = 0; i < array.Length; i++) {
                Add(array[i]);
            }

            return this;
        }
        public IMessage Add(double[] array, bool includeLength = true) {
            if (includeLength) AddArrayLength(array.Length);

            if (UnwrittenLength < array.Length * 8) throw new InsufficientCapacityException(this, array.Length, "double", 8);

            for (int i = 0; i < array.Length; i++) {
                Add(array[i]);
            }

            return this;
        }
        public IMessage Add(string[] array, bool includeLength = true) {
            if (array == null) {
                Add(false);
                return this;
            }
            Add(true);
            if (includeLength) AddArrayLength(array.Length);

            for (int i = 0; i < array.Length; i++) {
                Add(array[i]);
            }

            return this;
        }
        public IMessage Add(Vector3 value) {
            Add(value.x);
            Add(value.y);
            Add(value.z);
            return this;
        }
        public IMessage Add(Vector2 value) {
            Add(value.x);
            Add(value.y);
            return this;
        }
        public IMessage Add(Quaternion value) {
            Add(value.x);
            Add(value.y);
            Add(value.z);
            Add(value.w);
            return this;
        }

        public byte GetByte() {
            if (UnreadLength < 1) {
                Debug.LogError(NotEnoughBytesError("byte"));
                return 0;
            }

            return Bytes[ReadPos++];
        }
        public sbyte GetSByte() {
            if (UnreadLength < 1) {
                Debug.LogError(NotEnoughBytesError("sbyte"));
                return 0;
            }

            return (sbyte)Bytes[ReadPos++];
        }
        public bool GetBool() {
            if (UnreadLength < 1) {
                Debug.LogError(NotEnoughBytesError("bool", "false"));
                return false;
            }

            return Bytes[ReadPos++] == 1;
        }
        public short GetShort() {
            if (UnreadLength < 2) {
                Debug.LogError(NotEnoughBytesError("short"));
                return 0;
            }

            short result = Converter.ToShort(Bytes, ReadPos);
            ReadPos += 2;
            return result;
        }
        public ushort GetUShort() {
            if (UnreadLength < 2) {
                Debug.LogError(NotEnoughBytesError("ushort"));
                return 0;
            }

            ushort result = Converter.ToUShort(Bytes, ReadPos);
            ReadPos += 2;
            return result;
        }
        public int GetInt() {
            if (UnreadLength < 4) {
                Debug.LogError(NotEnoughBytesError("int"));
                return 0;
            }

            int result = Converter.ToInt(Bytes, ReadPos);
            ReadPos += 4;
            return result;
        }
        public uint GetUInt() {
            if (UnreadLength < 4) {
                Debug.LogError(NotEnoughBytesError("uint"));
                return 0u;
            }

            uint result = Converter.ToUInt(Bytes, ReadPos);
            ReadPos += 4;
            return result;
        }
        public long GetLong() {
            if (UnreadLength < 8) {
                Debug.LogError(NotEnoughBytesError("long"));
                return 0L;
            }

            long result = Converter.ToLong(Bytes, ReadPos);
            ReadPos += 8;
            return result;
        }
        public ulong GetULong() {
            if (UnreadLength < 8) {
                Debug.LogError(NotEnoughBytesError("ulong"));
                return 0uL;
            }

            ulong result = Converter.ToULong(Bytes, ReadPos);
            ReadPos += 8;
            return result;
        }
        public float GetFloat() {
            if (UnreadLength < 4) {
                Debug.LogError(NotEnoughBytesError("float"));
                return 0f;
            }

            float result = Converter.ToFloat(Bytes, ReadPos);
            ReadPos += 4;
            return result;
        }
        public double GetDouble() {
            if (UnreadLength < 8) {
                Debug.LogError(NotEnoughBytesError("double"));
                return 0.0;
            }

            double result = Converter.ToDouble(Bytes, ReadPos);
            ReadPos += 8;
            return result;
        }
        public string GetString() {
            if (!GetBool())
#pragma warning disable CS8603 // Possible null reference return.
                return null;
            ushort num = GetArrayLength();
            if (UnreadLength < num) {
                Debug.LogError(NotEnoughBytesError("string", "shortened string"));
                num = (ushort)UnreadLength;
            }

            string @string = Encoding.UTF8.GetString(Bytes, ReadPos, num);
            ReadPos += num;
            return @string;
        }
        public Vector3 GetVector3() {
            return new Vector3(GetFloat(), GetFloat(), GetFloat());
        }
        public Vector2 GetVector2() {
            return new Vector2(GetFloat(), GetFloat());
        }
        public Quaternion GetQuaternion() {
            return new Quaternion(GetFloat(), GetFloat(), GetFloat(), GetFloat());
        }

        public byte[] GetBytes() {
            return GetBytes(GetArrayLength());
        }
        public sbyte[] GetSBytes() {
            return GetSBytes(GetArrayLength());
        }
        public bool[] GetBools() {
            return GetBools(GetArrayLength());
        }
        public short[] GetShorts() {
            return GetShorts(GetArrayLength());
        }
        public ushort[] GetUShorts() {
            return GetUShorts(GetArrayLength());
        }
        public int[] GetInts() {
            return GetInts(GetArrayLength());
        }
        public uint[] GetUInts() {
            return GetUInts(GetArrayLength());
        }
        public long[] GetLongs() {
            return GetLongs(GetArrayLength());
        }
        public ulong[] GetULongs() {
            return GetULongs(GetArrayLength());
        }
        public float[] GetFloats() {
            return GetFloats(GetArrayLength());
        }
        public double[] GetDoubles() {
            return GetDoubles(GetArrayLength());
        }
        public string[] GetStrings() {
            if (!GetBool())
#pragma warning disable CS8603 // Possible null reference return.
                return null;
            return GetStrings(GetArrayLength());
        }

        public byte[] GetBytes(int amount) {
            byte[] array = new byte[amount];
            ReadBytes(amount, array);
            return array;
        }
        public sbyte[] GetSBytes(int amount) {
            sbyte[] array = new sbyte[amount];
            ReadSBytes(amount, array);
            return array;
        }
        public bool[] GetBools(int amount) {
            bool[] array = new bool[amount];
            int num = amount / 8 + (amount % 8 != 0 ? 1 : 0);
            if (UnreadLength < num) {
                Debug.LogError(NotEnoughBytesError(array.Length, "bool"));
                num = UnreadLength;
            }

            ReadBools(num, array);
            return array;
        }
        public short[] GetShorts(int amount) {
            short[] array = new short[amount];
            ReadShorts(amount, array);
            return array;
        }
        public ushort[] GetUShorts(int amount) {
            ushort[] array = new ushort[amount];
            ReadUShorts(amount, array);
            return array;
        }
        public int[] GetInts(int amount) {
            int[] array = new int[amount];
            ReadInts(amount, array);
            return array;
        }
        public uint[] GetUInts(int amount) {
            uint[] array = new uint[amount];
            ReadUInts(amount, array);
            return array;
        }
        public long[] GetLongs(int amount) {
            long[] array = new long[amount];
            ReadLongs(amount, array);
            return array;
        }
        public ulong[] GetULongs(int amount) {
            ulong[] array = new ulong[amount];
            ReadULongs(amount, array);
            return array;
        }
        public float[] GetFloats(int amount) {
            float[] array = new float[amount];
            ReadFloats(amount, array);
            return array;
        }
        public double[] GetDoubles(int amount) {
            double[] array = new double[amount];
            ReadDoubles(amount, array);
            return array;
        }
        public string[] GetStrings(int amount) {
            string[] array = new string[amount];
            for (int i = 0; i < array.Length; i++) {
                array[i] = GetString();
            }

            return array;
        }

        public IMessage AddClass<T>(T value) where T : class, IMessageSerializable, new() {
            if (value == null) {
                Add(false);
                return this;
            }
            Add(true);
            value.Serialize(this);
            return this;
        }
        public T GetClass<T>() where T : class, IMessageSerializable, new() {
            if (!GetBool()) return null;
            var newT = new T();
            newT.Deserialize(this);
            return newT;
        }

        public IMessage Clear(byte[] bytes = null) {
            ReadPos = 0;
            WritePos = 0;

            if (bytes == null) return this;
            for (int i = 0; i < bytes.Length; i++) {
                Add(bytes[i]);
            }
            return this;
        }

        private void ReadBytes(int amount, byte[] intoArray, int startIndex = 0) {
            if (UnreadLength < amount) {
                Debug.LogError(NotEnoughBytesError(intoArray.Length, "byte"));
                amount = UnreadLength;
            }

            Array.Copy(Bytes, ReadPos, intoArray, startIndex, amount);
            ReadPos += (ushort)amount;
        }
        private void ReadSBytes(int amount, sbyte[] intoArray, int startIndex = 0) {
            if (UnreadLength < amount) {
                Debug.LogError(NotEnoughBytesError(intoArray.Length, "sbyte"));
                amount = UnreadLength;
            }

            for (int i = 0; i < amount; i++) {
                intoArray[startIndex + i] = (sbyte)Bytes[ReadPos++];
            }
        }
        private void ReadBools(int byteAmount, bool[] intoArray, int startIndex = 0) {
            bool flag = intoArray.Length % 8 == 0;
            for (int i = 0; i < byteAmount; i++) {
                int num = 8;
                if (i + 1 == byteAmount && !flag) num = intoArray.Length % 8;

                for (int j = 0; j < num; j++) {
                    intoArray[startIndex + i * 8 + j] = (Bytes[ReadPos + i] >> j & 1) == 1;
                }
            }

            ReadPos += (ushort)byteAmount;
        }
        private void ReadShorts(int amount, short[] intoArray, int startIndex = 0) {
            if (UnreadLength < amount * 2) {
                Debug.LogError(NotEnoughBytesError(intoArray.Length, "short"));
                amount = UnreadLength / 2;
            }

            for (int i = 0; i < amount; i++) {
                intoArray[startIndex + i] = Converter.ToShort(Bytes, ReadPos);
                ReadPos += 2;
            }
        }
        private void ReadUShorts(int amount, ushort[] intoArray, int startIndex = 0) {
            if (UnreadLength < amount * 2) {
                Debug.LogError(NotEnoughBytesError(intoArray.Length, "ushort"));
                amount = UnreadLength / 2;
            }

            for (int i = 0; i < amount; i++) {
                intoArray[startIndex + i] = Converter.ToUShort(Bytes, ReadPos);
                ReadPos += 2;
            }
        }
        private void ReadInts(int amount, int[] intoArray, int startIndex = 0) {
            if (UnreadLength < amount * 4) {
                Debug.LogError(NotEnoughBytesError(intoArray.Length, "int"));
                amount = UnreadLength / 4;
            }

            for (int i = 0; i < amount; i++) {
                intoArray[startIndex + i] = Converter.ToInt(Bytes, ReadPos);
                ReadPos += 4;
            }
        }
        private void ReadUInts(int amount, uint[] intoArray, int startIndex = 0) {
            if (UnreadLength < amount * 4) {
                Debug.LogError(NotEnoughBytesError(intoArray.Length, "uint"));
                amount = UnreadLength / 4;
            }

            for (int i = 0; i < amount; i++) {
                intoArray[startIndex + i] = Converter.ToUInt(Bytes, ReadPos);
                ReadPos += 4;
            }
        }
        private void ReadLongs(int amount, long[] intoArray, int startIndex = 0) {
            if (UnreadLength < amount * 8) {
                Debug.LogError(NotEnoughBytesError(intoArray.Length, "long"));
                amount = UnreadLength / 8;
            }

            for (int i = 0; i < amount; i++) {
                intoArray[startIndex + i] = Converter.ToLong(Bytes, ReadPos);
                ReadPos += 8;
            }
        }
        private void ReadULongs(int amount, ulong[] intoArray, int startIndex = 0) {
            if (UnreadLength < amount * 8) {
                Debug.LogError(NotEnoughBytesError(intoArray.Length, "ulong"));
                amount = UnreadLength / 8;
            }

            for (int i = 0; i < amount; i++) {
                intoArray[startIndex + i] = Converter.ToULong(Bytes, ReadPos);
                ReadPos += 8;
            }
        }
        private void ReadFloats(int amount, float[] intoArray, int startIndex = 0) {
            if (UnreadLength < amount * 4) {
                Debug.LogError(NotEnoughBytesError(intoArray.Length, "float"));
                amount = UnreadLength / 4;
            }

            for (int i = 0; i < amount; i++) {
                intoArray[startIndex + i] = Converter.ToFloat(Bytes, ReadPos);
                ReadPos += 4;
            }
        }
        private void ReadDoubles(int amount, double[] intoArray, int startIndex = 0) {
            if (UnreadLength < amount * 8) {
                Debug.LogError(NotEnoughBytesError(intoArray.Length, "double"));
                amount = UnreadLength / 8;
            }

            for (int i = 0; i < amount; i++) {
                intoArray[startIndex + i] = Converter.ToDouble(Bytes, ReadPos);
                ReadPos += 8;
            }
        }

        private void AddArrayLength(int length) {
            if (UnwrittenLength < 1) throw new InsufficientCapacityException(this, "array length", length <= 127 ? 1 : 2);

            if (length <= 127) {
                Bytes[WritePos++] = (byte)length;
                return;
            }

            if (length > 32767) throw new ArgumentOutOfRangeException("length", $"Messages do not support auto-inclusion of array lengths for arrays with more than {32767} elements! Either send a smaller array or set the 'includeLength' paremeter to false in the AddNullable method and manually pass the array length to the Get method.");

            if (UnwrittenLength < 2) throw new InsufficientCapacityException(this, "array length", 2);

            length |= 0x8000;
            Bytes[WritePos++] = (byte)(length >> 8);
            Bytes[WritePos++] = (byte)length;
        }
        private ushort GetArrayLength() {
            if (UnreadLength < 1) {
                Debug.LogError(NotEnoughBytesError("array length"));
                return 0;
            }

            if ((Bytes[ReadPos] & 0x80) == 0) return GetByte();

            if (UnreadLength < 2) {
                Debug.LogError(NotEnoughBytesError("array length"));
                return 0;
            }

            return (ushort)((uint)(Bytes[ReadPos++] << 8 | Bytes[ReadPos++]) & 0x7FFFu);
        }
        private string NotEnoughBytesError(string valueName, string defaultReturn = "0") {
            return string.Format("Message only contains {0} unread {1}, which is not enough to retrieve a value of type '{2}'! Returning {3}.", UnreadLength, Helper.CorrectForm(UnreadLength, "byte"), valueName, defaultReturn);
        }
        private string NotEnoughBytesError(int arrayLength, string valueName) {
            return string.Format("Message only contains {0} unread {1}, which is not enough to retrieve {2} {3}! Returned array will contain default elements.", UnreadLength, Helper.CorrectForm(UnreadLength, "byte"), arrayLength, Helper.CorrectForm(arrayLength, valueName));
        }
    }
}
