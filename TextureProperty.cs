using UnityEngine;

namespace ShaderWizard {
    internal class TextureProperty : ShaderProperty {
        public TextureProperty() {
            TextureType = TextureType.TwoD;
            TexGenMode = TexGenMode.None;
            DefaultValue = DefaultTexture.Empty;
        }

        public TextureType TextureType { get; set; }
        public TexGenMode TexGenMode { get; set; }
        public DefaultTexture DefaultValue { get; set; }

        public override PropertyType PropertyType {
            get { return PropertyType.Texture; }
        }
    }
}