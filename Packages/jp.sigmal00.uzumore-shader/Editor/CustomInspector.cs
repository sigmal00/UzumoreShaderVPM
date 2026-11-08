#if UNITY_EDITOR
using System.IO;
using System.Collections.Generic;
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
        readonly List<MaterialProperty> propertyList = new List<MaterialProperty>(3);
        GUIContent uzumoreMaskLabel;
        bool shouldLoadLanguage;

        private static bool isShowCustomProperties;
        private static string prevLanguageName;
        private static readonly Dictionary<string, MaterialProperty> copySourcePropertyDict = new Dictionary<string, MaterialProperty>();
        private const string shaderName = "Sigmal00/Uzumore";
        private const string helpUrl = "https://github.com/sigmal00/UzumoreShaderVPM";

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            shouldLoadLanguage = lts?.name != shaderName + "/lilToon" || prevLanguageName != lilLanguageManager.langSet.languageName;
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

            propertyList.Clear();
            propertyList.Add(uzumoreAmount);
            propertyList.Add(uzumoreBias);
            propertyList.Add(uzumoreMask);
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

            if (!ShouldDrawBlock(propertyList))
            {
                return;
            }

            var titleLoc = GetLoc("sCustomShaderTitle");
            isShowCustomProperties = Foldout(titleLoc, titleLoc, isShowCustomProperties);
            DrawMenuButton(propertyList, helpUrl);
            if(isShowCustomProperties)
            {
                EditorGUILayout.BeginVertical(boxOuter);
                EditorGUILayout.LabelField(GetLoc("sCustomPropertyCategory"), customToggleFont);
                DrawMenuButton(propertyList, helpUrl);
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

        private static bool ShouldDrawBlock(List<MaterialProperty> propertyList)
        {
            foreach (var property in propertyList)
            {
                if (lilEditorGUI.CheckPropertyToDraw(property))
                {
                    return true;
                }
            }
            return false;
        }

        private static void DrawMenuButton(List<MaterialProperty> propertyList, string helpUrl = null)
        {
            var position = GUILayoutUtility.GetLastRect();
            position.x += position.width - 24.0f;
            position.width = 24.0f;

            if (!GUI.Button(position, EditorGUIUtility.IconContent("_Popup"), middleButton))
            {
                return;
            }

            var menu = new GenericMenu();
            menu.AddItem(new GUIContent(GetLoc("sCopy")), false, userData => CopyProperties((List<MaterialProperty>)userData), propertyList);
            menu.AddItem(new GUIContent(GetLoc("sPaste")), false, userData => PasteProperties((List<MaterialProperty>)userData, false), propertyList);
            menu.AddItem(new GUIContent(GetLoc("sPasteWithTexture")), false, userData => PasteProperties((List<MaterialProperty>)userData, true), propertyList);
#if UNITY_2019_3_OR_NEWER
            menu.AddItem(new GUIContent(GetLoc("sReset")), false, userData => ResetProperties((List<MaterialProperty>)userData), propertyList);
#endif  // UNITY_2019_3_OR_NEWER
            menu.AddItem(new GUIContent(GetLoc("sOpenManual")), false, userData => OpenHelpPage((string)userData), helpUrl);
            menu.ShowAsContext();
        }

        private static void CopyProperties(List<MaterialProperty> propertyList)
        {
            copySourcePropertyDict.Clear();
            foreach (var prop in propertyList)
            {
                copySourcePropertyDict[prop.name] = prop;
            }
        }

        private static void PasteProperties(List<MaterialProperty> propertyList, bool shouldCopyTex)
        {
            foreach (var prop in propertyList)
            {
                if (!copySourcePropertyDict.TryGetValue(prop.name, out var sourceProp))
                {
                    continue;
                }

                switch (prop.type)
                {
                    case MaterialProperty.PropType.Color:
                        prop.colorValue = sourceProp.colorValue;
                        break;
                    case MaterialProperty.PropType.Vector:
                        prop.vectorValue = sourceProp.vectorValue;
                        break;
                    case MaterialProperty.PropType.Float:
                    case MaterialProperty.PropType.Range:
                        prop.floatValue = sourceProp.floatValue;
                        break;
                    case MaterialProperty.PropType.Texture:
                        if (shouldCopyTex)
                        {
                            prop.textureValue = sourceProp.textureValue;
                            prop.textureScaleAndOffset = sourceProp.textureScaleAndOffset;
                        }
                        break;
                    case MaterialProperty.PropType.Int:
                        prop.intValue = sourceProp.intValue;
                        break;
                }
            }
        }

#if UNITY_2019_3_OR_NEWER
        private static void ResetProperties(List<MaterialProperty> propertyList)
        {
            foreach (var prop in propertyList)
            {
                var shader = (prop.targets[0] as Material)?.shader;
                if (shader == null)
                {
                    continue;
                }

                var propID = shader.FindPropertyIndex(prop.name);
                if (propID == -1)
                {
                    continue;
                }

                switch (prop.type)
                {
                    case MaterialProperty.PropType.Color:
                        prop.colorValue = shader.GetPropertyDefaultVectorValue(propID);
                        break;
                    case MaterialProperty.PropType.Vector:
                        prop.vectorValue = shader.GetPropertyDefaultVectorValue(propID);
                        break;
                    case MaterialProperty.PropType.Float:
                    case MaterialProperty.PropType.Range:
                        prop.floatValue = shader.GetPropertyDefaultFloatValue(propID);
                        break;
                    case MaterialProperty.PropType.Texture:
                        prop.textureValue = null;
                        prop.textureScaleAndOffset = new Vector4(1.0f, 1.0f, 0.0f, 0.0f);
                        break;
                    case MaterialProperty.PropType.Int:
                        prop.intValue = shader.GetPropertyDefaultIntValue(propID);
                        break;
                }
            }
        }
#endif  // UNITY_2019_3_OR_NEWER

        private static void OpenHelpPage(string url)
        {
            Application.OpenURL(url);
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

                Undo.RecordObject(material, "Assign customized lilToon shader (MenuItem: \"Assets/UzumoreShader/Convert material to UzumoreShader)\"");

                string tagval = material.GetTag("VRCFallback", false);

                inspector.ConvertMaterialToCustomShader(material);

                if(tagval.Length != 0)
                {
                    AssetDatabase.SaveAssetIfDirty(material);
                    material.SetOverrideTag("VRCFallback", tagval);
                }
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
                    string basePart = Path.GetDirectoryName(newPath) + "/" + Path.GetFileNameWithoutExtension(newPath);
#if UNITY_2020_1_OR_NEWER
                    if(EditorSettings.assetNamingUsesSpace)
                    {
                        basePart += " ";
                    }
#else
                    basePath += " ";
#endif  // UNITY_2020_1_OR_NEWER
                    string extension = Path.GetExtension(newPath);
                    int counter = 1;

                    do
                    {
                        newPath = basePart + counter + extension;
                        counter++;
                    }
                    while (File.Exists(newPath));
                }

                AssetDatabase.CreateAsset(copiedMaterial, newPath);
            }
        }
    }
}
#endif
