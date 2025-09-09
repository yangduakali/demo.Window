using System;
using System.Runtime.CompilerServices;

namespace network.client.message {

    public class Converter {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromShort(short value, byte[] array, int startIndex) {
            array[startIndex] = (byte)value;
            array[startIndex + 1] = (byte)(value >> 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromUShort(ushort value, byte[] array, int startIndex) {
            array[startIndex] = (byte)value;
            array[startIndex + 1] = (byte)(value >> 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ToShort(byte[] array, int startIndex) {
            return (short)(array[startIndex] | array[startIndex + 1] << 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ToUShort(byte[] array, int startIndex) {
            return (ushort)(array[startIndex] | array[startIndex + 1] << 8);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromInt(int value, byte[] array, int startIndex) {
            array[startIndex] = (byte)value;
            array[startIndex + 1] = (byte)(value >> 8);
            array[startIndex + 2] = (byte)(value >> 16);
            array[startIndex + 3] = (byte)(value >> 24);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromUInt(uint value, byte[] array, int startIndex) {
            array[startIndex] = (byte)value;
            array[startIndex + 1] = (byte)(value >> 8);
            array[startIndex + 2] = (byte)(value >> 16);
            array[startIndex + 3] = (byte)(value >> 24);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(byte[] array, int startIndex) {
            return array[startIndex] | array[startIndex + 1] << 8 | array[startIndex + 2] << 16 | array[startIndex + 3] << 24;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToUInt(byte[] array, int startIndex) {
            return (uint)(array[startIndex] | array[startIndex + 1] << 8 | array[startIndex + 2] << 16 | array[startIndex + 3] << 24);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromLong(long value, byte[] array, int startIndex) {
            array[startIndex] = (byte)value;
            array[startIndex + 1] = (byte)(value >> 8);
            array[startIndex + 2] = (byte)(value >> 16);
            array[startIndex + 3] = (byte)(value >> 24);
            array[startIndex + 4] = (byte)(value >> 32);
            array[startIndex + 5] = (byte)(value >> 40);
            array[startIndex + 6] = (byte)(value >> 48);
            array[startIndex + 7] = (byte)(value >> 56);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromULong(ulong value, byte[] array, int startIndex) {
            array[startIndex] = (byte)value;
            array[startIndex + 1] = (byte)(value >> 8);
            array[startIndex + 2] = (byte)(value >> 16);
            array[startIndex + 3] = (byte)(value >> 24);
            array[startIndex + 4] = (byte)(value >> 32);
            array[startIndex + 5] = (byte)(value >> 40);
            array[startIndex + 6] = (byte)(value >> 48);
            array[startIndex + 7] = (byte)(value >> 56);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ToLong(byte[] array, int startIndex) {
            return BitConverter.ToInt64(array, startIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ToULong(byte[] array, int startIndex) {
            return (ulong)BitConverter.ToInt64(array, startIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromFloat(float value, byte[] array, int startIndex) {
            FloatConverter floatConverter = default;
            floatConverter.FloatValue = value;
            FloatConverter floatConverter2 = floatConverter;
            array[startIndex] = floatConverter2.Byte0;
            array[startIndex + 1] = floatConverter2.Byte1;
            array[startIndex + 2] = floatConverter2.Byte2;
            array[startIndex + 3] = floatConverter2.Byte3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToFloat(byte[] array, int startIndex) {
            FloatConverter floatConverter = default;
            floatConverter.Byte0 = array[startIndex];
            floatConverter.Byte1 = array[startIndex + 1];
            floatConverter.Byte2 = array[startIndex + 2];
            floatConverter.Byte3 = array[startIndex + 3];
            return floatConverter.FloatValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromDouble(double value, byte[] array, int startIndex) {
            DoubleConverter doubleConverter = default;
            doubleConverter.DoubleValue = value;
            DoubleConverter doubleConverter2 = doubleConverter;
            array[startIndex] = doubleConverter2.Byte0;
            array[startIndex + 1] = doubleConverter2.Byte1;
            array[startIndex + 2] = doubleConverter2.Byte2;
            array[startIndex + 3] = doubleConverter2.Byte3;
            array[startIndex + 4] = doubleConverter2.Byte4;
            array[startIndex + 5] = doubleConverter2.Byte5;
            array[startIndex + 6] = doubleConverter2.Byte6;
            array[startIndex + 7] = doubleConverter2.Byte7;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDouble(byte[] array, int startIndex) {
            return BitConverter.ToDouble(array, startIndex);
        }
    }

}
