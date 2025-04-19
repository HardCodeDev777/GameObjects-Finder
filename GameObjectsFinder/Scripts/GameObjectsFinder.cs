// Copyright (c) 2025 HardCodeDev
// This code is licensed under the HardCodeDev License (Modified MIT).
// You may use, modify, and distribute this software under the terms of the LICENSE file.
// Selling this script as a standalone product or as part of a script collection is prohibited.
// If modified and redistributed, it must be stated that the script is based on the original work of HardCodeDev.
// Full license details can be found in LICENSE.txt.

using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace HardCodeDev.GameObjectsFinder
{
    #region Other
    public enum TypeOfObjects
    {
        Script,
        Tag
    }

    [Serializable]
    public struct GlobalIdBasic
    {
        public string globalId, materialPath;
    }

    [Serializable]
    public class GlobalID 
    {
        public string findedMat;
        public List<GlobalIdBasic> tagObjects = new(), scriptObjects = new();
    }
    #endregion

    #region Main Script
    public class GameObjectsFinder : EditorWindow
    {
        #region Variables
        private bool enableExtraDebug, saveToJson, clearedByScript, clearedByTag;

        private string gameObjTag, scriptName, JSONpath;

        private List<GameObject> findedGameObjsByTag = new(), findedGameObjsByScript = new();

        private Dictionary<GameObject, Material> defaultMaterialsByTag = new(), defaultMaterialsByScript = new();

        private Material findedMaterial;
        #endregion

        #region GUI
        [MenuItem("Finder/GameObjects Finder")]
        public static void ShowWindow() => GetWindow<GameObjectsFinder>("GameObjects Finder");

        private void OnEnable()
        {
            // Values for GameObjectsFinderDemo scene.

            saveToJson = true;

            gameObjTag = "Player";
            scriptName = "HardCodeDev.Examples.EmptyDemoScript";
            JSONpath = "Assets/Data.json";
        }

        private void OnGUI()
        {
            GUILayout.Label("Base", EditorStyles.boldLabel);

            enableExtraDebug = EditorGUILayout.Toggle(
                new GUIContent("Enable extra debugging", "Enables additional Debug logs. Turn it on if you need extra debugging (recommended)."),
                enableExtraDebug);

            EditorGUILayout.Space(5);

            saveToJson = EditorGUILayout.Toggle(
    new GUIContent("Save to JSON", "Automatically saves found GameObjects to JSON by their Global ID, their materials before finding and applied material."),
    saveToJson);

            if (saveToJson)
            {
                JSONpath = EditorGUILayout.TextField(
                new GUIContent("JSON data save path ", "Example: Assets/Data.json"), JSONpath);

                if (GUILayout.Button(
                    new GUIContent("Load JSON data")))
                    LoadFromJSON();
                if (GUILayout.Button("Clear JSON file")) ClearJSON();
            }

            EditorGUILayout.Space(5);

            findedMaterial = (Material)EditorGUILayout.ObjectField(
                new GUIContent("Applied material", "Material to apply to found objects."),
                findedMaterial, typeof(Material), true);

            EditorGUILayout.Space(10);

            GUILayout.Label("Search by Tag", EditorStyles.boldLabel);
            gameObjTag = EditorGUILayout.TagField(
                new GUIContent("GameObjects tag", "Tag to search objects by"),
                gameObjTag);

            EditorGUILayout.Space(10);

            GUILayout.Label("Search by Script", EditorStyles.boldLabel);
            scriptName = EditorGUILayout.TextField(
                new GUIContent("GameObjects full script name", "Full name of the script (with namespace if needed). Example: MyNamespace.MyScript"),
                scriptName);

            EditorGUILayout.Space(10);

            GUILayout.Label("Finding", EditorStyles.boldLabel);

            if (GUILayout.Button("Find Game Objects by Tag")) FindGameObjects(TypeOfObjects.Tag);
            if (GUILayout.Button("Clear Game Objects by Tag")) ClearMaterials(TypeOfObjects.Tag);
            EditorGUILayout.Space(10);

            if (GUILayout.Button("Find Game Objects by Script")) FindGameObjects(TypeOfObjects.Script);
            if (GUILayout.Button("Clear Game Objects by Script")) ClearMaterials(TypeOfObjects.Script);


            EditorGUILayout.Space(10);

            DrawList(TypeOfObjects.Script);
            DrawList(TypeOfObjects.Tag);
        }

        private void DrawList(TypeOfObjects type)
        {
            List<GameObject> finded = new();

            finded = type == TypeOfObjects.Tag ? findedGameObjsByTag: findedGameObjsByScript;
            var cleared = type == TypeOfObjects.Tag ? clearedByTag : clearedByScript;

            if (finded != null)
            {
                for (int i = 0; i < finded.Count; i++)
                {
                    if (!cleared)
                    {
                        EditorGUILayout.BeginHorizontal();

                        finded[i] = type == TypeOfObjects.Tag ?
                            (GameObject)EditorGUILayout.ObjectField("By Tag: ", finded[i], typeof(GameObject), true)
                            : (GameObject)EditorGUILayout.ObjectField("By Script: ", finded[i], typeof(GameObject), true);

                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        finded.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        #endregion

        #region Core
        private void FindGameObjects(TypeOfObjects type)
        {
            if (type == TypeOfObjects.Tag)
            {
                if (gameObjTag != null)
                {
                    ClearMaterials(TypeOfObjects.Tag, true);
                    clearedByTag = false;

                    try
                    {
                        var gameObjs = GameObject.FindGameObjectsWithTag(gameObjTag);
                        foreach (var gameObj in gameObjs)
                        {
                            if (gameObj.TryGetComponent(out MeshRenderer renderer))
                            {
                                if (!defaultMaterialsByTag.ContainsKey(gameObj)) defaultMaterialsByTag[gameObj] = renderer.sharedMaterial;
                                renderer.sharedMaterial = findedMaterial;
                            }
                            findedGameObjsByTag.Add(gameObj);
                        }
                        if (saveToJson) SaveToJSON();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"The tag does not exist! It may have been deleted. Error message: {ex.Message}");
                    }

                    if (findedGameObjsByTag.Count > 0)
                    {
                        if (enableExtraDebug) Debug.Log($"<color=green>{findedGameObjsByTag.Count} objects found!");
                    }
                    else
                    {
                        if (enableExtraDebug) Debug.Log("<color=yellow>No objects found!");
                    }
                }
                else Debug.LogError("No tag entered in GameObjects tag!"); 
            }

            if (type == TypeOfObjects.Script)
            {
                if (scriptName != null)
                {
                    ClearMaterials(TypeOfObjects.Script, true);
                    clearedByScript = false;

                    Type scriptType = Type.GetType(scriptName);
                    if (scriptType != null)
                    {
                        var scripts = FindObjectsByType(scriptType, FindObjectsSortMode.None);
                        foreach (var script in scripts)
                        {
                            Component component = script as Component;
                            if (component != null)
                            {
                                GameObject gameObj = component.gameObject;
                                if (gameObj.TryGetComponent(out MeshRenderer renderer))
                                {
                                    if (!defaultMaterialsByScript.ContainsKey(gameObj)) defaultMaterialsByScript[gameObj] = renderer.sharedMaterial;
                                    renderer.sharedMaterial = findedMaterial;
                                }
                                findedGameObjsByScript.Add(gameObj);
                            }
                        }
                        if(saveToJson)SaveToJSON();
                    }
                    else
                    {
                        if (enableExtraDebug) Debug.Log("<color=yellow>No objects found with this script.");
                    }

                    if (findedGameObjsByScript.Count > 0)
                    {
                        if (enableExtraDebug) Debug.Log($"<color=green>{findedGameObjsByScript.Count} objects found!");
                    }

                    else
                    {
                        if (enableExtraDebug) Debug.Log("<color=yellow>No objects found with this script.");
                    }
                }
                else Debug.LogError("No script name entered in GameObjects script name!"); 
            }
        }

        private void ClearMaterials(TypeOfObjects type, bool isInFinder = false)
        {
            clearedByScript = type == TypeOfObjects.Tag ? false : true;
            clearedByTag = type == TypeOfObjects.Tag ? true : false;

            List<GameObject> findedGameObjs = new();
            Dictionary<GameObject, Material> defaultMaterials = new();

            findedGameObjs = type == TypeOfObjects.Tag ? findedGameObjsByTag : findedGameObjsByScript;
            defaultMaterials = type == TypeOfObjects.Tag ? defaultMaterialsByTag : defaultMaterialsByScript;

            if (findedGameObjs.Count != 0)
            {
                foreach (var gameObj in findedGameObjs)
                {
                    if (defaultMaterials.TryGetValue(gameObj, out Material originalMat))
                    {
                        try
                        {
                            gameObj.GetComponent<MeshRenderer>().sharedMaterial = originalMat;
                            if (enableExtraDebug && !isInFinder) Debug.Log("<color=green>Materials cleared successfully!");
                        }
                        catch
                        {
                            Debug.LogError("The MeshRenderer component was removed from the GameObjects during material clearing!");
                        }
                    }
                }
                findedGameObjs.Clear();
                defaultMaterials.Clear();
            }
            else
            {
                if (enableExtraDebug && !isInFinder) Debug.Log("<color=yellow>No objects found for clearing.");
            }
        }
        #endregion

        #region JSON
        private void SaveToJSON() 
        {
            if (JSONpath != null)
            {
                GlobalID global = new();

                GlobalID globalExisted = new();
                var jsonExisted = File.ReadAllText(JSONpath);
                globalExisted = JsonUtility.FromJson<GlobalID>(jsonExisted);

                if (findedGameObjsByScript != null)
                {
                    for (int i = 0; i < findedGameObjsByScript.Count; i++)
                    {
                        GlobalIdBasic basic = new();    
                        var globalId = GlobalObjectId.GetGlobalObjectIdSlow(findedGameObjsByScript[i]).ToString();
                        basic.globalId = globalId;


                        if (findedGameObjsByScript[i].TryGetComponent(out MeshRenderer renderer)) 
                        {
                            var mat = defaultMaterialsByScript[findedGameObjsByScript[i]];
                            string path = AssetDatabase.GetAssetPath(mat);
                            basic.materialPath = path;
                        }

                        global.scriptObjects.Add(basic);
                    }
                    
                    global.findedMat = AssetDatabase.GetAssetPath(findedMaterial);

                    if (enableExtraDebug) Debug.Log("<color=green>Found by scrpt objects data saved to JSON successfully!");
                }

                if (findedGameObjsByTag != null)
                {
                    for (int i = 0; i < findedGameObjsByTag.Count; i++)
                    {
                        GlobalIdBasic basic = new();
                        var globalId = GlobalObjectId.GetGlobalObjectIdSlow(findedGameObjsByTag[i]).ToString();
                        basic.globalId = globalId;


                        if (findedGameObjsByTag[i].TryGetComponent(out MeshRenderer renderer))
                        {
                            var mat = defaultMaterialsByTag[findedGameObjsByTag[i]];
                            string path = AssetDatabase.GetAssetPath(mat);
                            basic.materialPath = path;
                        }

                        global.tagObjects.Add(basic);
                    }

                    global.findedMat = AssetDatabase.GetAssetPath(findedMaterial);

                    if (enableExtraDebug) Debug.Log("<color=green>Found by tag objects data saved to JSON successfully!");
                }

                var updatedJson = JsonUtility.ToJson(global, true);

                File.WriteAllText(JSONpath, updatedJson);
                AssetDatabase.Refresh();
                if (enableExtraDebug) Debug.Log("<color=yellow>No found GameObjects with tag/script.");
            }
            else Debug.LogError("No JSON path was found!");
        }

        private void LoadFromJSON()
        {
            if (JSONpath != null)
            {
                GlobalID global = new();
                string json = File.ReadAllText(JSONpath);

                global = JsonUtility.FromJson<GlobalID>(json);

                for (int i = 0; i < global.scriptObjects.Count; i++)
                {
                    var basic = global.scriptObjects[i];
                    if (GlobalObjectId.TryParse(basic.globalId, out GlobalObjectId globalId))
                    {
                        UnityEngine.Object obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(globalId);
                        if (obj is GameObject)
                        {
                            var go = obj as GameObject;

                            findedGameObjsByScript.Add(go);

                            if (go.TryGetComponent(out MeshRenderer renderer)) renderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(global.findedMat);
                            var loadedMat = AssetDatabase.LoadAssetAtPath<Material>(basic.materialPath);
                            defaultMaterialsByScript[go] = loadedMat;
                        }
                    }
                }

                for (int i = 0; i < global.tagObjects.Count; i++)
                {
                    var basic = global.tagObjects[i];
                    if (GlobalObjectId.TryParse(basic.globalId, out GlobalObjectId globalId))
                    {
                        UnityEngine.Object obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(globalId);
                        if (obj is GameObject)
                        {
                            var go = obj as GameObject;

                            findedGameObjsByTag.Add(go);

                            if (go.TryGetComponent(out MeshRenderer renderer)) renderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(global.findedMat);
                            var loadedMat = AssetDatabase.LoadAssetAtPath<Material>(basic.materialPath);
                            defaultMaterialsByTag[go] = loadedMat;
                        }
                    }
                }

                if (findedGameObjsByScript != null || findedGameObjsByTag != null)
                {
                    if (enableExtraDebug) Debug.Log("<color=green>Data loaded from JSON successfully!");
                }
                else
                {
                    if (enableExtraDebug) Debug.Log("<color=yellow>No saved data found in JSON file.");
                }
            }
            else 
            { 
                if(saveToJson) Debug.LogError("No JSON path was found!"); 
            }
        }

        private void ClearJSON()
        {
            if (JSONpath != null && File.Exists(JSONpath))
            {
                File.WriteAllText(JSONpath, "{}");
                AssetDatabase.Refresh();
                if (enableExtraDebug) Debug.Log("<color=green>JSON cleared successfully!");
            }
            else Debug.LogError("No JSON path was found or file on this path doesn't exist!"); 
        }
        #endregion
    }
    #endregion
}
#endif