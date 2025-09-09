using UnityEngine;
using UnityEngine.UIElements;

namespace ui {
    public abstract class UIComponent<T> : MonoBehaviour where T : VisualElement {

        public UIDocument uIDocument;

        protected VisualElement root;
        protected T component;

        protected virtual void Awake() {
            uIDocument = GetComponentInParent<UIDocument>();
            root = uIDocument.rootVisualElement;
            component = root.Q<T>(gameObject.name);
        }

        protected virtual void OnValidate() {
            if(uIDocument == null)
            uIDocument = GetComponentInParent<UIDocument>();
        }

        private void OnDisable() {
            if (component != null) component.style.display = DisplayStyle.None;
        }

        private void OnEnable() {
            if (component != null) component.style.display = DisplayStyle.Flex;
        }
    }
}
