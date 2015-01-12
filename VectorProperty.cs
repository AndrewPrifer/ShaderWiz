using UnityEngine;

namespace ShaderWizard {
    internal class VectorProperty : ShaderProperty {
        public VectorProperty() {
            DefaultValue = new Vector4(0, 0, 0, 0);
        }

        public Vector4 DefaultValue { get; set; }

        public override PropertyType PropertyType {
            get { return PropertyType.Vector; }
        }
    }
}