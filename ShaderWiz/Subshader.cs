using UnityEngine;

namespace ShaderWiz {
    internal abstract class Subshader : ScriptableObject {
        protected Subshader() {
            RenderPosition = RenderPosition.Geometry;
            IgnoreProjector = false;
            ForceNoShadowCasting = false;
        }

        private void OnEnable() {
            hideFlags = HideFlags.HideAndDontSave;
        }

        public abstract SubshaderType SubshaderType { get; }
        public bool ForceNoShadowCasting { get; set; }
        public bool IgnoreProjector { get; set; }
        public RenderPosition RenderPosition { get; set; }
    }
}