using UnityEngine;

namespace ShaderWiz {
    internal abstract class Subshader : ScriptableObject {
        private void OnEnable() {
            hideFlags = HideFlags.HideAndDontSave;
        }

        public abstract SubshaderType SubshaderType { get; }
    }
}