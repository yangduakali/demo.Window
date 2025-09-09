using System;
using UnityEngine;

namespace network.client.ultis {
    internal static class Helper {
        internal static string CorrectForm(int amount, string singular, string plural = "") {
            if (string.IsNullOrEmpty(plural)) plural = singular + "s";

            if (amount != 1) return plural;

            return singular;
        }
        internal static int GetSequenceGap(ushort seqId1, ushort seqId2) {
            int num = seqId1 - seqId2;
            if (Math.Abs(num) <= 32768) return num;

            return (seqId1 <= 32768 ? 65536 + seqId1 : seqId1) - (seqId2 <= 32768 ? 65536 + seqId2 : seqId2);
        }
        public static ushort GetComponentId<T>()where T : Component {
            if (typeof(T) == typeof(Transform)) return 1;
            if (typeof(T) == typeof(Animator)) return 2;
            return 0;
        }
    }

}

