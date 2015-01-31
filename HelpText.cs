namespace ShaderWizard {
    internal static class HelpText {
        public const string ShaderName = "Your shader will appear in the material inspector listed under Name.";

        public const string ShaderProperties =
            "Shaders can have a list of properties. Any properties declared in a shader are shown in the material inspector inside Unity. Typical properties are the object color, textures, or just arbitrary values to be used by the shader.";

        public const string Subshaders =
            "Each shader is comprised of a list of sub-shaders. You must have at least one. When loading a shader, Unity will go through the list of subshaders, and pick the first one that is supported by the end user’s machine. If no subshaders are supported, Unity will try to use fallback shader.";

        public const string Fallback =
            "After all Subshaders a Fallback can be defined. It basically says 'if none of subshaders can run on this hardware, try using the ones from another shader'.";

        public const string VertexModifier =
            "It is possible to use a “vertex modifier” function that will modify incoming vertex data in the vertex shader. This can be used for procedural animation, extrusion along normals and so on.";

        public const string CustomDataPerVertex =
            "Using a vertex modifier function it is also possible to compute custom data in a vertex shader, which then will be passed to the surface shader function per-pixel.";

        public const string FinalColorModifier =
            "It is possible to use a “final color modifier” function that will modify final color computed by the shader.";

        public const string Tessellation =
            "Surface Shaders have some support for DirectX 11 GPU Tessellation. Vertex modifier is invoked after tessellation. Limited to triangle domain and shader model 5.";

        public const string HalfDirInLighting =
            "Pass half-direction vector into the lighting function instead of view-direction. Half-direction will be computed and normalized per vertex. This is faster, but not entirely correct.";

        public const string BuiltinLighting =
            "Built-in lighting models are Lambert (diffuse) and BlinnPhong (specular).";

        public const string CustomLighting = "Allows you to implement your own lighting model.";

        public const string SingleLightmap =
            "All static illumination (i.e. from baked only and auto lights, sky lights and emissive materials) gets baked into one set of lightmaps. These lightmaps are used on all lightmapped objects regardless of shadow distance.";

        public const string DualLightmap =
            "Dual lightmaps is Unity’s approach to make lightmapping work with specular, normal mapping and proper blending of baked and realtime shadows. It’s also a way to make your lightmaps look good even if the lightmap resolution is low. Use the dualforward surface shader directive to enable them in the forward rendering path.";

        public const string DirectionalLightmap =
            "Directional lightmaps store information on light directionality in a second set of lightmaps. The first set of lightmaps (color) stores just the diffuse lighting, same as Single Lightmaps do. The second set of lightmaps (scale) stores the ratio of the desaturated incoming light per basis vector.";

        public const string OutputNormalSpecified =
            "If you write to o.Normal and want to use reflection vector or normal vector based on per-pixel normal map (WorldReflectionVector (IN, o.Normal), WorldNormalVector (IN, o.Normal)), then you MUST check this checkbox.";

        public const string ForwardAdditive =
            "Disabling it makes the shader support one full directional light, with all other lights computed per-vertex/SH.";
    }
}