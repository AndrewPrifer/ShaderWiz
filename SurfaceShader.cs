namespace ShaderWizard {
    internal class SurfaceShader : Subshader {

        public SurfaceShader() {
            // Include settings
            CgincTessellation = false;
            CgincTerrainEngine = false;
            CgincLighting = false;
            CgincAutoLight = false;
            CgincUnityCg = false;

            // Misc settings
            RenderPosition = RenderPosition.Geometry;
            IgnoreProjector = false;
            ForceNoShadowCasting = false;
            DisableWhenSoftVegIsOff = false;
            AlphaBlended = false;
            CommentFinal = false;

            // Shadow settings
            SupportAllShadowTypes = false;
            AddShadowPasses = false;

            // Lightmap settings
            DualForward = false;
            SupportDirectionalLightmaps = true;
            SupportLightmaps = true;

            // Lighting settings
            ViewDirPerVert = false;
            EnableAdditivePass = true;
            ApplyVertexLights = true;
            ApplyAmbient = true;
            OutNormalSpecified = false;

            // Input settings
            WorldNormalInInput = false;
            WorldReflectionVectorInInput = false;
            SsPositionInInput = false;
            ViewDirInInput = false;
            VertColorInInput = false;
            UvInInput = true;

            // Custom function settings
            ViewDirInDirectional = false;
            ViewDirInDual = false;
            ViewDirInSingle = false;
            UseDirectionalLightMap = false;
            UseDualLightMap = false;
            UseSingleLightMap = false;
            UsePrePass = false;
            IncludeViewDirInLighting = false;
            LightingModel = LightingModel.Lambert;
            UseBuiltinLighting = true;
            PassHalfDirInLighting = false;
            UseTessellation = false;
            UseFinalColorModifier = false;
            CustomDataPerVertex = false;
            WsPositionInInput = false;
            UseVertexModifier = false;
        }

        public bool UseVertexModifier { get; set; }
        public bool WsPositionInInput { get; set; }
        public bool CustomDataPerVertex { get; set; }
        public bool UseFinalColorModifier { get; set; }
        public bool UseTessellation { get; set; }
        public bool PassHalfDirInLighting { get; set; }
        public bool UseBuiltinLighting { get; set; }
        public LightingModel LightingModel { get; set; }
        public bool IncludeViewDirInLighting { get; set; }
        public bool UsePrePass { get; set; }
        public bool UseSingleLightMap { get; set; }
        public bool UseDualLightMap { get; set; }
        public bool UseDirectionalLightMap { get; set; }
        public bool ViewDirInSingle { get; set; }
        public bool ViewDirInDual { get; set; }
        public bool ViewDirInDirectional { get; set; }
        public bool VertColorInInput { get; set; }
        public bool ViewDirInInput { get; set; }
        public bool SsPositionInInput { get; set; }
        public bool WorldReflectionVectorInInput { get; set; }
        public bool WorldNormalInInput { get; set; }
        public bool UvInInput { get; set; }
        public bool OutNormalSpecified { get; set; }
        public bool ApplyAmbient { get; set; }
        public bool ApplyVertexLights { get; set; }
        public bool EnableAdditivePass { get; set; }
        public bool ViewDirPerVert { get; set; }
        public bool SupportLightmaps { get; set; }
        public bool SupportDirectionalLightmaps { get; set; }
        public bool DualForward { get; set; }
        public bool AddShadowPasses { get; set; }
        public bool SupportAllShadowTypes { get; set; }
        public bool AlphaBlended { get; set; }
        public bool DisableWhenSoftVegIsOff { get; set; }
        public bool ForceNoShadowCasting { get; set; }
        public bool IgnoreProjector { get; set; }
        public RenderPosition RenderPosition { get; set; }
        public bool CommentFinal { get; set; }
        public bool CgincUnityCg { get; set; }
        public bool CgincAutoLight { get; set; }
        public bool CgincLighting { get; set; }
        public bool CgincTerrainEngine { get; set; }
        public bool CgincTessellation { get; set; }

        public override SubshaderType SubshaderType {
            get { return SubshaderType.Surface; }
        }
    }
}