using UnityEngine;

namespace ShaderWiz {
    internal class Pass : ScriptableObject {
        private void OnEnable() {
            hideFlags = HideFlags.HideAndDontSave;
        }
    }
}