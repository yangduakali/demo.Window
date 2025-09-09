using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace network.client {
    public static class ClientUltis {
        public static long TotalByteSend;
        public static long TotalByteRecive;

        public static byte[] AddSendByte(this byte[] bytes) {
            for (int i = 0; i < bytes.Length; i++) {
                TotalByteSend += bytes[i];
            }
            return bytes;
        }

        public static byte[] AddReciveByte(this byte[] bytes) {
            for (int i = 0; i < bytes.Length; i++) {
                TotalByteRecive += bytes[i];
            }
            return bytes;
        }
    }
}
