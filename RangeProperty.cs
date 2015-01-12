namespace ShaderWizard {
    internal class RangeProperty : ShaderProperty {
        public RangeProperty() {
            MinValue = -10;
            MaxValue = 10;
            DefaultValue = 0;
        }

        public float MinValue { get; set; }
        public float DefaultValue { get; set; }
        public float MaxValue { get; set; }

        public override PropertyType PropertyType {
            get { return PropertyType.Range; }
        }
    }
}