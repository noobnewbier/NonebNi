
using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Rendering.Toon;
using UnityEngine;
using UnityEngine.Rendering;

namespace NonebNi.Art.Toon
{
    [UsedImplicitly]
    internal partial class ToonShaderGUI : ShaderGUI
    {
        internal const string ShaderDefineShadinggrademap = "_SHADINGGRADEMAP";
        internal const string ShaderDefineAngelringOn = "_IS_ANGELRING_ON";
        internal const string ShaderPropAngelRing = "_AngelRing";
        internal const string ShaderPropMatCap = "_MatCap";
        internal const string ShaderPropClippingMode = "_ClippingMode";
        internal const string ShaderPropClippingMask = "_ClippingMask";
        internal const string ShaderPropSet1StShadePosition = "_Set_1st_ShadePosition";
        internal const string ShaderPropSet2NdShadePosition = "_Set_2nd_ShadePosition";
        internal const string ShaderPropShadingGradeMap = "_ShadingGradeMap";
        internal const string ShaderPropSetRimLightMask = "_Set_RimLightMask";
        internal const string ShaderPropSetHighColorMask = "_Set_HighColorMask";
        internal const string ShaderPropMatCapSampler = "_MatCap_Sampler";
        internal const string ShaderPropSetMatcapMask = "_Set_MatcapMask";

        internal const string ShaderPropUtsTechniqe = "_utsTechnique";
        internal const string ShaderPropAutoRenderQueue = "_AutoRenderQueue";
        internal const string ShaderPropStencilMode = "_StencilMode";
        internal const string ShaderPropStencilNo = "_StencilNo";
        internal const string ShaderPropTransparentEnabled = "_TransparentEnabled";
        internal const string ShaderPropStencilComp = "_StencilComp";
        internal const string ShaderPropStencilOpPass = "_StencilOpPass";
        internal const string ShaderPropStencilOpFail = "_StencilOpFail";
        internal const string ShaderPropNormalMapToHighColor = "_Is_NormalMapToHighColor";
        internal const string ShaderPropIsNormalMapToRimLight = "_Is_NormalMapToRimLight";
        internal const string ShaderPropSetSystemShadowsToBase = "_Set_SystemShadowsToBase";
        internal const string ShaderPropIsFilterHiCutPointLightColor = "_Is_Filter_HiCutPointLightColor";
        internal const string ShaderPropInverseClipping = "_Inverse_Clipping";
        internal const string ShaderPropIsBaseMapAlphaAsClippingMask = "_IsBaseMapAlphaAsClippingMask";
        internal const string ShaderPropIsLightColorBase = "_Is_LightColor_Base";
        internal const string ShaderPropCameraRollingStabilizer = "_CameraRolling_Stabilizer";
        internal const string ShaderPropIsOrtho = "_Is_Ortho";
        internal const string ShaderPropGIIntensity = "_GI_Intensity";


        internal const string ShaderPropIsLightColor1StShade = "_Is_LightColor_1st_Shade";
        internal const string ShaderPropIsLightColor2NdShade = "_Is_LightColor_2nd_Shade";
        internal const string ShaderPropIsLightColorHighColor = "_Is_LightColor_HighColor";
        internal const string ShaderPropIsLightColorRimLight = "_Is_LightColor_RimLight";
        internal const string ShaderPropIsLightColorApRimLight = "_Is_LightColor_Ap_RimLight";
        internal const string ShaderPropIsLightColorMatCap = "_Is_LightColor_MatCap";
        internal const string ShaderPropIsLightColorAR = "_Is_LightColor_AR";
        internal const string ShaderPropInvertMatcapMask = "_Inverse_MatcapMask";
        internal const string ShaderPropIsNormalMapToBase = "_Is_NormalMapToBase";
        internal const string ShaderPropIsColorShift = "_Is_ColorShift";
        internal const string ShaderPropRimLight = "_RimLight";
        internal const string ShaderPropRimLightFeatherOff = "_RimLight_FeatherOff";
        internal const string ShaderPropApRimLightFeatherOff = "_Ap_RimLight_FeatherOff";
        internal const string ShaderPropIsBlendAddToMatCap = "_Is_BlendAddToMatCap";
        internal const string ShaderPropARSamplerAlphaOn = "_ARSampler_AlphaOn";
        internal const string ShaderPropIsUseTweakHighColorOnShadow = "_Is_UseTweakHighColorOnShadow";

        internal const string ShaderPropIsSpecularToHighColor = "_Is_SpecularToHighColor";
        internal const string ShaderPropIsBlendAddToHiColor = "_Is_BlendAddToHiColor";

        internal const string ShaderPropAddAntipodeanRimLight = "_Add_Antipodean_RimLight";
        internal const string ShaderPropLightDirectionMaskOn = "_LightDirection_MaskOn";

        internal const string ShaderProp2NdShadeColorStep = "_2nd_ShadeColor_Step";
        internal const string ShaderPropShadeColorStep = "_ShadeColor_Step";
        internal const string ShaderProp2NdShadeColorFeather = "_2nd_ShadeColor_Feather";
        internal const string ShaderProp1St2NdShadesFeather = "_1st2nd_Shades_Feather";
        internal const string ShaderPropIsNormalMapForMatCap = "_Is_NormalMapForMatCap";
        internal const string ShaderPropIsUseTweakMatCapOnShadow = "_Is_UseTweakMatCapOnShadow";
        internal const string ShaderPropIsViewCoordScroll = "_Is_ViewCoord_Scroll";
        internal const string ShaderPropIsPingPongBase = "_Is_PingPong_Base";

        internal const string ShaderPropIsViewShift = "_Is_ViewShift";
        internal const string ShaderPropIsOutlineTex = "_Is_OutlineTex";


        internal const string ShaderDefineIsOutlineClippingNo = "_IS_OUTLINE_CLIPPING_NO";
        internal const string ShaderDefineIsOutlineClippingYes = "_IS_OUTLINE_CLIPPING_YES";

        internal const string ShaderDefineIsClippingOff = "_IS_CLIPPING_OFF";
        internal const string ShaderDefineIsClippingMode = "_IS_CLIPPING_MODE";
        internal const string ShaderDefineIsClippingTransmode = "_IS_CLIPPING_TRANSMODE";

        internal const string ShaderDefineIsTransclippingOff = "_IS_TRANSCLIPPING_OFF";
        internal const string ShaderDefineIsTransclippingOn = "_IS_TRANSCLIPPING_ON";


        protected readonly string[] UtsModeNames =
        {
            "Standard",
            "With Additional Control Maps"
        };
        protected readonly string[] EmissiveScrollMode =
        {
            "UV Coordinate Scroll",
            "View Coordinate Scroll"
        };
        protected readonly string[] ClippingModeNames =
        {
            "Off",
            "On",
            "Clip Transparency"
        };
        protected readonly string[] StencilModeNames =
        {
            "Off",
            "Draw If Not Equal to",
            "Replace Stencil Buffer with"
        };

        public enum UtsMode : uint
        {
            ThreeColorToon,
            ShadingGradeMap
        }

        public enum UtsClippingMode
        {
            Off,
            On,
            TransClippingMode
        }

        public enum UtsTransClippingMode
        {
            Off,
            On,
        }

        public enum UtsTransparentMode : uint
        {
            Off,
            On,
        }

        public enum UtsStencilMode
        {
            Off,
            StencilOut,
            StencilMask
        }

        public enum UtsSpeculerMode
        {
            Hard,
            Soft
        }

        public enum UtsSpeculerColorBlendMode
        {
            Multiply,
            Add
        }

        public enum UtsMatcapColorBlendMode
        {
            Multiply,
            Add
        }

        public enum StencilOperation
        {
            //https://docs.unity3d.com/Manual/SL-Stencil.html
            Keep, //    Keep the current contents of the buffer.
            Zero, //    Write 0 into the buffer.
            Replace, // Write the reference value into the buffer.
            IncrSat, // Increment the current value in the buffer. If the value is 255 already, it stays at 255.
            DecrSat, // Decrement the current value in the buffer. If the value is 0 already, it stays at 0.
            Invert, //  Negate all the bits.
            IncrWrap, //    Increment the current value in the buffer. If the value is 255 already, it becomes 0.
            DecrWrap, //    Decrement the current value in the buffer. If the value is 0 already, it becomes 255.
        }

        public enum StencilCompFunction
        {
            Disabled, //    Depth or stencil test is disabled.
            Never, //   Never pass depth or stencil test.
            Less, //   Pass depth or stencil test when new value is less than old one.
            Equal, //  Pass depth or stencil test when values are equal.
            LessEqual, // Pass depth or stencil test when new value is less or equal than old one.
            Greater, // Pass depth or stencil test when new value is greater than old one.
            NotEqual, //    Pass depth or stencil test when values are different.
            GreaterEqual, // Pass depth or stencil test when new value is greater or equal than old one.
            Always, //  Always pass depth or stencil test.
        }


        public enum OutlineMode
        {
            NormalDirection,
            PositionScaling
        }

        public enum CullingMode
        {
            Off,
            Frontface,
            Backface
        }

        public enum EmissionMode
        {
            SimpleEmissive,
            EmissiveAnimation
        }

        public enum CameraProjectionType
        {
            Perspective,
            Orthographic
        }

        [Flags]
        protected enum Expandable
        {
            Shader = 1 << 0,
            Basic = 1 << 1,
            ShadingStepAndFeather = 1 << 2,
            NormalMap = 1 << 3,
            Highlight = 1 << 4,
            Rimlight = 1 << 5,
            MatCap = 1 << 6,
            AngelRing = 1 << 7,
            Emission = 1 << 8,
            Outline = 1 << 9,
            TessellationLegacy = 1 << 10,
            SceneLight = 1 << 11,
            EnvironmentalLightEffectiveness = 1 << 12,
        }

        // variables which must be gotten from shader at the beggning of GUI
        internal int MAutoRenderQueue = 1;

        internal int MRenderQueue = (int)RenderQueue.Geometry;

        // variables which just to be held.
        internal OutlineMode MOutlineMode;
        internal CullingMode MCullingMode;
        internal EmissionMode MEmissionMode;
        internal bool MFirstTimeApply = true;
        private UtsMode _mWorkflowMode;


        private readonly UTS3MaterialHeaderScopeList _mMaterialScopeList = new (uint.MaxValue & ~(uint)Expandable.Basic);


        //Button sizes
        static internal GUILayoutOption[] longButtonStyle = new GUILayoutOption[]
        {
            GUILayout.Width(180)
        };
        static internal GUILayoutOption[] shortButtonStyle = new GUILayoutOption[]
        {
            GUILayout.Width(130)
        };
        static internal GUILayoutOption[] middleButtonStyle = new GUILayoutOption[]
        {
            GUILayout.Width(130)
        };
        static internal GUILayoutOption[] toggleStyle = new GUILayoutOption[]
        {
            GUILayout.Width(130)
        };

        //
        protected UtsTransparentMode transparencyEnabled;
        protected int stencilNumberSetting;

        protected static bool SimpleUI = false;

        //Initial values of Foldout.


        protected bool NormalMapFoldout = false;
        protected bool ShadowControlMapsFoldout;
        protected bool AdditionalLookdevsFoldout;
        protected bool AdvancedOutlineFoldout = false;


        // -----------------------------------------------------
        //Specify only those that use the m_MaterialEditor method as their UI.
        // UTS3 materal properties -------------------------
        protected MaterialProperty utsTechnique;
        protected MaterialProperty specularMode;
        protected MaterialProperty specularBlendMode;
        protected MaterialProperty matcapCameraMode;
        protected MaterialProperty matcapBlendMode;
        protected MaterialProperty matcapOrtho = null;
        protected MaterialProperty transparentMode;
        protected MaterialProperty clippingMode;
        protected MaterialProperty clippingMask;

        protected MaterialProperty stencilValue;

        protected MaterialProperty stencilMode;
        protected MaterialProperty mainTex;
        protected MaterialProperty baseColor;
        protected MaterialProperty firstShadeMap;
        protected MaterialProperty firstShadeColor;
        protected MaterialProperty secondShadeMap;
        protected MaterialProperty secondShadeColor;
        protected MaterialProperty normalMap;
        protected MaterialProperty bumpScale;
        protected MaterialProperty Set1StShadePosition;
        protected MaterialProperty Set2NdShadePosition;
        protected MaterialProperty shadingGradeMap;


        protected MaterialProperty HighColorTex;
        protected MaterialProperty highColor;

        protected MaterialProperty SetHighColorMask;
        protected MaterialProperty TweakHighColorMaskLevel;

        protected MaterialProperty RimLightPower = null;
        protected MaterialProperty APRimLightPower = null;
        protected MaterialProperty SetRimLightMask;

        protected MaterialProperty MatCapSampler;
        protected MaterialProperty matCapColor;
        protected MaterialProperty normalMapForMatCap;
        protected MaterialProperty bumpScaleMatcap;
        protected MaterialProperty SetMatcapMask;

        protected MaterialProperty AngelRingSampler;
        protected MaterialProperty AngelRingColor;

        protected MaterialProperty EmissiveTex;
        protected MaterialProperty EmissiveColor;
        protected MaterialProperty RotateEmissiveUV = null;

        protected MaterialProperty OutlineSampler;
        protected MaterialProperty OffsetZ = null;
        protected MaterialProperty outlineTex;
        protected MaterialProperty bakedNormal;

        //------------------------------------------------------
        protected MaterialEditor MMaterialEditor;


        private bool IsShadingGrademap
        {
            get
            {
                Material material = MMaterialEditor.target as Material;
                return MaterialGetInt(material, ShaderPropUtsTechniqe) == (int)UtsMode.ShadingGradeMap;
            }
        }


        private void FindProperties(MaterialProperty[] props)
        {
            // false is added if theare are possiblities the properties are not aveialable
            utsTechnique = FindProperty(ShaderPropUtsTechniqe, props);
            specularMode = FindProperty(ShaderPropIsSpecularToHighColor, props);
            specularBlendMode = FindProperty(ShaderPropIsBlendAddToHiColor, props);
            matcapBlendMode = FindProperty(ShaderPropIsBlendAddToMatCap, props);
            matcapCameraMode = FindProperty(ShaderPropIsOrtho, props);
            transparentMode = FindProperty(ShaderPropTransparentEnabled, props);
            clippingMask = FindProperty(ShaderPropClippingMask, props);
            clippingMode = FindProperty(ShaderPropClippingMode, props);

            stencilValue = FindProperty(ShaderPropStencilNo, props);

            stencilMode = FindProperty(ShaderPropStencilMode, props);
            mainTex = FindProperty(ToonConstants.ShaderPropMainTEX, props);
            baseColor = FindProperty(ToonConstants.ShaderPropBaseColor, props);
            firstShadeMap = FindProperty(ToonConstants.ShaderProp1StShadeMap, props);
            firstShadeColor = FindProperty(ToonConstants.ShaderProp1StShadeColor, props);
            secondShadeMap = FindProperty(ToonConstants.ShaderProp2NdShadeMap, props);
            secondShadeColor = FindProperty(ToonConstants.ShaderProp2NdShadeColor, props);
            normalMap = FindProperty(ToonConstants.ShaderPropNormalMap, props);
            bumpScale = FindProperty(ToonConstants.ShaderPropBumpScale, props);
            Set1StShadePosition = FindProperty(ShaderPropSet1StShadePosition, props, false);
            Set2NdShadePosition = FindProperty(ShaderPropSet2NdShadePosition, props, false);
            shadingGradeMap = FindProperty(ShaderPropShadingGradeMap, props, false);


            HighColorTex = FindProperty(ToonConstants.ShaderPropToon3DHighColorTEX, props);
            highColor = FindProperty(ToonConstants.ShaderPropToon3DHighColor, props);

            SetHighColorMask = FindProperty(ShaderPropSetHighColorMask, props);
            TweakHighColorMaskLevel = FindProperty("_Tweak_HighColorMaskLevel", props);


            SetRimLightMask = FindProperty(ShaderPropSetRimLightMask, props);

            MatCapSampler = FindProperty(ShaderPropMatCapSampler, props);
            matCapColor = FindProperty("_MatCapColor", props);

            normalMapForMatCap = FindProperty("_NormalMapForMatCap", props);
            bumpScaleMatcap = FindProperty("_BumpScaleMatcap", props);


            SetMatcapMask = FindProperty(ShaderPropSetMatcapMask, props);

            AngelRingSampler = FindProperty("_AngelRing_Sampler", props, false);
            AngelRingColor = FindProperty("_AngelRing_Color", props, false);

            EmissiveTex = FindProperty("_Emissive_Tex", props);
            EmissiveColor = FindProperty("_Emissive_Color", props);


            OutlineSampler = FindProperty(ToonConstants.ShaderPropToon3DOutlineSampler, props, false);
            outlineTex = FindProperty(ToonConstants.ShaderPropOutlineTEX, props, false);
            bakedNormal = FindProperty(ToonConstants.ShaderPropToon3DBakedNormal, props, false);
        }
//----------------------------------------------------------------------------------------------------------------------

        private static void Line()
        {
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        }

        protected static bool Foldout(bool display, GUIContent title)
        {
            return EditorGUILayout.Foldout(display, title, true);
        }

        private static bool FoldoutSubMenu(bool display, GUIContent title)
        {
            return EditorGUILayout.Foldout(display, title, true);
        }


        private class RangeProperty
        {
            internal GUIContent MGuiContent;
            internal readonly string MPropertyName;
            internal float MMin;
            internal float MMax;
            internal float MDefaultValue;

            internal RangeProperty(string label, string tooltip, string propName, float defaultValue, float min, float max)
            {
                MGuiContent = new GUIContent(label,
                    tooltip + " The range is from " + min + " to " + max + ". " + "The default value is " + defaultValue + ".");
                MPropertyName = propName;
                MMin = min;
                MMax = max;
                MDefaultValue = defaultValue;
            }
        };

        private class FloatProperty
        {
            internal GUIContent MGuiContent;
            internal readonly string MPropertyName;
            internal float MDefaultValue;

            internal FloatProperty(string label, string tooltip, string propName, float defaultValue)
            {
                MGuiContent = new GUIContent(label, tooltip + " The default value is " + defaultValue + ".");
                MPropertyName = propName;
                MDefaultValue = defaultValue;
            }
        }

        private class ColorProperty
        {
            internal GUIContent MGuiContent;
            internal readonly string MPropertyName;
            internal bool MIsHDR;

            internal ColorProperty(string label, string tooltip, string propName, bool isHDR)
            {
                MGuiContent = new GUIContent(label, tooltip);
                MPropertyName = propName;
                MIsHDR = isHDR;
            }
        }


//----------------------------------------------------------------------------------------------------------------------
        //Specify only those that use the m_MaterialEditor method as their UI. For specifying textures and colors on a single line.
        private static class Styles
        {
            public static readonly GUIContent shaderFoldout = EditorGUIUtility.TrTextContent("Shader Settings",
                "Shader Settings provides basic settings that are not specific to  cel-shading but are needed for general CG.");

            public static readonly GUIContent basicColorFoldout = EditorGUIUtility.TrTextContent("Three Color and Control Map Settings",
                "Three Color Map and Control Map Settings provide very basic settings for cel-shading in Unity Toon Shader.");

            public static readonly GUIContent shadingStepAndFeatherFoldout =
                EditorGUIUtility.TrTextContent("Shading Step and Feather Settings", "Basic 3 color step and feather settings.");

            public static readonly GUIContent normalMapFoldout = EditorGUIUtility.TrTextContent("Normal Map Settings",
                "Normal Map settings. Normal Map itself and its effectiveness to some areas.");

            public static readonly GUIContent highlightFoldout =
                EditorGUIUtility.TrTextContent("Highlight Settings", "Highlight  settings. Such as power, show or hide, light shape and so on.");

            public static readonly GUIContent rimLightFoldout = EditorGUIUtility.TrTextContent("Rim Light Settings",
                "Rim Light Settings. Such as color, direction, inverted rim light and so on.");

            public static readonly GUIContent matCapFoldout = EditorGUIUtility.TrTextContent("Material Capture (MatCap) Settings",
                "MatCap settings. Sphere maps for metallic or unusual expressions.");

            public static readonly GUIContent angelRingFoldout = EditorGUIUtility.TrTextContent("Angel Ring Projection Settings",
                "Angel ring projection settings. A kind of specular specialized for hairs.");

            public static readonly GUIContent emissionFoldout =
                EditorGUIUtility.TrTextContent("Emission Settings", "Emission settings. Textures, animations and so on.");

            public static readonly GUIContent outlineFoldout =
                EditorGUIUtility.TrTextContent("Outline Settings", "Outline settings. Such as width, colors and so on.");

            public static readonly GUIContent lightEffectivenessFoldout =
                EditorGUIUtility.TrTextContent("Scene Light Effectiveness Settings", "Scene light effectiveness to each parameter.");

            public static readonly GUIContent shadowControlMapFoldout =
                EditorGUIUtility.TrTextContent("Shadow Control Maps", "Shadow control map settings. Such as positions and highlight filtering.");

            public static readonly GUIContent pointLightFoldout =
                EditorGUIUtility.TrTextContent("Point Light Settings", "Point light settings. Such as filtering and step offset.");

            public static readonly GUIContent baseColorText = new ("Base Map", "Base Color : Texture(sRGB) × Color(RGB) Default:White");
            public static readonly GUIContent firstShadeColorText = new ("1st Shading Map", "The map used for the brighter portions of the shadow.");
            public static readonly GUIContent secondShadeColorText = new ("2nd Shading Map", "The map used for the darker portions of the shadow.");
            public static readonly GUIContent normalMapText = new ("Normal Map", "A texture that dictates the bumpiness of the material.");
            public static readonly GUIContent highColorText = new ("Highlight", "Highlight : Texture(sRGB) × Color(RGB) Default:White");

            public static readonly GUIContent highColorMaskText = new ("Highlight Mask", "A grayscale texture which utilises its brightness to control intensity.");

            public static readonly GUIContent rimLightMaskText = new ("Rim Light Mask",
                "Rim Light Mask : Texture(linear). The white part of the texture is displayed as Rim Light, and the black part is masked and not displayed.");

            public static readonly GUIContent matCapSamplerText = new ("MatCap Map", "MatCap Color : Texture(sRGB) × Color(RGB) Default:White");

            public static readonly GUIContent matCapMaskText = new ("MatCap Mask",
                "The MatCap mask is positioned with respect to the UV coordinates of the mesh onto which the MatCap is projected, and the pixels on black areas are hidden.");

            public static readonly GUIContent angelRingText = new ("Angel Ring", "Angel Ring : Texture(sRGB) × Color(RGB) Default:Black.");

            public static readonly GUIContent emissiveTexText = new ("Emission Map", "Primarily used with the Bloom Post Effect, Luminous objects can be represented.");

            public static readonly GUIContent shadingGradeMapText = new ("Shading Grade Map", "Specify shadow-prone areas in UV coordinates. Shading Grade Map : Texture(linear)");

            public static readonly GUIContent firstPositionMapText = new ("1st Shading Position Map",
                "Specify the position of fixed shadows that falls in 1st shade color areas in UV coordinates. 1st Position Map : Texture(linear)");

            public static readonly GUIContent secondPositionMapText = new ("2nd Shading Position Map",
                "Specify the position of fixed shadows that falls in 2nd shade color areas in UV coordinates. 2nd Position Map : Texture(linear)");

            public static readonly GUIContent outlineSamplerText = new ("Outline Width Map",
                "Outline Width Map as Grayscale Texture : Texture(linear). In the case of white, the Outline Width is displayed as it is. In black, the width is 0.");

            public static readonly GUIContent outlineTexText = new ("Outline Color Map", "Outline texture : Texture(sRGB) Default:White");

            public static readonly GUIContent bakedNormalOutlineText = new ("Baked Normal Map for Outline",
                "Unpacked Normal Map : Texture(linear) .Note that this is not a standard NORMAL MAP.");

            public static readonly GUIContent clippingMaskText = new ("Clipping Mask", "A grayscale texture which utilises its brightness to control transparency.");

            public static readonly GUIContent specularModeText = new ("Specular Mode", "Specular light mode. Hard or Soft.");
            public static readonly GUIContent specularBlendModeText = new ("Color Blending Mode", "Specular color blending mode. Multiply or Add.");
            public static readonly GUIContent matcapBlendModeText = new ("Color Blending Mode", "MatCap color blending mode. Multiply or Add.");

            public static readonly GUIContent matcapOrthoText = new ("MatCap Camera Mode", "Control how the MatCap Map is rendered based on the type of camera.");

            public static readonly GUIContent transparentModeText = new ("Transparency", "Enable different modes that allow the simulation of a variety of transparent objects. ");

            public static readonly GUIContent stencilVauleText = new ("Stencil Value",
                "Stencil value that is submitted to the stencil buffer for controlling the per-pixel drawing.");

            public static readonly GUIContent workflowModeText = new ("Mode", "The UTS mode that controls what settings are exposed in the shader.");

//----------------------------------------------------------------------------------------------------------------------
            public static readonly GUIContent clippingmodeModeText0 = new ("Clipping", "Allow for the use of a texture to control the transparency of the shader.");

            public static readonly GUIContent clippingmodeModeText1 = new ("Trans Clipping", "Select trans clipping mode that fits your purpose. ");
            public static readonly GUIContent stencilmodeModeText = new ("Stencil", "Control the stencil buffer which manipulates pixel drawing.");

            public static readonly GUIContent cullingModeText = new ("Culling Mode", "Controls the sides of polygons that should not be drawn (culled).");

// ----------------------------------------------------- for GUI Toggles
            public static readonly GUIContent autoRenderQueueText = new ("Auto Render Queue", "When enabled, rendering order is determined by system automatically.");

            public static readonly GUIContent renderQueueText = new ("Render Queue", "Rendering order in the scene.");
            public static readonly GUIContent invertClippingMaskText = new ("Invert Clipping Mask", "Invert clipping mask results.");

            public static readonly GUIContent baseMapAlphaAsClippingMask = new ("Use Base Map Alpha as Clipping Mask",
                "Use Base Map Alpha as Clipping Mask instead of Clipping mask texture.");

            public static readonly GUIContent applyTo1StShademapText = new ("Apply to 1st shading map", "Apply Base map to the 1st shading map.");

            public static readonly GUIContent applyTo2NdShademapText = new ("Apply to 2nd shading map", "Apply Base map or the 1st shading map to the 2st shading map.");

            public static readonly GUIContent threeBasicColorToNormalmapText = new ("Three Basic Colors",
                "Normal map effectiveness to Three Basic color areas, lit, the 1st shading and the 2nd.");

            public static readonly GUIContent highLightToNormalmapText = new ("Highlight", "Normal map effectiveness to high lit areas.");
            public static readonly GUIContent rimlightToNormalmapText = new ("Rim Light", "Normal map effectiveness to rim lit areas.");
            public static readonly GUIContent receiveShadowText = new ("Receive Shadows", "Determine if the material reflects shadows.");
            public static readonly GUIContent filterPointLightText = new ("Filter Point Light Highlights", "Show or hide highlight of point lights.");

            public static readonly GUIContent highlightOnShadowText = new ("Highlight Blending on Shadows", "Control the blending for the highlights in shadows.");

            public static readonly GUIContent lightColorEffectiveness =
                EditorGUIUtility.TrTextContent("Light Color Effectiveness", "light color effectiveness to each parameter.");

            public static readonly GUIContent lightColorEffectivinessToBaseColorText = new ("Base Color", "Light color effect the base color areas.");

            public static readonly GUIContent lightColorEffectivinessTo1StShadingText = new ("1st Shading Color", "Light color effect in the 1st shading color areas.");

            public static readonly GUIContent lightColorEffectivinessTo2NdShadingText = new ("2nd Shading Color", "Light color effect in the 2nd shading color areas.");

            public static readonly GUIContent lightColorEffectivinessToHighlitText = new ("Highlight", "Light color effect in high lit areas.");
            public static readonly GUIContent lightColorEffectivinessToRimlitText = new ("Rim Light", "Light color effect in rim lit areas.");

            public static readonly GUIContent lightColorEffectivinessToInvRimlitText = new ("Inverted Light Direciton Rim Light", "Light color effect in inverted direction rim lit areas.");

            public static readonly GUIContent lightColorEffectivinessToMatCapText = new ("MatCap", "Light color effect in MatCap areas.");

            public static readonly GUIContent lightColorEffectivenessToAngelRingText = new ("Angel Ring",
                "Light color effect in angel ring area. Angel Ring is only available in With Additional Control Maps mode.");

            public static readonly GUIContent lightColorEffectivinessToOutlineText = new ("Outline", "Light color effect in outlines.");

            public static readonly GUIContent rimlightText = new ("Rim Light",
                "A light that hits the 3D model from behind and emphasizes the contours of the model from the front.");

            public static readonly GUIContent rimlightFeatherText = new ("Rim Light Feather Off", "Disable Rim light feather.");

            public static readonly GUIContent rimlightDirectionMaskText = new ("Light Direction", "When Enabled, rim light is generated only in the direction of the light source.");

            public static readonly GUIContent invertedRimlightText = new ("Inverted Direciton Rim Light", "Rim light from inverted/antipodean direction.");

            public static readonly GUIContent camearRollingStabilizerText = new ("Stabilize Camera rolling", "Stabilize Camera rolling when capturing materials with camera.");

            public static readonly GUIContent invertedRimlightFeatherText = new ("Inverted Rim Light Feather Off", "Disable Inverted Rim light feather.");

            public static readonly GUIContent matCapText = new ("MatCap", "Enable/Disable MatCap (Material Capture)");

            public static readonly GUIContent matCapNormalmapSpecularaMask = new ("Normal Map Specular Mask for MatCap",
                "If Enabled, gives a normal map specifically for MatCap.If you are using MatCap as speculum lighting, you can use this to mask it.");

            public static readonly GUIContent matCapOnShadow = new ("MatCap Blending on Shadows", "Enables the blending rate of the MatCap range in shadows.");

            public static readonly GUIContent invertMatCapMaskText = new ("Invert MatCap Mask", "When enabled, MatCap Mask Texture is inverted.");

            public static readonly GUIContent angelRingProjectionText = new ("Angel Ring Projection",
                "Enable the Angel Ring effect for UTS, which is used to express shine or luster in hair.");

            public static readonly GUIContent angelRingAlphaAdClippingMaskText = new ("Alpha Channel as Clipping Mask",
                "Texture alpha channel is used for clipping mask. If disabled, alpha does not affect at all.");

            public static readonly GUIContent pingpongMoveText = new ("Ping-pong moves for base",
                "When enabled, you can set PingPong (back and forth) in the direction of the animation.");

            public static readonly GUIContent colorShitWithTimeText = new ("Color Shifting with Time",
                "The color that is multiplied by the Emissive texture is changed by linear interpolation (Lerp) toward the Destination Color.");

            public static readonly GUIContent colorShiftWithViewAngle = new ("Color Shifting with View Angle", "Emissive color shifts in accordance with view angle.");

            public static readonly GUIContent baseColorToOtulineText = new ("Blend Base Color to Outline", "Base Color is blended into outline color.");

            public static readonly GUIContent outlineColorMapText = new ("Outline Color Map", "Apply a texture as outline color map.");

            public static readonly GUIContent bakedNormalForOutlineText = new ("Baked Normal Map",
                "Normal maps with vertex normals previously baked in from other models can be loaded as an addition when setting up normal inversion outlines.");

            public static readonly GUIContent emissiveScrollAnimationModeText = new ("Animation Mode", "Controls the animated scrolling of the emissive texture.");

            public static readonly GUIContent emissionAnimationText = new ("Emission Map Animation", "When Enabled, the UV and Color of the Emission Map are animated.");

            public static readonly GUIContent outlineModeText = new ("Outline Mode", "Specifies how the inverted-outline object is spawned.");

            public static readonly GUIContent limitLightIntensityText = new ("Limit Light Intensity", "Limit the brightness of the light to 1 to avoid white-out.");

            // Range properties
            public static readonly RangeProperty tweakTransParencyText = new (
                label: "Transparency Level", tooltip: "Adjusts the transparency by considering the grayscale level of the clipping mask as an alpha value.",
                propName: "_Tweak_transparency", defaultValue: 0.0f, min: -1.0f, max: 1.0f);

            public static readonly RangeProperty clippingLevelText = new (
                label: "Clipping Level", tooltip: "Specifies the strength of the clipping mask.",
                propName: "_Clipping_Level", defaultValue: 0.0f, min: 0.0f, max: 1.0f);

            public static readonly RangeProperty scrollEmissiveUText = new (
                label: "Scroll U/X direction",
                tooltip:
                "Specifies how much the Emissive texture should scroll in the u-direction (x-axis direction) when updating the animation. The scrolling animation is ultimately determined by Base Speed (Time) x Scroll U Direction x Scroll V Direction.",
                propName: "_Scroll_EmissiveU", defaultValue: 0.0f, min: -1.0f, max: 1.0f);

            public static readonly RangeProperty scrollEmissiveVText = new (
                label: "Scroll V/Y direction",
                tooltip:
                "Specifies how much the Emissive texture should scroll in the u-direction (y-axis direction) when updating the animation. The scrolling animation is ultimately determined by Base Speed (Time) x Scroll U Direction x Scroll V Direction.",
                propName: "_Scroll_EmissiveV", defaultValue: 0.0f, min: -1.0f, max: 1.0f);

            public static readonly RangeProperty tweakHighColorOnShadowText = new (
                label: "Blending Level", tooltip: "Adjusts the intensity of highlight applied to shadow areas.",
                propName: "_TweakHighColorOnShadow", defaultValue: 0.0f, min: 0, max: 1);

            public static readonly RangeProperty tweakMatCapOnShadowText = new (
                label: "Blending Level", tooltip: "Adjusts the intensity of MatCap applied to shadow areas.",
                propName: "_TweakMatCapOnShadow", defaultValue: 0.0f, min: 0, max: 1);

            public static readonly RangeProperty tweakSystemShadowLevelText = new (
                label: "System Shadow Level", tooltip: "Define the appearance of self-shadows and other received shadows that blend with the toon shader.",
                propName: "_Tweak_SystemShadowsLevel", defaultValue: 0.0f, min: -0.5f, max: 0.5f);

            public static readonly RangeProperty shaderPropBaseColorText = new (
                label: "Base Color Step", tooltip: "Sets the boundary between the Base Color and the Shade Colors.",
                propName: ToonConstants.ShaderPropToon3DBaseColorStep, defaultValue: 0.5f, min: 0, max: 1);

            public static readonly RangeProperty shaderPropBaseFeatherText = new (
                label: "Base Shading Feather", tooltip: "Feathers the boundary between the Base Color and the Shade Colors..",
                propName: ToonConstants.ShaderPropToon3DBaseShadeFeather, defaultValue: 0.0001f, min: 0.0001f, max: 1);

            public static readonly RangeProperty shaderPropShadeColorStepText = new (
                label: "Shading Color Step", tooltip: "Sets the boundary between the 1st and 2nd Shade Colors. Set this to 0 if no 2nd Shade Color is used.",
                propName: ShaderPropShadeColorStep, defaultValue: 0, min: 0, max: 1);

            public static readonly RangeProperty shaderProp1St2NdShadesFeatherText = new (
                label: "1st/2nd Shading Feather", tooltip: "Feathers the boundary between the 1st and 2nd Shade Colors.",
                propName: ShaderProp1St2NdShadesFeather, defaultValue: 0.0001f, min: 0.0001f, max: 1);

            public static readonly RangeProperty shaderProp1StShadeColorStepText = new (
                label: "1st Shade Color Step", tooltip: "Sets the step between the Base color and 1st Shade Color, the same as the BaseColor_Step property..",
                propName: ToonConstants.ShaderPropToon3D1StShadeColorStep, defaultValue: 0.5f, min: 0, max: 1);

            public static readonly RangeProperty shaderProp1StShadeColorFeatherText = new (
                label: "1st Shade Color Feather",
                tooltip: "Feathers the boundary between the Base Color and the 1st Shade Color, the same as the Base/Shade_Feather property.",
                propName: ToonConstants.ShaderPropToon3D1StShadeColorFeather, defaultValue: 0.0001f, min: 0.0001f, max: 1);

            public static readonly RangeProperty shaderProp2NdShadeColorStepText = new (
                label: "2nd Shade Color Step", tooltip: "Sets the step between the 1st and 2nd Shade Colors, the same as the ShadeColor_Step property.",
                propName: ShaderProp2NdShadeColorStep, defaultValue: 0, min: 0, max: 1);

            public static readonly RangeProperty shaderProp2NdShadeColorFeatherText = new (
                label: "2nd Shade Color Feather",
                tooltip: "Feathers the boundary between the 1st and 2nd Shade Colors, the same as the 1st/2nd_Shades_Feather properties.",
                propName: ShaderProp2NdShadeColorFeather, defaultValue: 0.0001f, min: 0.0001f, max: 1);

            public static readonly RangeProperty shaderPropStepOffsetText = new (
                label: "Step Offset", tooltip: "Fine tunes light steps (boundaries) added in the ForwardAdd pass, such as real-time point lights.",
                propName: "_StepOffset", defaultValue: 0, min: -0.5f, max: 0.5f);

            public static readonly RangeProperty shaderPropHilightPowerText = new (
                label: "Highlight Power", tooltip: "Highlight power factor, pow(x,5) is used inside the shader.",
                propName: "_HighColor_Power", defaultValue: 0, min: 0, max: 1);

            public static readonly RangeProperty hilightMaskLevelText = new (
                label: "Highlight Mask Level", tooltip: "Highlight mask texture blending level to highlights.",
                propName: "_Tweak_HighColorMaskLevel", defaultValue: 0, min: -1, max: 1);

            public static readonly RangeProperty shadingGradeMapLevelText = new (
                label: "ShadingGradeMap Level", tooltip: "Level-corrects the grayscale values in the Shading Grade Map.",
                propName: "_Tweak_ShadingGradeMapLevel", defaultValue: 0, min: -0.5f, max: 0.5f);

            public static readonly RangeProperty blureLevelSgmText = new (
                label: "ShadingGradeMap Blur Level",
                tooltip:
                "The Mip Map feature is used to blur the Shading Grade Map; to enable Mip Map, turn on Advanced > Generate Mip Maps in the Texture Import Settings. The default is 0 (no blur).",
                propName: "_BlurLevelSGM", defaultValue: 0, min: 0, max: 10);

            public static readonly RangeProperty rimLightMaskLevelText = new (
                label: "Rim Light Mask Level",
                tooltip:
                "-1 gives 0% for the Rim Light effect, 0 gives 100% for the Rim Light and Mask effect, 1 gives 100% for the Rim Light and 0% for the Mask effect.",
                propName: "_Tweak_RimLightMaskLevel", defaultValue: 0, min: -1, max: 1);

            public static readonly RangeProperty lightDirectionMaskLevelText = new (
                label: "Light Direction Rim Light Level", tooltip: "Specifies intensity of Rim Light in the light source direction,",
                propName: "_Tweak_LightDirection_MaskLevel", defaultValue: 0, min: 0f, max: 0.5f);

            public static readonly RangeProperty tweakMatCapUVText = new (
                label: "Scale MatCap UV", tooltip: "Scaling UV of MatCap Map.",
                propName: "_Tweak_MatCapUV", defaultValue: 0, min: -0.5f, max: 0.5f);

            public static readonly RangeProperty rotateMatCapUVText = new (
                label: "Rotate MatCap UV", tooltip: "Rotating UV of MatCap Map.",
                propName: "_Rotate_MatCapUV", defaultValue: 0, min: -1, max: 1);

            public static readonly RangeProperty matcapBlurLevelText = new (
                label: "MatCap Blur Level",
                tooltip:
                "Blur MatCap Map using the Mip Map feature; to enable Mip Map, turn on Advanced > Generate Mip Maps in the Texture Import Settings. Default is 0 (no blur).",
                propName: "_BlurLevelMatcap", defaultValue: 0, min: 0, max: 10);

            public static readonly RangeProperty arOffsetUText = new (
                label: "Offset U", tooltip: "Adjusts the Angel Ring's shape in the horizontal direction.",
                propName: "_AR_OffsetU", defaultValue: 0, min: 0, max: 0.5f);

            public static readonly RangeProperty arOffsetVText = new (
                label: "Offset V", tooltip: "Adjusts the Angel Ring's shape in the vertical direction.",
                propName: "_AR_OffsetV", defaultValue: 0, min: 0, max: 1);

            public static readonly RangeProperty rimLightPowerText = new (
                label: "Rim Light Level", tooltip: "Specifies Rim Light Intensity.",
                propName: "_RimLight_Power", defaultValue: 0.1f, min: 0, max: 1);

            public static readonly RangeProperty invertedRimLightPowerText = new (
                label: "Inverted Rim Light Level", tooltip: "Specifies Inverted/Antipodean Rim Light Level.",
                propName: "_Ap_RimLight_Power", defaultValue: 0.1f, min: 0, max: 1);

            public static readonly RangeProperty giIntensityText = new (
                label: "Light Probe Intensity", tooltip: "The light probe color is added to the material color according to the GI Intensity value.",
                propName: ShaderPropGIIntensity, defaultValue: 0.0f, min: 0, max: 1);

            public static readonly RangeProperty tweakMatCapMaskLevelText = new (
                label: "MatCap Mask Level",
                "Adjusts the level of the MatCap Mask. When the value is 1, MatCap is displayed 100% irrespective of whether or not there is a mask. When the value is -1, MatCap will not be displayed at all and MatCap will be the same as in the off state.",
                propName: "_Tweak_MatcapMaskLevel", defaultValue: 0.0f, min: -1, max: 1);

            public static readonly RangeProperty rotateNormalMapForMatCapUVText = new (
                label: "Rotate Normal Map UV", "Rotates the MatCap normal map UV based on its center.",
                propName: "_Rotate_NormalMapForMatCapUV", defaultValue: 0.0f, min: -1, max: 1);

            public static readonly RangeProperty rimLightInsideMaskText = new (
                label: "Adjust Rim Light Area", "Increasing this value narrows the area of influence of Rim Light.",
                propName: "_RimLight_InsideMask", defaultValue: 0.0001f, min: 0.0001f, max: 1);

            // Float properties
            public static readonly FloatProperty baseSpeedText = new (label: "Base Speed (Time)",
                tooltip:
                "Specifies the base update speed of scroll animation. If the value is 1, it will be updated in 1 second. Specifying a value of 2 results in twice the speed of a value of 1, so it will be updated in 0.5 seconds.",
                propName: "_Base_Speed", defaultValue: 0);

            public static readonly FloatProperty outlineWidthText = new (label: "Outline Width",
                tooltip: "Specifies the width of the outline. This value relies on the scale when the model was imported to Unity.",
                propName: ToonConstants.ShaderPropToon3DOutlineWidth, defaultValue: 0);

            public static readonly FloatProperty farthestDistanceText = new (label: "Farthest Distance to vanish",
                tooltip:
                "Specify the furthest distance, where the outline width changes with the distance between the camera and the object. The outline will be zero at this distance.",
                propName: ToonConstants.ShaderPropToon3DFarthestDistance, defaultValue: 100);

            public static readonly FloatProperty nearestDistanceText = new (label: "Nearest Distance to draw with Outline Width",
                tooltip:
                "Specify the closest distance, where the outline width changes with the distance between the camera and the object. At this distance, the outline will be the maximum width set by Outline_Width.",
                propName: ToonConstants.ShaderPropToon3DNearestDistance, defaultValue: 0.5f);

            public static readonly FloatProperty rotateEmissiveUVText = new (label: "Rotate around UV center",
                tooltip:
                "When Base Speed=1, the Emissive texture will rotate clockwise by 1. When combined with scrolling, rotation will occur after scrolling.",
                propName: "_Rotate_EmissiveUV", defaultValue: 0);

            public static readonly FloatProperty offsetZText = new (label: "Offset Outline with Camera Z-axis",
                tooltip: "Offsets the outline in the depth (Z) direction of the camera.",
                propName: ToonConstants.ShaderPropToon3DOffsetZ, defaultValue: 0);

            public static readonly FloatProperty colorShiftSpeedText = new (label: "Color Shifting Speed (Time)",
                tooltip: "Sets the reference speed for color shift. When the value is 1, one cycle should take around 6 seconds.",
                propName: "_ColorShift_Speed", defaultValue: 0);

            // Color prperties
            public static readonly ColorProperty viewShiftText = new (label: "Shifting Target Color",
                tooltip: "Target color above, must be specified in HDR.",
                propName: "_ViewShift", isHDR: true);

            public static readonly ColorProperty colorShiftText = new (label: "Destination Color",
                tooltip: "Destination color above, must be specified in HDR.",
                propName: "_ColorShift", isHDR: true);

            public static readonly ColorProperty rimLightColorText = new (label: "Rim Light Color",
                tooltip: "Specifies the color of rim light.",
                propName: "_RimLightColor", isHDR: false);

            public static readonly ColorProperty apRimLightColorText = new (label: "Inverted Rim Light Color",
                tooltip: "Specifies the color of inverted/antipodean rim light.",
                propName: "_Ap_RimLightColor", isHDR: false);

            public static readonly ColorProperty outlineColorText = new (label: "Outline Color",
                tooltip: "Specifies the color of outline.",
                propName: ToonConstants.ShaderPropToon3DOutlineColor, isHDR: false);
        }
//----------------------------------------------------------------------------------------------------------------------

        private void OnOpenGUI(Material material, MaterialEditor materialEditor, MaterialProperty[] props)
        {
            _mMaterialScopeList.RegisterHeaderScope(Styles.shaderFoldout, Expandable.Shader, DrawShaderOptions, (uint)UtsMode.ThreeColorToon,
                (uint)UtsTransparentMode.Off, isTessellation: 0);
            _mMaterialScopeList.RegisterHeaderScope(Styles.basicColorFoldout, Expandable.Basic, GUI_BasicThreeColors, (uint)UtsMode.ThreeColorToon,
                (uint)UtsTransparentMode.Off, isTessellation: 0);
            _mMaterialScopeList.RegisterHeaderScope(Styles.shadingStepAndFeatherFoldout, Expandable.ShadingStepAndFeather, GUI_StepAndFeather,
                (uint)UtsMode.ThreeColorToon, (uint)UtsTransparentMode.Off, isTessellation: 0);
            _mMaterialScopeList.RegisterHeaderScope(Styles.normalMapFoldout, Expandable.NormalMap, GUI_NormalmapSettings, (uint)UtsMode.ThreeColorToon,
                (uint)UtsTransparentMode.Off, isTessellation: 0);
            _mMaterialScopeList.RegisterHeaderScope(Styles.highlightFoldout, Expandable.Highlight, GUI_HighlightSettings, (uint)UtsMode.ThreeColorToon,
                (uint)UtsTransparentMode.Off, isTessellation: 0);
            _mMaterialScopeList.RegisterHeaderScope(Styles.rimLightFoldout, Expandable.Rimlight, GUI_RimLight, (uint)UtsMode.ThreeColorToon,
                (uint)UtsTransparentMode.Off, isTessellation: 0);
            _mMaterialScopeList.RegisterHeaderScope(Styles.matCapFoldout, Expandable.MatCap, GUI_MatCap, (uint)UtsMode.ThreeColorToon,
                (uint)UtsTransparentMode.Off, isTessellation: 0);
            _mMaterialScopeList.RegisterHeaderScope(Styles.angelRingFoldout, Expandable.AngelRing, GUI_AngelRing, (uint)UtsMode.ShadingGradeMap,
                (uint)UtsTransparentMode.Off, isTessellation: 0);
            _mMaterialScopeList.RegisterHeaderScope(Styles.emissionFoldout, Expandable.Emission, GUI_Emissive, (uint)UtsMode.ThreeColorToon,
                (uint)UtsTransparentMode.Off, isTessellation: 0);
            _mMaterialScopeList.RegisterHeaderScope(Styles.outlineFoldout, Expandable.Outline, GUI_Outline, (uint)UtsMode.ThreeColorToon,
                (uint)UtsTransparentMode.On, isTessellation: 0);

            // originally these were in simple UI
            _mMaterialScopeList.RegisterHeaderScope(Styles.lightEffectivenessFoldout, Expandable.SceneLight, GUI_LightColorEffectiveness,
                (uint)UtsMode.ThreeColorToon, (uint)UtsTransparentMode.Off, isTessellation: 0);
        }

        private void Uts3DrawHeaders(MaterialEditor materialEditor, Material material)
        {
            if (material == null)
                throw new ArgumentNullException(nameof(material));

            if (materialEditor == null)
                throw new ArgumentNullException(nameof(materialEditor));

            foreach (var item in _mMaterialScopeList.m_Items)
            {
                if (!IsShadingGrademap && item.workflowMode == (uint)UtsMode.ShadingGradeMap)
                {
                    continue;
                }

                if (transparencyEnabled == UtsTransparentMode.On && item.transparentEnabled == (uint)UtsTransparentMode.On)
                {
                    continue;
                }

                using (var header = new UTS3MaterialHeaderScope(
                           item.headerTitle,
                           item.expandable,
                           materialEditor,
                           defaultExpandedState: _mMaterialScopeList.m_DefaultExpandedState,
                           documentationURL: item.url))
                    // never called from the system??
                {
                    if (!header.expanded)
                        continue;

                    item.drawMaterialScope(material);

                    EditorGUILayout.Space();
                }
            }
        }

        private void ShaderPropertiesGUI(MaterialEditor materialEditor, Material material, MaterialProperty[] properties)
        {
            Uts3DrawHeaders(materialEditor, material);
        }

        private void DrawShaderOptions(Material material)
        {
            GUI_SetCullingMode(material);
            GUI_SetRenderQueue(material);
            GUI_Tranparent(material);
            GUI_StencilMode(material);

            switch (_mWorkflowMode)
            {
                case UtsMode.ThreeColorToon:

                    DoPopup(Styles.clippingmodeModeText0, clippingMode, ClippingModeNames);
                    UtsClippingMode mode0 = (UtsClippingMode)MaterialGetInt(material, ShaderPropClippingMode);
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginDisabledGroup(mode0 == UtsClippingMode.Off);
                {
                    GUI_SetClippingMask(material);
                }
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.BeginDisabledGroup(mode0 != UtsClippingMode.TransClippingMode);
                {
                    GUI_SetTransparencySetting(material);
                }
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.indentLevel--;
                    break;
                case UtsMode.ShadingGradeMap:

                    DoPopup(Styles.clippingmodeModeText1, clippingMode, Enum.GetNames(typeof(UtsTransClippingMode)));
                    UtsTransClippingMode mode1 = (UtsTransClippingMode)MaterialGetInt(material, ShaderPropClippingMode);
                    EditorGUI.indentLevel++;
                    EditorGUI.BeginDisabledGroup(mode1 != UtsTransClippingMode.On);
                {
                    GUI_SetClippingMask(material);
                    GUI_SetTransparencySetting(material);
                }
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.indentLevel--;
                    break;
            }

            EditorGUILayout.Space();
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            MMaterialEditor = materialEditor;
            Material material = materialEditor.target as Material;
            EditorGUIUtility.fieldWidth = 0;
            if (MFirstTimeApply)
            {
                OnOpenGUI(material, materialEditor, props);
                MFirstTimeApply = false;
            }

            FindProperties(props);

            MAutoRenderQueue = MaterialGetInt(material, ShaderPropAutoRenderQueue);
            transparencyEnabled = (UtsTransparentMode)MaterialGetInt(material, ShaderPropTransparentEnabled);
            stencilNumberSetting = MaterialGetInt(material, ShaderPropStencilNo);

            EditorGUI.BeginChangeCheck();

            // select UTS technique here.
            DoPopup(Styles.workflowModeText, utsTechnique, UtsModeNames);
            _mWorkflowMode = (UtsMode)MaterialGetInt(material, ShaderPropUtsTechniqe);
            switch (_mWorkflowMode)
            {
                case UtsMode.ThreeColorToon:
                    material.DisableKeyword(ShaderDefineShadinggrademap);
                    break;
                case UtsMode.ShadingGradeMap:
                    material.EnableKeyword(ShaderDefineShadinggrademap);
                    break;
            }

            SetupTransparentModeForOutline(material, transparencyEnabled == UtsTransparentMode.On);

            ShaderPropertiesGUI(materialEditor, material, props);

            ApplyClippingMode(material);
            ApplyStencilMode(material);
            ApplyAngelRing(material);
            ApplyMatCapMode(material);
            ApplyQueueAndRenderType(_mWorkflowMode, material);

            if (EditorGUI.EndChangeCheck())
            {
                MMaterialEditor.PropertiesChanged();
            }
        } // End of OnGUI()


        internal static int MaterialGetInt(Material material, string prop)
        {
            return (int)material.GetFloat(prop);
        }

        internal static void MaterialSetInt(Material material, string prop, int value)
        {
            material.SetFloat(prop, value);
        }

        private float GUI_FloatProperty(Material material, FloatProperty floatProp)
        {
            float ret = material.GetFloat(floatProp.MPropertyName);
            EditorGUI.BeginChangeCheck();
            ret = EditorGUILayout.FloatField(floatProp.MGuiContent, ret);

            if (EditorGUI.EndChangeCheck())
            {
                MMaterialEditor.RegisterPropertyChangeUndo(floatProp.MGuiContent.text);
                material.SetFloat(floatProp.MPropertyName, ret);
            }

            return ret;
        }

        private Color GUI_ColorProperty(Material material, ColorProperty colorProp)
        {
            Color ret = material.GetColor(colorProp.MPropertyName);
            EditorGUI.BeginChangeCheck();

            ret = EditorGUILayout.ColorField(colorProp.MGuiContent, ret, showEyedropper: true, showAlpha: true, hdr: colorProp.MIsHDR);

            if (EditorGUI.EndChangeCheck())
            {
                MMaterialEditor.RegisterPropertyChangeUndo(colorProp.MGuiContent.text);
                material.SetColor(colorProp.MPropertyName, ret);
            }

            return ret;
        }

        private float GUI_RangeProperty(Material material, RangeProperty rangeProp)
        {
            return GUI_RangeProperty(material, rangeProp.MGuiContent, rangeProp.MPropertyName, rangeProp.MMin, rangeProp.MMax);
        }

        private float GUI_RangeProperty(Material material, GUIContent guiContent, string propName, float min, float max)
        {
            float ret = material.GetFloat(propName);
            EditorGUI.BeginChangeCheck();
            ret = EditorGUILayout.Slider(guiContent, ret, min, max);

            if (EditorGUI.EndChangeCheck())
            {
                MMaterialEditor.RegisterPropertyChangeUndo(guiContent.text);
                material.SetFloat(propName, ret);
            }

            return ret;
        }

        private bool GUI_Toggle(Material material, GUIContent guiContent, string prop, bool value)
        {
            EditorGUI.BeginChangeCheck();
            var ret = EditorGUILayout.Toggle(guiContent, value);
            if (EditorGUI.EndChangeCheck())
            {
                MMaterialEditor.RegisterPropertyChangeUndo(guiContent.text);
                MaterialSetInt(material, prop, ret ? 1 : 0);
            }

            return ret;
        }

//- --------------------------------------------------------------------------------------------------------------------

        private void GUI_SetRenderQueue(Material material)
        {
            EditorGUILayout.BeginHorizontal();

            MAutoRenderQueue = GUI_Toggle(material, Styles.autoRenderQueueText, ShaderPropAutoRenderQueue, MAutoRenderQueue == 1) ? 1 : 0;

            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(MAutoRenderQueue == 1);
            MRenderQueue = (int)EditorGUILayout.IntField(Styles.renderQueueText, material.renderQueue);
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;
        }


        private void GUI_SetCullingMode(Material material)
        {
            const string cullMode = "_CullMode";
            int cullModeSetting = MaterialGetInt(material, cullMode);
            //Convert it to Enum format and store it in the offlineMode variable.
            if ((int)CullingMode.Off == cullModeSetting)
            {
                MCullingMode = CullingMode.Off;
            }
            else if ((int)CullingMode.Frontface == cullModeSetting)
            {
                MCullingMode = CullingMode.Frontface;
            }
            else
            {
                MCullingMode = CullingMode.Backface;
            }

            //GUI description with EnumPopup.
            MCullingMode = (CullingMode)EditorGUILayout.EnumPopup(Styles.cullingModeText, MCullingMode);
            //If the value changes, write to the material.
            if (cullModeSetting != (int)MCullingMode)
            {
                switch (MCullingMode)
                {
                    case CullingMode.Off:
                        MaterialSetInt(material, cullMode, 0);
                        break;
                    case CullingMode.Frontface:
                        MaterialSetInt(material, cullMode, 1);
                        break;
                    default:
                        MaterialSetInt(material, cullMode, 2);
                        break;
                }
            }
        }

        private void GUI_Tranparent(Material material)
        {
            const string zWriteMode = "_ZWriteMode";
            const string zOverDrawMode = "_ZOverDrawMode";
            DoPopup(Styles.transparentModeText, transparentMode, Enum.GetNames(typeof(UtsTransparentMode)));
            transparencyEnabled = (UtsTransparentMode)MaterialGetInt(material, ShaderPropTransparentEnabled);

            if (transparencyEnabled == UtsTransparentMode.On)
            {
                if (MaterialGetInt(material, ShaderPropUtsTechniqe) == (int)UtsMode.ThreeColorToon)
                {
                    MaterialSetInt(material, ShaderPropClippingMode, (int)UtsClippingMode.TransClippingMode);
                }
                else
                {
                    // ShadingGradeMap
                    MaterialSetInt(material, ShaderPropClippingMode, (int)UtsTransClippingMode.On);
                }

                MaterialSetInt(material, zWriteMode, 0);
                material.SetFloat(zOverDrawMode, 1);
            }
            else
            {
                MaterialSetInt(material, zWriteMode, 1);
                material.SetFloat(zOverDrawMode, 0);
            }
        }

        private void GUI_StencilMode(Material material)
        {
            const string kStencilValue = "Stencil Value";

            DoPopup(Styles.stencilmodeModeText, stencilMode, StencilModeNames);


            EditorGUI.indentLevel++;
            int currentStencilValue = stencilNumberSetting;
            EditorGUI.BeginDisabledGroup((UtsStencilMode)MaterialGetInt(material, ShaderPropStencilMode) == UtsStencilMode.Off);
            EditorGUI.BeginChangeCheck();
            currentStencilValue = EditorGUILayout.IntSlider(Styles.stencilVauleText, stencilNumberSetting, 0, 255);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(material, "Changed " + kStencilValue);
                MaterialSetInt(material, ShaderPropStencilNo, currentStencilValue);
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel--;
        }

        private void GUI_SetClippingMask(Material material)
        {
            MMaterialEditor.TexturePropertySingleLine(Styles.clippingMaskText, clippingMask);

            GUI_Toggle(material, Styles.invertClippingMaskText, ShaderPropInverseClipping, MaterialGetInt(material, ShaderPropInverseClipping) != 0);
            GUI_RangeProperty(material, Styles.clippingLevelText);
        }

        private void GUI_SetTransparencySetting(Material material)
        {
            GUI_RangeProperty(material, Styles.tweakTransParencyText);
            GUI_Toggle(material, Styles.baseMapAlphaAsClippingMask, ShaderPropIsBaseMapAlphaAsClippingMask,
                MaterialGetInt(material, ShaderPropIsBaseMapAlphaAsClippingMask) != 0);
        }


        private void GUI_BasicThreeColors(Material material)
        {
            MMaterialEditor.TexturePropertySingleLine(Styles.baseColorText, mainTex, baseColor);
            //v.2.0.7 Synchronize _Color to _BaseColor. [TODO-sin: 2025-11-28] Verify this
            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", material.GetColor(ToonConstants.ShaderPropBaseColor));
            }
            //

            EditorGUI.indentLevel += 2;
            var applyTo1St = GUI_Toggle(material, Styles.applyTo1StShademapText, ToonConstants.ShaderPropUseBaseAs1St,
                MaterialGetInt(material, ToonConstants.ShaderPropUseBaseAs1St) != 0);
            EditorGUI.indentLevel -= 2;


            if (applyTo1St)
            {
                EditorGUI.indentLevel += 2;
                MMaterialEditor.ColorProperty(firstShadeColor, Styles.firstShadeColorText.text);
                EditorGUI.indentLevel -= 2;
            }
            else
            {
                MMaterialEditor.TexturePropertySingleLine(Styles.firstShadeColorText, firstShadeMap, firstShadeColor);
            }
            //            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel += 2;
            bool applyTo2Nd = GUI_Toggle(material, Styles.applyTo2NdShademapText,
                ToonConstants.ShaderPropUse1StAs2Nd,
                MaterialGetInt(material, ToonConstants.ShaderPropUse1StAs2Nd) != 0);
            EditorGUI.indentLevel -= 2;


            if (applyTo2Nd)
            {
                EditorGUI.indentLevel += 2;
                MMaterialEditor.ColorProperty(secondShadeColor, Styles.secondShadeColorText.text);
                EditorGUI.indentLevel -= 2;
            }
            else
            {
                MMaterialEditor.TexturePropertySingleLine(Styles.secondShadeColorText, secondShadeMap, secondShadeColor);
            }

            EditorGUILayout.Space();
            MMaterialEditor.TextureScaleOffsetProperty(mainTex);

            ShadowControlMapsFoldout = FoldoutSubMenu(ShadowControlMapsFoldout, Styles.shadowControlMapFoldout);
            if (ShadowControlMapsFoldout)
            {
                EditorGUI.indentLevel++;
                GUI_ShadowControlMaps(material);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }

        private void GUI_ShadowControlMaps(Material material)
        {
            if (material.HasProperty(ShaderPropUtsTechniqe)) //DoubleWithFeather or ShadingGradeMap
            {
                if (MaterialGetInt(material, ShaderPropUtsTechniqe) == (int)UtsMode.ThreeColorToon) //DWF
                {
                    EditorGUILayout.LabelField("Mode: Standard", EditorStyles.boldLabel);
                    MMaterialEditor.TexturePropertySingleLine(Styles.firstPositionMapText, Set1StShadePosition);
                    MMaterialEditor.TexturePropertySingleLine(Styles.secondPositionMapText, Set2NdShadePosition);
                }
                else if (MaterialGetInt(material, ShaderPropUtsTechniqe) == (int)UtsMode.ShadingGradeMap)
                {
                    EditorGUILayout.LabelField("Mode: With Additional Control Maps", EditorStyles.boldLabel);
                    MMaterialEditor.TexturePropertySingleLine(Styles.shadingGradeMapText, shadingGradeMap);
                    GUI_RangeProperty(material, Styles.shadingGradeMapLevelText);
                    GUI_RangeProperty(material, Styles.blureLevelSgmText);
                }
            }
        }

        private void GUI_StepAndFeather(Material material)
        {
            GUI_ShadingStepAndFeatherSettings(material);

            if (!SimpleUI)
            {
                GUI_SystemShadows(material);

                AdditionalLookdevsFoldout = FoldoutSubMenu(AdditionalLookdevsFoldout, Styles.pointLightFoldout);
                if (AdditionalLookdevsFoldout)
                {
                    GUI_AdditionalLookdevs(material);
                }
            }
        }

        private void GUI_SystemShadows(Material material)
        {
            bool isEnabled = GUI_Toggle(material, Styles.receiveShadowText, ShaderPropSetSystemShadowsToBase,
                MaterialGetInt(material, ShaderPropSetSystemShadowsToBase) != 0);

            //           if (material.GetFloat(ShaderPropSetSystemShadowsToBase) == 1)
            EditorGUI.BeginDisabledGroup(!isEnabled);
            {
                EditorGUI.indentLevel++;
                GUI_RangeProperty(material, Styles.tweakSystemShadowLevelText);

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
        }

        private void GUI_ShadingStepAndFeatherSettings(Material material)
        {
            if (material.HasProperty(ShaderPropUtsTechniqe))
            {
                var mode = MaterialGetInt(material, ShaderPropUtsTechniqe);
                if (mode == (int)UtsMode.ThreeColorToon)
                {
                    EditorGUILayout.LabelField("Mode: Standard", EditorStyles.boldLabel);
                    GUI_RangeProperty(material, Styles.shaderPropBaseColorText);
                    GUI_RangeProperty(material, Styles.shaderPropBaseFeatherText);
                    GUI_RangeProperty(material, Styles.shaderPropShadeColorStepText);
                    GUI_RangeProperty(material, Styles.shaderProp1St2NdShadesFeatherText);


                    //Sharing variables with ShadingGradeMap method.

                    material.SetFloat(ToonConstants.ShaderPropToon3D1StShadeColorStep, material.GetFloat(ToonConstants.ShaderPropToon3DBaseColorStep));
                    material.SetFloat(ToonConstants.ShaderPropToon3D1StShadeColorFeather, material.GetFloat(ToonConstants.ShaderPropToon3DBaseShadeFeather));
                    material.SetFloat(ShaderProp2NdShadeColorStep, material.GetFloat(ShaderPropShadeColorStep));
                    material.SetFloat(ShaderProp2NdShadeColorFeather, material.GetFloat(ShaderProp1St2NdShadesFeather));
                }
                else if (mode == (int)UtsMode.ShadingGradeMap)
                {
                    //SGM
                    EditorGUILayout.LabelField("Mode: With Additional Control Maps", EditorStyles.boldLabel);

                    GUI_RangeProperty(material, Styles.shaderProp1StShadeColorStepText);
                    GUI_RangeProperty(material, Styles.shaderProp1StShadeColorFeatherText);
                    GUI_RangeProperty(material, Styles.shaderProp2NdShadeColorStepText);
                    GUI_RangeProperty(material, Styles.shaderProp2NdShadeColorFeatherText);

                    //Share variables with DoubleWithFeather method.
                    material.SetFloat(ToonConstants.ShaderPropToon3DBaseColorStep, material.GetFloat(ToonConstants.ShaderPropToon3D1StShadeColorStep));
                    material.SetFloat(ToonConstants.ShaderPropToon3DBaseShadeFeather, material.GetFloat(ToonConstants.ShaderPropToon3D1StShadeColorFeather));
                    material.SetFloat(ShaderPropShadeColorStep, material.GetFloat(ShaderProp2NdShadeColorStep));
                    material.SetFloat(ShaderProp1St2NdShadesFeather, material.GetFloat(ShaderProp2NdShadeColorFeather));
                }
                else
                {
                    // OutlineObj.
                    return;
                }
            }

            EditorGUILayout.Space();
        }

        private void GUI_AdditionalLookdevs(Material material)
        {
            EditorGUI.indentLevel++;
            GUI_RangeProperty(material, Styles.shaderPropStepOffsetText);
            GUI_Toggle(material, Styles.filterPointLightText, ShaderPropIsFilterHiCutPointLightColor,
                MaterialGetInt(material, ShaderPropIsFilterHiCutPointLightColor) != 0);

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        private void GUI_NormalmapSettings(Material material)
        {
            MMaterialEditor.TexturePropertySingleLine(Styles.normalMapText, normalMap, bumpScale);
            MMaterialEditor.TextureScaleOffsetProperty(normalMap);

            EditorGUILayout.LabelField("Normal Map Effectiveness", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            GUI_Toggle(material, Styles.threeBasicColorToNormalmapText, ShaderPropIsNormalMapToBase,
                MaterialGetInt(material, ShaderPropIsNormalMapToBase) != 0);
            GUI_Toggle(material, Styles.highLightToNormalmapText, ShaderPropNormalMapToHighColor,
                MaterialGetInt(material, ShaderPropNormalMapToHighColor) != 0);
            GUI_Toggle(material, Styles.rimlightToNormalmapText, ShaderPropIsNormalMapToRimLight,
                MaterialGetInt(material, ShaderPropIsNormalMapToRimLight) != 0);

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        private void GUI_HighlightSettings(Material material)
        {
            MMaterialEditor.TexturePropertySingleLine(Styles.highColorText, HighColorTex, highColor);
            GUI_RangeProperty(material, Styles.shaderPropHilightPowerText);

            if (!SimpleUI)
            {
                EditorGUI.showMixedValue = specularMode.hasMixedValue;

                int mode = (int)specularMode.floatValue;
                EditorGUI.BeginChangeCheck();
                mode = EditorGUILayout.Popup(Styles.specularModeText, mode, Enum.GetNames(typeof(UtsSpeculerMode)));
                if (EditorGUI.EndChangeCheck())
                {
                    MMaterialEditor.RegisterPropertyChangeUndo(Styles.specularModeText.text);
                    switch ((UtsSpeculerMode)mode)
                    {
                        case UtsSpeculerMode.Hard:
                            break;
                        case UtsSpeculerMode.Soft:
                            specularBlendMode.floatValue = 1.0f;
                            break;
                    }


                    specularMode.floatValue = mode;
                }

                EditorGUI.showMixedValue = false;


                EditorGUILayout.BeginHorizontal();
                EditorGUI.indentLevel++;

                //GUILayout.Space(60);
                EditorGUI.BeginDisabledGroup(mode != 0);
                EditorGUI.showMixedValue = specularBlendMode.hasMixedValue;
                int blendingMode = (int)specularBlendMode.floatValue;
                EditorGUI.BeginChangeCheck();
                blendingMode = EditorGUILayout.Popup(Styles.specularBlendModeText, blendingMode, Enum.GetNames(typeof(UtsSpeculerColorBlendMode)));
                if (EditorGUI.EndChangeCheck())
                {
                    MMaterialEditor.RegisterPropertyChangeUndo(Styles.specularModeText.text);
                    specularBlendMode.floatValue = blendingMode;
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.EndHorizontal();
                EditorGUI.showMixedValue = false;
                EditorGUI.EndDisabledGroup();


                var ret = GUI_Toggle(material, Styles.highlightOnShadowText, ShaderPropIsUseTweakHighColorOnShadow,
                    MaterialGetInt(material, ShaderPropIsUseTweakHighColorOnShadow) != 0);
                EditorGUI.BeginDisabledGroup(!ret);
                {
                    EditorGUI.indentLevel++;
                    GUI_RangeProperty(material, Styles.tweakHighColorOnShadowText);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.Space();
            //Line();
            //EditorGUILayout.Space();

            EditorGUILayout.LabelField("Highlight Mask", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            MMaterialEditor.TexturePropertySingleLine(Styles.highColorMaskText, SetHighColorMask);
            GUI_RangeProperty(material, Styles.hilightMaskLevelText);

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
        }

        private void GUI_RimLight(Material material)
        {
            EditorGUILayout.BeginHorizontal();
            var rimLightEnabled = GUI_Toggle(material, Styles.rimlightText, ShaderPropRimLight, MaterialGetInt(material, ShaderPropRimLight) != 0);
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginDisabledGroup(!rimLightEnabled);
            EditorGUI.indentLevel++;

            GUI_ColorProperty(material, Styles.rimLightColorText);
            GUI_RangeProperty(material, Styles.rimLightPowerText);

            if (!SimpleUI)
            {
                GUI_RangeProperty(material, Styles.rimLightInsideMaskText);

                EditorGUILayout.BeginHorizontal();
                GUI_Toggle(material, Styles.rimlightFeatherText, ShaderPropRimLightFeatherOff, MaterialGetInt(material, ShaderPropRimLightFeatherOff) != 0);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();

                //GUILayout.Space(60);
                var direcitonMaskEnabled = GUI_Toggle(material, Styles.rimlightDirectionMaskText, ShaderPropLightDirectionMaskOn,
                    MaterialGetInt(material, ShaderPropLightDirectionMaskOn) != 0);
                EditorGUILayout.EndHorizontal();

                EditorGUI.BeginDisabledGroup(!direcitonMaskEnabled);
                {
                    EditorGUI.indentLevel++;
                    GUI_RangeProperty(material, Styles.lightDirectionMaskLevelText);

                    EditorGUILayout.BeginHorizontal();
                    var antipodeanRimLight = GUI_Toggle(material, Styles.invertedRimlightText, ShaderPropAddAntipodeanRimLight,
                        MaterialGetInt(material, ShaderPropAddAntipodeanRimLight) != 0);
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.BeginDisabledGroup(!antipodeanRimLight);
                    {
                        EditorGUI.indentLevel++;
                        GUI_ColorProperty(material, Styles.apRimLightColorText);
                        GUI_RangeProperty(material, Styles.invertedRimLightPowerText);

                        EditorGUILayout.BeginHorizontal();
                        GUI_Toggle(material, Styles.invertedRimlightFeatherText, ShaderPropApRimLightFeatherOff,
                            MaterialGetInt(material, ShaderPropApRimLightFeatherOff) != 0);


                        EditorGUILayout.EndHorizontal();
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.indentLevel--;
                } //Light Direction Mask ON
                EditorGUI.EndDisabledGroup();
            }

            //EditorGUI.indentLevel++;

            EditorGUILayout.Space();
            //Line();
            //EditorGUILayout.Space();

            MMaterialEditor.TexturePropertySingleLine(Styles.rimLightMaskText, SetRimLightMask);
            GUI_RangeProperty(material, Styles.rimLightMaskLevelText);

            //EditorGUI.indentLevel--;

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUI.EndDisabledGroup();
        }

        private void GUI_MatCap(Material material)
        {
            EditorGUILayout.BeginHorizontal();
            var matcapEnabled = GUI_Toggle(material, Styles.matCapText, ShaderPropMatCap, MaterialGetInt(material, ShaderPropMatCap) != 0);
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginDisabledGroup(!matcapEnabled);

            MMaterialEditor.TexturePropertySingleLine(Styles.matCapSamplerText, MatCapSampler, matCapColor);
            EditorGUI.indentLevel++;
            MMaterialEditor.TextureScaleOffsetProperty(MatCapSampler);

            if (!SimpleUI)
            {
                GUI_RangeProperty(material, Styles.matcapBlurLevelText);
                EditorGUILayout.BeginHorizontal();
                DoPopup(Styles.matcapBlendModeText, matcapBlendMode, Enum.GetNames(typeof(UtsMatcapColorBlendMode)));
                EditorGUILayout.EndHorizontal();

                GUI_RangeProperty(material, Styles.tweakMatCapUVText);
                GUI_RangeProperty(material, Styles.rotateMatCapUVText);

                EditorGUILayout.BeginHorizontal();
                GUI_Toggle(material, Styles.camearRollingStabilizerText, ShaderPropCameraRollingStabilizer,
                    MaterialGetInt(material, ShaderPropCameraRollingStabilizer) != 0);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                var isNormalMapForMatCap = GUI_Toggle(material, Styles.matCapNormalmapSpecularaMask, ShaderPropIsNormalMapForMatCap,
                    MaterialGetInt(material, ShaderPropIsNormalMapForMatCap) != 0);

                //GUILayout.Space(60);

                EditorGUILayout.EndHorizontal();
                EditorGUI.BeginDisabledGroup(!isNormalMapForMatCap);
                {
                    EditorGUI.indentLevel++;
                    MMaterialEditor.TexturePropertySingleLine(Styles.normalMapText, normalMapForMatCap, bumpScaleMatcap);
                    MMaterialEditor.TextureScaleOffsetProperty(normalMapForMatCap);
                    GUI_RangeProperty(material, Styles.rotateNormalMapForMatCapUVText);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.BeginHorizontal();
                var tweakMatCapOnShadows = GUI_Toggle(material, Styles.matCapOnShadow, ShaderPropIsUseTweakMatCapOnShadow,
                    MaterialGetInt(material, ShaderPropIsUseTweakMatCapOnShadow) != 0);

                EditorGUILayout.EndHorizontal();
                EditorGUI.BeginDisabledGroup(!tweakMatCapOnShadows);
                {
                    EditorGUI.indentLevel++;
                    GUI_RangeProperty(material, Styles.tweakMatCapOnShadowText);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.EndDisabledGroup();
                DoPopup(Styles.matcapOrthoText, matcapCameraMode, Enum.GetNames(typeof(CameraProjectionType)));
            }

            EditorGUILayout.Space();
            //Line();
            //EditorGUILayout.Space();

            EditorGUILayout.LabelField("MatCap Mask", EditorStyles.boldLabel);
            MMaterialEditor.TexturePropertySingleLine(Styles.matCapMaskText, SetMatcapMask);
            MMaterialEditor.TextureScaleOffsetProperty(SetMatcapMask);
            GUI_RangeProperty(material, Styles.tweakMatCapMaskLevelText);

            GUI_Toggle(material, Styles.invertMatCapMaskText, ShaderPropInvertMatcapMask, MaterialGetInt(material, ShaderPropInvertMatcapMask) != 0);


            EditorGUI.indentLevel--;

            EditorGUI.EndDisabledGroup();


            //EditorGUILayout.Space();
        }

        private void GUI_AngelRing(Material material)
        {
            var angelRingEnabled = GUI_Toggle(material, Styles.angelRingProjectionText, ShaderPropAngelRing,
                MaterialGetInt(material, ShaderPropAngelRing) != 0);
            EditorGUI.BeginDisabledGroup(!angelRingEnabled);
            {
                MMaterialEditor.TexturePropertySingleLine(Styles.angelRingText, AngelRingSampler, AngelRingColor);
                EditorGUI.indentLevel++;
                //m_MaterialEditor.TextureScaleOffsetProperty(angelRing_Sampler);
                GUI_RangeProperty(material, Styles.arOffsetUText);
                GUI_RangeProperty(material, Styles.arOffsetVText);

                GUI_Toggle(material, Styles.angelRingAlphaAdClippingMaskText, ShaderPropARSamplerAlphaOn,
                    MaterialGetInt(material, ShaderPropARSamplerAlphaOn) != 0);

                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();
        }

        private void ApplyQueueAndRenderType(UtsMode technique, Material material)
        {
            if (MAutoRenderQueue == 1)
            {
                material.renderQueue = -1; //  (int)UnityEngine.Rendering.RenderQueue.Geometry;
            }

            const string opaque = "Opaque";
            const string transparentcutout = "TransparentCutOut";
            const string transparent = "Transparent";
            const string rendertype = "RenderType";
            const string ignoreprojection = "IgnoreProjection";
            const string doIgnoreprojection = "True";
            const string dontIgnoreprojection = "False";
            var renderType = opaque;
            var ignoreProjection = dontIgnoreprojection;

            if (transparencyEnabled == UtsTransparentMode.On)
            {
                renderType = transparent;
                ignoreProjection = doIgnoreprojection;
            }
            else
            {
                switch (technique)
                {
                    case UtsMode.ThreeColorToon:
                    {
                        UtsClippingMode clippingMode = (UtsClippingMode)MaterialGetInt(material, ShaderPropClippingMode);
                        if (clippingMode == UtsClippingMode.Off) { }
                        else
                        {
                            renderType = transparentcutout;
                        }

                        break;
                    }
                    case UtsMode.ShadingGradeMap:
                    {
                        UtsTransClippingMode transClippingMode = (UtsTransClippingMode)MaterialGetInt(material, ShaderPropClippingMode);
                        if (transClippingMode == UtsTransClippingMode.Off) { }
                        else
                        {
                            renderType = transparentcutout;
                        }

                        break;
                    }
                }
            }

            if (MAutoRenderQueue == 1)
            {
                SetRenderQueueAuto(material);
            }
            else
            {
                material.renderQueue = MRenderQueue;
            }

            material.SetOverrideTag(rendertype, renderType);
            material.SetOverrideTag(ignoreprojection, ignoreProjection);
        }

        private void SetRenderQueueAuto(Material material)
        {
            const int transparent = (int)RenderQueue.Transparent;
            const int alphaTest = (int)RenderQueue.AlphaTest;
            const int alphaTestMinusOne = (int)RenderQueue.AlphaTest - 1;

            if (transparencyEnabled == UtsTransparentMode.On)
            {
                material.renderQueue = transparent;
                return;
            }

            UtsStencilMode stencilMode = (UtsStencilMode)MaterialGetInt(material, ShaderPropStencilMode);

            switch (stencilMode)
            {
                case UtsStencilMode.StencilMask:
                    material.renderQueue = alphaTestMinusOne;
                    return;
                case UtsStencilMode.StencilOut:
                    material.renderQueue = alphaTest;
                    return;
                case UtsStencilMode.Off:
                    ClippingMode();
                    return;
                default:
                    throw new NotImplementedException();
            }

            void ClippingMode()
            {
                UtsClippingMode clippingMode = (UtsClippingMode)MaterialGetInt(material, ShaderPropClippingMode);

                switch (clippingMode)
                {
                    case UtsClippingMode.On:
                    case UtsClippingMode.TransClippingMode:
                        material.renderQueue = alphaTest;
                        return;
                    case UtsClippingMode.Off:
                        return;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private void ApplyMatCapMode(Material material)
        {
            if (MaterialGetInt(material, ShaderPropClippingMode) == 0)
            {
                if (material.GetFloat(ShaderPropMatCap) == 1)
                    material.EnableKeyword(ShaderPropMatCap);
                else
                    material.DisableKeyword(ShaderPropMatCap);
            }
            else
            {
                material.DisableKeyword(ShaderPropMatCap);
            }
        }


        private void ApplyAngelRing(Material material)
        {
            int angelRingEnabled = MaterialGetInt(material, ShaderPropAngelRing);
            if (angelRingEnabled == 0)
            {
                material.DisableKeyword(ShaderDefineAngelringOn);
            }
            else
            {
                material.EnableKeyword(ShaderDefineAngelringOn);
            }
        }

        private void ApplyStencilMode(Material material)
        {
            UtsStencilMode mode = (UtsStencilMode)(MaterialGetInt(material, ShaderPropStencilMode));
            switch (mode)
            {
                case UtsStencilMode.Off:
                    MaterialSetInt(material, ShaderPropStencilComp, (int)StencilCompFunction.Disabled);
                    MaterialSetInt(material, ShaderPropStencilOpPass, (int)StencilOperation.Keep);
                    MaterialSetInt(material, ShaderPropStencilOpFail, (int)StencilOperation.Keep);
                    break;
                case UtsStencilMode.StencilMask:
                    MaterialSetInt(material, ShaderPropStencilComp, (int)StencilCompFunction.Always);
                    MaterialSetInt(material, ShaderPropStencilOpPass, (int)StencilOperation.Replace);
                    MaterialSetInt(material, ShaderPropStencilOpFail, (int)StencilOperation.Replace);
                    break;
                case UtsStencilMode.StencilOut:
                    MaterialSetInt(material, ShaderPropStencilComp, (int)StencilCompFunction.NotEqual);
                    MaterialSetInt(material, ShaderPropStencilOpPass, (int)StencilOperation.Keep);
                    MaterialSetInt(material, ShaderPropStencilOpFail, (int)StencilOperation.Keep);

                    break;
            }
        }

        private void ApplyClippingMode(Material material)
        {
            if (!IsShadingGrademap)
            {
                material.DisableKeyword(ShaderDefineIsTransclippingOff);
                material.DisableKeyword(ShaderDefineIsTransclippingOn);

                switch ((UtsClippingMode)MaterialGetInt(material, ShaderPropClippingMode))
                {
                    case UtsClippingMode.Off:
                        material.EnableKeyword(ShaderDefineIsClippingOff);
                        material.DisableKeyword(ShaderDefineIsClippingMode);
                        material.DisableKeyword(ShaderDefineIsClippingTransmode);
                        material.EnableKeyword(ShaderDefineIsOutlineClippingNo);
                        material.DisableKeyword(ShaderDefineIsOutlineClippingYes);
                        break;
                    case UtsClippingMode.On:
                        material.DisableKeyword(ShaderDefineIsClippingOff);
                        material.EnableKeyword(ShaderDefineIsClippingMode);
                        material.DisableKeyword(ShaderDefineIsClippingTransmode);
                        material.DisableKeyword(ShaderDefineIsOutlineClippingNo);
                        material.EnableKeyword(ShaderDefineIsOutlineClippingYes);
                        break;
                    default: // UTS_ClippingMode.TransClippingMode
                        material.DisableKeyword(ShaderDefineIsClippingOff);
                        material.DisableKeyword(ShaderDefineIsClippingMode);
                        material.EnableKeyword(ShaderDefineIsClippingTransmode);
                        material.DisableKeyword(ShaderDefineIsOutlineClippingNo);
                        material.EnableKeyword(ShaderDefineIsOutlineClippingYes);
                        break;
                }
            }
            else
            {
                material.DisableKeyword(ShaderDefineIsClippingOff);
                material.DisableKeyword(ShaderDefineIsClippingMode);
                material.DisableKeyword(ShaderDefineIsClippingTransmode);
                switch ((UtsTransClippingMode)MaterialGetInt(material, ShaderPropClippingMode))
                {
                    case UtsTransClippingMode.Off:
                        material.EnableKeyword(ShaderDefineIsTransclippingOff);
                        material.DisableKeyword(ShaderDefineIsTransclippingOn);
                        break;
                    default:
                        material.DisableKeyword(ShaderDefineIsTransclippingOff);
                        material.EnableKeyword(ShaderDefineIsTransclippingOn);
                        break;
                }
            }
        }

        private void GUI_Emissive(Material material)
        {
            MMaterialEditor.TexturePropertySingleLine(Styles.emissiveTexText, EmissiveTex, EmissiveColor);
            MMaterialEditor.TextureScaleOffsetProperty(EmissiveTex);

            int emissiveModeSetting = MaterialGetInt(material, "_EMISSIVE");
            if ((int)EmissionMode.SimpleEmissive == emissiveModeSetting)
            {
                MEmissionMode = EmissionMode.SimpleEmissive;
            }
            else if ((int)EmissionMode.EmissiveAnimation == emissiveModeSetting)
            {
                MEmissionMode = EmissionMode.EmissiveAnimation;
            }

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            var ret = EditorGUILayout.Toggle(Styles.emissionAnimationText, MEmissionMode != EmissionMode.SimpleEmissive);
            if (EditorGUI.EndChangeCheck())
            {
                MMaterialEditor.RegisterPropertyChangeUndo(Styles.emissionAnimationText.text);
                if (ret)
                {
                    material.SetFloat("_EMISSIVE", 1);
                    material.EnableKeyword("_EMISSIVE_ANIMATION");
                    material.DisableKeyword("_EMISSIVE_SIMPLE");
                }
                else
                {
                    material.SetFloat("_EMISSIVE", 0);
                    material.EnableKeyword("_EMISSIVE_SIMPLE");
                    material.DisableKeyword("_EMISSIVE_ANIMATION");
                }
            }


            EditorGUI.BeginDisabledGroup(MEmissionMode != EmissionMode.EmissiveAnimation);
            {
                EditorGUI.indentLevel++;

                GUI_FloatProperty(material, Styles.baseSpeedText);
                if (!SimpleUI)
                {
                    int mode = MaterialGetInt(material, ShaderPropIsViewCoordScroll);
                    EditorGUI.BeginChangeCheck();
                    mode = EditorGUILayout.Popup(Styles.emissiveScrollAnimationModeText, (int)mode, EmissiveScrollMode);
                    if (EditorGUI.EndChangeCheck())
                    {
                        MMaterialEditor.RegisterPropertyChangeUndo(Styles.emissiveScrollAnimationModeText.text);
                        MaterialSetInt(material, ShaderPropIsViewCoordScroll, mode);
                    }
                }


                GUI_RangeProperty(material, Styles.scrollEmissiveUText);
                GUI_RangeProperty(material, Styles.scrollEmissiveVText);
                GUI_FloatProperty(material, Styles.rotateEmissiveUVText);

                GUI_Toggle(material, Styles.pingpongMoveText, ShaderPropIsPingPongBase, MaterialGetInt(material, ShaderPropIsPingPongBase) != 0);


                if (!SimpleUI)
                {
                    EditorGUILayout.Space();

                    //GUILayout.Space(60);
                    var isColorShiftEnabled = GUI_Toggle(material, Styles.colorShitWithTimeText, ShaderPropIsColorShift,
                        MaterialGetInt(material, ShaderPropIsColorShift) != 0);


                    EditorGUI.indentLevel++;
                    EditorGUI.BeginDisabledGroup(!isColorShiftEnabled);
                    {
                        GUI_ColorProperty(material, Styles.colorShiftText);
                        GUI_FloatProperty(material, Styles.colorShiftSpeedText);
                    }
                    EditorGUI.EndDisabledGroup();

                    EditorGUI.indentLevel--;

                    EditorGUILayout.Space();

                    var isViewShiftEnabled = GUI_Toggle(material, Styles.colorShiftWithViewAngle, ShaderPropIsViewShift,
                        MaterialGetInt(material, ShaderPropIsViewShift) != 0);


                    EditorGUI.indentLevel++;
                    EditorGUI.BeginDisabledGroup(!isViewShiftEnabled);
                    GUI_ColorProperty(material, Styles.viewShiftText);
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.indentLevel--;
                } //!_SimpleUI

                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
        }



        internal static void SetupTransparentModeForOutline(Material material, bool isTransparent)
        {


#if URP_IS_INSTALLED_FOR_UTS || HDRP_IS_INSTALLED_FOR_UTS
            string srpDefaultLightModeTag = material.GetTag("LightMode", false, ToonConstants.SHADER_LIGHT_MODE_NAME_FOR_OUTLINE);
            
            if (srpDefaultLightModeTag != ToonConstants.SHADER_LIGHT_MODE_NAME_FOR_OUTLINE) 
                return;
#endif

            const string outlineColorMask = "_SPRDefaultUnlitColorMask";
            const string outlineCullMode = "_SRPDefaultUnlitColMode";

            if (isTransparent)
            {
#if URP_IS_INSTALLED_FOR_UTS || HDRP_IS_INSTALLED_FOR_UTS
                material.SetShaderPassEnabled(ToonConstants.SHADER_LIGHT_MODE_NAME_FOR_OUTLINE, true);
#endif
                MaterialSetInt(material, outlineColorMask, 0); //Don't write to the render target
                MaterialSetInt(material, outlineCullMode, (int)CullingMode.Backface);
            }
            else
            {
                MaterialSetInt(material, outlineColorMask, 15); //0xFF: presumably write to RGBA channels ?
                MaterialSetInt(material, outlineCullMode, (int)CullingMode.Frontface);
            }
        }


        private void GUI_Outline(Material material)
        {

#if URP_IS_INSTALLED_FOR_UTS || HDRP_IS_INSTALLED_FOR_UTS
            bool isOutlineEnabled = material.GetShaderPassEnabled(ToonConstants.SHADER_LIGHT_MODE_NAME_FOR_OUTLINE);
#else
            const string kDisableOutlineKeyword = "_DISABLE_OUTLINE";
            bool isOutlineEnabled = !material.IsKeywordEnabled(kDisableOutlineKeyword);
#endif

            EditorGUI.BeginChangeCheck();
            const string outlineLabel = "Outline";
            isOutlineEnabled = EditorGUILayout.Toggle(outlineLabel, isOutlineEnabled);
            if (EditorGUI.EndChangeCheck())
            {
                MMaterialEditor.RegisterPropertyChangeUndo(outlineLabel);
#if URP_IS_INSTALLED_FOR_UTS || HDRP_IS_INSTALLED_FOR_UTS
                material.SetShaderPassEnabled(ToonConstants.SHADER_LIGHT_MODE_NAME_FOR_OUTLINE, isOutlineEnabled);
#else
                if (isOutlineEnabled)
                {
                    material.DisableKeyword(kDisableOutlineKeyword);
                }
                else
                {
                    material.EnableKeyword(kDisableOutlineKeyword);
                }
#endif
            }

            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(!isOutlineEnabled);
            //
            //Express Shader property [KeywordEnum(NML,POS)] by EumPopup.
            //Load the outline mode settings in the material.
            int outlineModeSetting = MaterialGetInt(material, ToonConstants.ShaderPropToon3DOutline);
            //Convert it to Enum format and store it in the offlineMode variable.

            if ((int)OutlineMode.NormalDirection == outlineModeSetting)
            {
                MOutlineMode = OutlineMode.NormalDirection;
            }
            else if ((int)OutlineMode.PositionScaling == outlineModeSetting)
            {
                MOutlineMode = OutlineMode.PositionScaling;
            }

            //GUI description with EnumPopup.
            MOutlineMode = (OutlineMode)EditorGUILayout.EnumPopup(Styles.outlineModeText, MOutlineMode);
            //If the value changes, write to the material.
            if (MOutlineMode == OutlineMode.NormalDirection)
            {
                material.SetFloat(ToonConstants.ShaderPropToon3DOutline, 0);
                //The keywords on the UTCS_Outline.cginc side are also toggled around.
                material.EnableKeyword("_OUTLINE_NML");
                material.DisableKeyword("_OUTLINE_POS");
            }
            else if (MOutlineMode == OutlineMode.PositionScaling)
            {
                material.SetFloat(ToonConstants.ShaderPropToon3DOutline, 1);
                material.EnableKeyword("_OUTLINE_POS");
                material.DisableKeyword("_OUTLINE_NML");
            }

            GUI_FloatProperty(material, Styles.outlineWidthText);
            GUI_ColorProperty(material, Styles.outlineColorText);

            GUI_Toggle(material, Styles.baseColorToOtulineText, ToonConstants.ShaderPropToon3DIsBlendBaseColor, MaterialGetInt(material, ToonConstants.ShaderPropToon3DIsBlendBaseColor) != 0);

            MMaterialEditor.TexturePropertySingleLine(Styles.outlineSamplerText, OutlineSampler);
            GUI_FloatProperty(material, Styles.offsetZText);

            if (!SimpleUI)
            {
                EditorGUILayout.Space();
                //                _AdvancedOutline_Foldout = FoldoutSubMenu(_AdvancedOutline_Foldout, Styles.AdvancedOutlineFoldout);
                //                if (_AdvancedOutline_Foldout)
                {
                    EditorGUILayout.LabelField("Camera Distance for Outline Width");
                    EditorGUI.indentLevel++;
                    GUI_FloatProperty(material, Styles.farthestDistanceText);
                    GUI_FloatProperty(material, Styles.nearestDistanceText);
                    EditorGUI.indentLevel--;

                    var useOutlineTexture = GUI_Toggle(material, Styles.outlineColorMapText, ShaderPropIsOutlineTex,
                        MaterialGetInt(material, ShaderPropIsOutlineTex) != 0);
                    ;
                    EditorGUI.BeginDisabledGroup(!useOutlineTexture);
                    MMaterialEditor.TexturePropertySingleLine(Styles.outlineTexText, outlineTex);
                    EditorGUI.EndDisabledGroup();
                    EditorGUI.BeginDisabledGroup(MOutlineMode != OutlineMode.NormalDirection);
                    {
                        var isBackedNormal = GUI_Toggle(material, Styles.bakedNormalForOutlineText, ToonConstants.ShaderPropToon3DIsBakedNormal,
                            MaterialGetInt(material, ToonConstants.ShaderPropToon3DIsBakedNormal) != 0);
                        EditorGUI.BeginDisabledGroup(!isBackedNormal);
                        MMaterialEditor.TexturePropertySingleLine(Styles.bakedNormalOutlineText, bakedNormal);
                        EditorGUI.EndDisabledGroup();
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUI.EndDisabledGroup(); //!isOutlineEnabled
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
        }

        private void GUI_LightColorEffectiveness(Material material)
        {
            EditorGUILayout.LabelField(Styles.lightColorEffectiveness);
            EditorGUI.indentLevel++;
            GUI_Toggle(material, Styles.lightColorEffectivinessToBaseColorText, ShaderPropIsLightColorBase,
                MaterialGetInt(material, ShaderPropIsLightColorBase) != 0);
            GUI_Toggle(material, Styles.lightColorEffectivinessTo1StShadingText, ShaderPropIsLightColor1StShade,
                MaterialGetInt(material, ShaderPropIsLightColor1StShade) != 0);
            GUI_Toggle(material, Styles.lightColorEffectivinessTo2NdShadingText, ShaderPropIsLightColor2NdShade,
                MaterialGetInt(material, ShaderPropIsLightColor2NdShade) != 0);
            GUI_Toggle(material, Styles.lightColorEffectivinessToHighlitText, ShaderPropIsLightColorHighColor,
                MaterialGetInt(material, ShaderPropIsLightColorHighColor) != 0);
            GUI_Toggle(material, Styles.lightColorEffectivinessToRimlitText, ShaderPropIsLightColorRimLight,
                MaterialGetInt(material, ShaderPropIsLightColorRimLight) != 0);
            GUI_Toggle(material, Styles.lightColorEffectivinessToInvRimlitText, ShaderPropIsLightColorApRimLight,
                MaterialGetInt(material, ShaderPropIsLightColorApRimLight) != 0);

            EditorGUI.BeginDisabledGroup(_mWorkflowMode != UtsMode.ShadingGradeMap);
            {
                GUI_Toggle(material, Styles.lightColorEffectivenessToAngelRingText, ShaderPropIsLightColorAR,
                    MaterialGetInt(material, ShaderPropIsLightColorAR) != 0);
            }
            EditorGUI.EndDisabledGroup();

            GUI_Toggle(material, Styles.lightColorEffectivinessToMatCapText, ShaderPropIsLightColorMatCap,
                MaterialGetInt(material, ShaderPropIsLightColorMatCap) != 0);
            GUI_Toggle(material, Styles.lightColorEffectivinessToOutlineText, ToonConstants.ShaderPropToon3DIsLightColorOutline,
                MaterialGetInt(material, ToonConstants.ShaderPropToon3DIsLightColorOutline) != 0);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            GUI_RangeProperty(material, Styles.giIntensityText);

            EditorGUI.BeginChangeCheck();
            var prop = ShaderPropIsLightColorBase;

            var value = MaterialGetInt(material, prop);
            var ret = EditorGUILayout.Toggle(Styles.limitLightIntensityText.text, value != 0);
            if (EditorGUI.EndChangeCheck())
            {
                var boolValue = ret ? 1 : 0;
                MMaterialEditor.RegisterPropertyChangeUndo(Styles.limitLightIntensityText.text);
                MaterialSetInt(material, prop, boolValue);
                if (boolValue != 0)
                {
                    MaterialSetInt(material, ShaderPropIsLightColorBase, boolValue);
                    MaterialSetInt(material, ShaderPropIsLightColor1StShade, boolValue);
                    MaterialSetInt(material, ShaderPropIsLightColor2NdShade, boolValue);
                    if (material.HasProperty(ToonConstants.ShaderPropToon3DOutline)) //If OUTLINE is available.
                    {
                        MaterialSetInt(material, ToonConstants.ShaderPropToon3DIsLightColorOutline, boolValue);
                    }
                }
            }

            EditorGUILayout.Space();
        }

        private void DoPopup(GUIContent label, MaterialProperty property, string[] options)
        {
            DoPopup(label, property, options, MMaterialEditor);
        }

        private static void DoPopup(GUIContent label, MaterialProperty property, string[] options, MaterialEditor materialEditor)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            EditorGUI.showMixedValue = property.hasMixedValue;

            var mode = property.floatValue;

            EditorGUI.BeginChangeCheck();
            mode = EditorGUILayout.Popup(label, (int)mode, options);
            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo(label.text);
                property.floatValue = mode;
            }

            EditorGUI.showMixedValue = false;
        }
    }

    internal static class ToonConstants
    {

        //Common constants
        //Colors
        internal const string ShaderPropBaseColor = "_BaseColor";
        internal const string ShaderPropMainTEX = "_MainTex";

        internal const string ShaderPropNormalMap = "_NormalMap";
        internal const string ShaderPropBumpScale = "_BumpScale";

        internal const string ShaderProp1StShadeColor = "_1st_ShadeColor";
        internal const string ShaderProp1StShadeMap = "_1st_ShadeMap";
        internal const string ShaderPropUseBaseAs1St = "_Use_BaseAs1st";

        internal const string ShaderProp2NdShadeColor = "_2nd_ShadeColor";
        internal const string ShaderProp2NdShadeMap = "_2nd_ShadeMap";
        internal const string ShaderPropUse1StAs2Nd = "_Use_1stAs2nd";
        
        //Outline
        internal const string ShaderPropOutlineTEX = "_OutlineTex";

        // Toon 3D (legacy) shader property names (used for upgrade/mapping flows)
        internal const string ShaderPropToon3DBaseColorStep = "_BaseColor_Step";
        internal const string ShaderPropToon3DBaseShadeFeather = "_BaseShade_Feather";
        internal const string ShaderPropToon3D1StShadeColorStep = "_1st_ShadeColor_Step";
        internal const string ShaderPropToon3D1StShadeColorFeather = "_1st_ShadeColor_Feather";

        internal const string ShaderPropToon3DHighColor = "_HighColor";
        internal const string ShaderPropToon3DHighColorTEX = "_HighColor_Tex";

        internal const string ShaderPropToon3DOutline = "_OUTLINE";
        internal const string ShaderPropToon3DOutlineWidth = "_Outline_Width";
        internal const string ShaderPropToon3DOutlineSampler = "_Outline_Sampler";
        internal const string ShaderPropToon3DOutlineColor = "_Outline_Color";
        internal const string ShaderPropToon3DIsBlendBaseColor = "_Is_BlendBaseColor";
        internal const string ShaderPropToon3DIsLightColorOutline = "_Is_LightColor_Outline";
        internal const string ShaderPropToon3DOffsetZ = "_Offset_Z";
        internal const string ShaderPropToon3DNearestDistance = "_Nearest_Distance";
        internal const string ShaderPropToon3DFarthestDistance = "_Farthest_Distance";

        internal const string ShaderPropToon3DIsBakedNormal = "_Is_BakedNormal";
        internal const string ShaderPropToon3DBakedNormal = "_BakedNormal";

    }

}
