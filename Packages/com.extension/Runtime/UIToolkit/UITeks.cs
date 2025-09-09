using System;
using UnityEngine.UIElements;

namespace ui {
    public class UITeks : UIComponent<Label> {

        public void SetValue(string value) {
            component.text = value;
        }
    }

}
