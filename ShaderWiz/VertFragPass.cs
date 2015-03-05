using System.Collections.Generic;
using UnityEngine.Rendering;

namespace ShaderWiz {
    internal class VertFragPass : Pass {
        public VertFragPass() {
            WriteToDepthBuffer = true;
            ZTestFunc = CompareFunction.LessEqual;
            UseDepthOffset = false;
            DepthFactor = 0;
            DepthUnit = 0;
//            AlphaTestFunc = CompareFunction.Always;
//            UseAlphaTest = false;
//            AlphaTestValue = 0;
            ApplyBlending = false;
            SourceBlendFactor = BlendMode.SrcAlpha;
            DestBlendFactor = BlendMode.OneMinusSrcAlpha;
            BlendAlphaSeparately = false;
            AlphaSourceBlendFactor = BlendMode.One;
            AlphaDestBlendFactor = BlendMode.One;
            BlendOp = BlendOp.Add;
            UseGeometryShader = false;
            InputTopology = InputTopology.Point;
            OutputTopology = OutputTopology.TriangleStream;
            MaxVertCount = 0;
            CompileToGlsl = false;
            AutoNormalizeVectors = true;
            GenerateDebugInfo = false;
            ShaderTarget = ShaderTarget.ShaderModel2;
            ExcludedRenderers = new List<Renderer>();
            LightMode = LightMode.Always;
            RequireSoftVegatation = false;
            FaceCullMode = CullMode.Back;
            ApplyFog = true;
            AllowExternalReference = false;
            UsePresetInput = true;
            InputPreset = VertexInputPreset.AppdataFull;
            UsePosition = true;
            UseNormal = true;
            UseTexcoord = true;
            UseTexcoord1 = true;
            UseTangent = true;
            UseColor = true;

            CgincAutoLight = false;
            CgincLighting = false;
            CgincTerrainEngine = false;
            CgincTessellation = false;
            CgincUnityCg = true;
        }

        public bool WriteToDepthBuffer { get; set; }
        public CompareFunction ZTestFunc { get; set; }
        public bool UseDepthOffset { get; set; }
        public float DepthFactor { get; set; }
        public float DepthUnit { get; set; }
//        public bool UseAlphaTest { get; set; }
//        public CompareFunction AlphaTestFunc { get; set; }
//        public float AlphaTestValue { get; set; }
        public bool ApplyBlending { get; set; }
        public BlendMode SourceBlendFactor { get; set; }
        public BlendMode DestBlendFactor { get; set; }
        public bool BlendAlphaSeparately { get; set; }
        public BlendMode AlphaSourceBlendFactor { get; set; }
        public BlendMode AlphaDestBlendFactor { get; set; }
        public BlendOp BlendOp { get; set; }
        public bool UseGeometryShader { get; set; }
        public InputTopology InputTopology { get; set; }
        public OutputTopology OutputTopology { get; set; }
        public int MaxVertCount { get; set; }
        public bool CompileToGlsl { get; set; }
        public bool AutoNormalizeVectors { get; set; }
        public bool GenerateDebugInfo { get; set; }
        public ShaderTarget ShaderTarget { get; set; }
        public List<Renderer> ExcludedRenderers { get; set; }
        public LightMode LightMode { get; set; }
        public bool RequireSoftVegatation { get; set; }
        public CullMode FaceCullMode { get; set; }
        public bool ApplyFog { get; set; }
        public bool AllowExternalReference { get; set; }
        public bool UsePresetInput { get; set; }
        public VertexInputPreset InputPreset { get; set; }
        public bool UsePosition { get; set; }
        public bool UseNormal { get; set; }
        public bool UseTexcoord { get; set; }
        public bool UseTexcoord1 { get; set; }
        public bool UseTangent { get; set; }
        public bool UseColor { get; set; }
        public bool CgincUnityCg { get; set; }
        public bool CgincAutoLight { get; set; }
        public bool CgincLighting { get; set; }
        public bool CgincTerrainEngine { get; set; }
        public bool CgincTessellation { get; set; }

        public override PassType PassType {
            get { return PassType.VertFrag; }
        }
    }
}