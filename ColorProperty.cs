using UnityEngine;

namespace ShaderWizard {
    internal class ColorProperty : ShaderProperty {

        public ColorProperty() {
            DefaultValue = new Color(0, 0, 0);
        }

        public Color DefaultValue { get; set; }

        public override PropertyType PropertyType {
            get { return PropertyType.Color; }
        }
    }
}