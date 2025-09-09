using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine;

namespace ui {
    public class UIIntField : UIComponent<TextField> {

        public int Value {
            get => _value;
            set {
                _value = value;
                component.value = _value.ToString();
                OnValueChange?.Invoke(_value);
            }
        }

        [Space(30)]
        [SerializeField] private int _value = 0;
        public bool useNumber;
        [Space(30)]
        public UnityEvent<int> OnValueChange;

        protected override void Awake() {
            base.Awake();
            component.isDelayed = true;
            component.value = _value.ToString();
            component.RegisterValueChangedCallback(x => OnValueChangeAction(int.Parse( x.newValue)));
        }

        private void OnValueChangeAction(int value) {
            Value = value;
        }


    }


}

