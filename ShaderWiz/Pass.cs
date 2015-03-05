using UnityEngine;

namespace ShaderWiz {
    internal abstract class Pass : ScriptableObject {
        private void OnEnable() {
            hideFlags = HideFlags.HideAndDontSave;
        }

        public abstract PassType PassType { get; }
    }
}