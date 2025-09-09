using UnityEngine;

namespace network.client.message {
    public enum SendMode {
        Unreliable,
        Reliable
    }
    public interface IMessage {
        SendMode SendMode { get; }
        byte[] Bytes { get; }
        byte[] WritedBytes { get; }
        int UnreadLength { get; }
        int UnwrittenLength { get; }
        int WrittenLength { get; }

        IMessage Add(bool value);
        IMessage Add(byte value);
        IMessage Add(double value);
        IMessage Add(float value);
        IMessage Add(int value);
        IMessage Add(long value);
        IMessage Add(sbyte value);
        IMessage Add(short value);
        IMessage Add(string value);
        IMessage Add(uint value);
        IMessage Add(ulong value);
        IMessage Add(ushort value);

        IMessage Add(bool[] array, bool includeLength = true);
        IMessage Add(byte[] array, bool includeLength = true);
        IMessage Add(double[] array, bool includeLength = true);
        IMessage Add(float[] array, bool includeLength = true);
        IMessage Add(int[] array, bool includeLength = true);
        IMessage Add(long[] array, bool includeLength = true);
        IMessage Add(sbyte[] array, bool includeLength = true);
        IMessage Add(short[] array, bool includeLength = true);
        IMessage Add(string[] array, bool includeLength = true);
        IMessage Add(uint[] array, bool includeLength = true);
        IMessage Add(ulong[] array, bool includeLength = true);
        IMessage Add(ushort[] array, bool includeLength = true);
        IMessage Add(Vector3 value);
        IMessage Add(Vector2 value);
        IMessage Add(Quaternion value);

        bool GetBool();
        byte GetByte();
        double GetDouble();
        float GetFloat();
        int GetInt();
        long GetLong();
        sbyte GetSByte();
        short GetShort();
        string GetString();
        uint GetUInt();
        ushort GetUShort();
        ulong GetULong();
        Vector3 GetVector3();
        Vector2 GetVector2();
        Quaternion GetQuaternion();

        bool[] GetBools();
        byte[] GetBytes();
        double[] GetDoubles();
        float[] GetFloats();
        int[] GetInts();
        long[] GetLongs();
        sbyte[] GetSBytes();
        short[] GetShorts();
        string[] GetStrings();
        uint[] GetUInts();
        ulong[] GetULongs();
        ushort[] GetUShorts();

        bool[] GetBools(int amount);
        byte[] GetBytes(int amount);
        double[] GetDoubles(int amount);
        float[] GetFloats(int amount);
        int[] GetInts(int amount);
        long[] GetLongs(int amount);
        sbyte[] GetSBytes(int amount);
        short[] GetShorts(int amount);
        string[] GetStrings(int amount);
        uint[] GetUInts(int amount);
        ulong[] GetULongs(int amount);
        ushort[] GetUShorts(int amount);

        IMessage AddClass<T>(T value) where T : class, IMessageSerializable, new();
        T GetClass<T>() where T : class, IMessageSerializable, new();

        IMessage Clear(byte[] bytes = null);
        void Release();
    }
}