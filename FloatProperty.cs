namespace ShaderWizard {
    internal class FloatProperty : ShaderProperty {

        public FloatProperty() {
            DefaultValue = 0;
        }  

        public float DefaultValue { get; set; }

        public override PropertyType PropertyType {
            get { return PropertyType.Float; }
        }
    }
}