using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;

namespace ui {
    public class UITextField : UIComponent<TextField> {

        public string Value {
            get => _value;
            set {
                _value = value;
                component.value = _value;
                OnValueChange?.Invoke(_value);
            }
        }

        [Space(30)]
        [SerializeField] private string _value = "Text";
        public bool useNumber;
        [Space(30)]
        public UnityEvent<string> OnValueChange;

        protected override void Awake() {
            base.Awake();
            component.isDelayed = true; 
            component.value = _value;
            component.RegisterValueChangedCallback(x => OnValueChangeAction(x.newValue));
        }

        private void OnValueChangeAction(string value) {
            Value = value;
        }

        
    }


}

