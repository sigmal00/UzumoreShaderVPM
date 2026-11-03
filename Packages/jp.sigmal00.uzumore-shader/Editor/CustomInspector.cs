#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace lilToon
{
    public class UzumoreInspector : lilToonInspector
    {
        // Custom properties
        MaterialProperty uzumoreAmount;
        MaterialProperty uzumoreBias;
        MaterialProperty uzumoreMask;
        GUIContent uzumoreMaskLabel;
        bool shouldLoadLanguage;

        private static bool isShowCustomProperties;
        private static string prevLanguageName;
        private const string shaderName = "Sigmal00/Uzumore";

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            shouldLoadLanguage = lts == null || lts.name != shaderName + "/lilToon" || prevLanguageName != lilLanguageManager.langSet.languageName;
            base.OnGUI(materialEditor, props);
        }

        protected override void LoadCustomProperties(MaterialProperty[] props, Material material)
        {
            isCustomShader = true;

            // If you want to change rendering modes in the editor, specify the shader here
            ReplaceToCustomShaders();
            isShowRenderMode = !material.shader.name.Contains("Optional");

            // If not, set isShowRenderMode to false
            //isShowRenderMode = false;

            if (shouldLoadLanguage)
            {
                LoadCustomLanguage("0bbf2656367e4884686b214c7ac0184c");
                uzumoreMaskLabel = new GUIContent(GetLoc("sUzumoreMask"));
            }

            uzumoreAmount = FindProperty("_UzumoreAmount", props);
            uzumoreBias = FindProperty("_UzumoreBias", props);
            uzumoreMask = FindProperty("_UzumoreMask", props);
        }

        protected override void DrawCustomProperties(Material material)
        {
            // GUIStyles Name   Description
            // ---------------- ------------------------------------
            // boxOuter         outer box
            // boxInnerHalf     inner box
            // boxInner         inner box without label
            // customBox        box (similar to unity default box)
            // customToggleFont label for box

            var titleLoc = GetLoc("sCustomShaderTitle");
            isShowCustomProperties = Foldout(titleLoc, titleLoc, isShowCustomProperties);
            if(isShowCustomProperties)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                EditorGUILayout.LabelField(GetLoc("sCustomPropertyCategory"), customToggleFont);
                EditorGUILayout.BeginVertical(boxInnerHalf);

                lilEditorGUI.LocalizedProperty(m_MaterialEditor, uzumoreAmount);
                lilEditorGUI.TexturePropertySingleLine(m_MaterialEditor, uzumoreMaskLabel, uzumoreMask);
                lilEditorGUI.LocalizedProperty(m_MaterialEditor, uzumoreBias);
                EditorGUILayout.HelpBox(GetLoc("sUzumoreHelp"), MessageType.Info);

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
            }
        }

        protected override void ReplaceToCustomShaders()
        {
            lts         = Shader.Find(shaderName + "/lilToon");
            ltsc        = Shader.Find("Hidden/" + shaderName + "/Cutout");
            ltst        = Shader.Find("Hidden/" + shaderName + "/Transparent");
            ltsot       = Shader.Find("Hidden/" + shaderName + "/OnePassTransparent");
            ltstt       = Shader.Find("Hidden/" + shaderName + "/TwoPassTransparent");

            ltso        = Shader.Find("Hidden/" + shaderName + "/OpaqueOutline");
            ltsco       = Shader.Find("Hidden/" + shaderName + "/CutoutOutline");
            ltsto       = Shader.Find("Hidden/" + shaderName + "/TransparentOutline");
            ltsoto      = Shader.Find("Hidden/" + shaderName + "/OnePassTransparentOutline");
            ltstto      = Shader.Find("Hidden/" + shaderName + "/TwoPassTransparentOutline");

            ltsoo       = Shader.Find(shaderName + "/[Optional] OutlineOnly/Opaque");
            ltscoo      = Shader.Find(shaderName + "/[Optional] OutlineOnly/Cutout");
            ltstoo      = Shader.Find(shaderName + "/[Optional] OutlineOnly/Transparent");

            ltstess     = Shader.Find("Hidden/" + shaderName + "/Tessellation/Opaque");
            ltstessc    = Shader.Find("Hidden/" + shaderName + "/Tessellation/Cutout");
            ltstesst    = Shader.Find("Hidden/" + shaderName + "/Tessellation/Transparent");
            ltstessot   = Shader.Find("Hidden/" + shaderName + "/Tessellation/OnePassTransparent");
            ltstesstt   = Shader.Find("Hidden/" + shaderName + "/Tessellation/TwoPassTransparent");

            ltstesso    = Shader.Find("Hidden/" + shaderName + "/Tessellation/OpaqueOutline");
            ltstessco   = Shader.Find("Hidden/" + shaderName + "/Tessellation/CutoutOutline");
            ltstessto   = Shader.Find("Hidden/" + shaderName + "/Tessellation/TransparentOutline");
            ltstessoto  = Shader.Find("Hidden/" + shaderName + "/Tessellation/OnePassTransparentOutline");
            ltstesstto  = Shader.Find("Hidden/" + shaderName + "/Tessellation/TwoPassTransparentOutline");

            ltsl        = Shader.Find(shaderName + "/lilToonLite");
            ltslc       = Shader.Find("Hidden/" + shaderName + "/Lite/Cutout");
            ltslt       = Shader.Find("Hidden/" + shaderName + "/Lite/Transparent");
            ltslot      = Shader.Find("Hidden/" + shaderName + "/Lite/OnePassTransparent");
            ltsltt      = Shader.Find("Hidden/" + shaderName + "/Lite/TwoPassTransparent");

            ltslo       = Shader.Find("Hidden/" + shaderName + "/Lite/OpaqueOutline");
            ltslco      = Shader.Find("Hidden/" + shaderName + "/Lite/CutoutOutline");
            ltslto      = Shader.Find("Hidden/" + shaderName + "/Lite/TransparentOutline");
            ltsloto     = Shader.Find("Hidden/" + shaderName + "/Lite/OnePassTransparentOutline");
            ltsltto     = Shader.Find("Hidden/" + shaderName + "/Lite/TwoPassTransparentOutline");

            ltsref      = Shader.Find("Hidden/" + shaderName + "/Refraction");
            ltsrefb     = Shader.Find("Hidden/" + shaderName + "/RefractionBlur");
            ltsfur      = Shader.Find("Hidden/" + shaderName + "/Fur");
            ltsfurc     = Shader.Find("Hidden/" + shaderName + "/FurCutout");
            ltsfurtwo   = Shader.Find("Hidden/" + shaderName + "/FurTwoPass");
            ltsfuro     = Shader.Find(shaderName + "/[Optional] FurOnly/Transparent");
            ltsfuroc    = Shader.Find(shaderName + "/[Optional] FurOnly/Cutout");
            ltsfurotwo  = Shader.Find(shaderName + "/[Optional] FurOnly/TwoPass");
            ltsgem      = Shader.Find("Hidden/" + shaderName + "/Gem");
            ltsfs       = Shader.Find(shaderName + "/[Optional] FakeShadow");

            ltsover     = Shader.Find(shaderName + "/[Optional] Overlay");
            ltsoover    = Shader.Find(shaderName + "/[Optional] OverlayOnePass");
            ltslover    = Shader.Find(shaderName + "/[Optional] LiteOverlay");
            ltsloover   = Shader.Find(shaderName + "/[Optional] LiteOverlayOnePass");

            ltsm        = Shader.Find(shaderName + "/lilToonMulti");
            ltsmo       = Shader.Find("Hidden/" + shaderName + "/MultiOutline");
            ltsmref     = Shader.Find("Hidden/" + shaderName + "/MultiRefraction");
            ltsmfur     = Shader.Find("Hidden/" + shaderName + "/MultiFur");
            ltsmgem     = Shader.Find("Hidden/" + shaderName + "/MultiGem");
        }

        // You can create a menu like this
        [MenuItem("Assets/UzumoreShader/Convert material to UzumoreShader", false, 1100)]
        private static void ConvertMaterialToCustomShaderMenu()
        {
            if(Selection.objects.Length == 0) return;
            UzumoreInspector inspector = new UzumoreInspector();
            for(int i = 0; i < Selection.objects.Length; i++)
            {
                Material material = Selection.objects[i] as Material;
                if(material == null)
                {
                    continue;
                }

#if UNITY_2022_1_OR_NEWER
                if(material.parent != null)
                {
                    Debug.LogWarningFormat("Ignore Material Variant: {0}", AssetDatabase.GetAssetPath(material));
                    continue;
                }
#endif  // UNITY_2022_1_OR_NEWER

                inspector.ConvertMaterialToCustomShader((Material)Selection.objects[i]);
            }
        }

        [MenuItem("Assets/UzumoreShader/Copy && Convert material to UzumoreShader", false, 1101)]
        private static void CopyAndConvertMaterialToCustomShaderMenu()
        {
            if(Selection.objects.Length == 0) return;
            UzumoreInspector inspector = new UzumoreInspector();
            for(int i = 0; i < Selection.objects.Length; i++)
            {
                Material originalMaterial = Selection.objects[i] as Material;
                if(originalMaterial == null)
                {
                    continue;
                }

#if UNITY_2022_1_OR_NEWER
                if(originalMaterial.parent != null)
                {
                    Debug.LogWarningFormat("Ignore Material Variant: {0}", AssetDatabase.GetAssetPath(originalMaterial));
                    continue;
                }
#endif  // UNITY_2022_1_OR_NEWER

                Material copiedMaterial = new Material(originalMaterial);
                inspector.ConvertMaterialToCustomShader(copiedMaterial);
                string path = AssetDatabase.GetAssetPath(originalMaterial);
                string newPath = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + "_Uzumore" + Path.GetExtension(path);

                if(File.Exists(newPath))
                {
                    string directory = Path.GetDirectoryName(newPath);
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newPath);
                    string extension = Path.GetExtension(newPath);
                    int counter = 1;
                    
                    while(File.Exists(newPath))
                    {
                        newPath = directory + "/" + fileNameWithoutExtension + " " + counter + extension;
                        counter++;
                    }
                }

                AssetDatabase.CreateAsset(copiedMaterial, newPath);
            }
        }
    }
}
#endif
