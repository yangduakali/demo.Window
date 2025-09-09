using UnityEngine.Events;
using UnityEngine.UIElements;

namespace ui {
    public class UIButton : UIComponent<Button> {

        public UnityEvent OnClick;

        protected override void Awake() {
            base.Awake();
            component.clicked += OnClickAction;
        }

        private void OnClickAction() {
            OnClick?.Invoke();
        }
    }

}
