using UnityEngine;

namespace ShaderWizard {
    internal abstract class ShaderProperty : ScriptableObject {

        private void OnEnable() {
            hideFlags = HideFlags.HideAndDontSave;
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public abstract PropertyType PropertyType { get; }
    }
}