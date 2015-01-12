using UnityEngine;

namespace ShaderWizard {
    internal abstract class Subshader : ScriptableObject {
        private void OnEnable() {
            hideFlags = HideFlags.HideAndDontSave;
        }

        public abstract SubshaderType SubshaderType { get; }
    }
}