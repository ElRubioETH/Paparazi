using UnityEngine;
using UnityEditor;

public class MaterialURPConverter : EditorWindow
{
    [MenuItem("Tools/Convert Materials to URP")]
    public static void ConvertMaterials()
    {
        string[] materialGuids = AssetDatabase.FindAssets("t:Material");
        int converted = 0;

        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat == null)
                continue;

            string shaderName = mat.shader.name;

            // Kiểm tra shader thuộc legacy hoặc built-in
            if (shaderName.StartsWith("Legacy") || shaderName.StartsWith("Particles") || shaderName.StartsWith("Mobile") || shaderName.StartsWith("Sprites") || shaderName.StartsWith("Standard"))
            {
                mat.shader = Shader.Find("Universal Render Pipeline/Lit");

                // Nếu có texture main
                if (mat.HasProperty("_MainTex"))
                {
                    Texture mainTex = mat.GetTexture("_MainTex");
                    if (mainTex != null && mat.HasProperty("_BaseMap"))
                        mat.SetTexture("_BaseMap", mainTex);
                }

                EditorUtility.SetDirty(mat);
                converted++;
                Debug.Log($"✅ Converted {mat.name} to URP/Lit");
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"🎉 Converted {converted} materials to URP!");
        EditorUtility.DisplayDialog("Done!", $"Converted {converted} materials to URP.", "OK");
    }
}
