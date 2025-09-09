using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace value {
    [CreateAssetMenu(menuName = "Value/Type", fileName = "New Type")]
    public class ValueType : ScriptableObject {
        public Type Type => Type.GetType($"{@namespace}.{@class}, {asmdef}");
        public string @namespace;
        public string @class;
        public string asmdef;

        private void OnValidate() {
            

        }

        public static implicit operator Type (ValueType valueType) {
            return valueType.Type;
        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ValueType))]
    public class ValueTypeEditor : Editor {


        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            //var ValueType = (ValueType)target;
            //var path = AssetDatabase.GetAssetPath(ValueType);
            //var cor = path.Replace(ValueType.name + ".asset","");
            //if (string.IsNullOrWhiteSpace(ValueType.@namespace) || string.IsNullOrWhiteSpace(ValueType.@class) || string.IsNullOrWhiteSpace(ValueType.asmdef)) {
            //    ValueType.name = AssetDatabase.GenerateUniqueAssetPath(cor + "Null Type.asset");
            //}
        }

        private void OnValidate() {
            Debug.Log("Ad");
            var ValueType = (ValueType)target;
            var path = AssetDatabase.GetAssetPath(ValueType);
            var cor = path.Replace(ValueType.name + ".asset","");
            if (string.IsNullOrWhiteSpace(ValueType.@namespace) || string.IsNullOrWhiteSpace(ValueType.@class) || string.IsNullOrWhiteSpace(ValueType.asmdef)) {
                ValueType.name = AssetDatabase.GenerateUniqueAssetPath(cor + "Null Type.asset");
            }
        }
    }

#endif
}
