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
        private Vector2 _scrollView;

        private bool _enableExtraDebug, _saveToJson, _clearedByScript, _clearedByTag;

        private string _gameObjTag, _scriptName, _JSONpath;

        private List<GameObject> _findedGameObjsByTag = new(), _findedGameObjsByScript = new();

        private Dictionary<GameObject, Material> _defaultMaterialsByTag = new(), _defaultMaterialsByScript = new();

        private Material _findedMaterial;
        #endregion

        #region GUI
        [MenuItem("HardCodeDev/GameObjects Finder")]
        public static void ShowWindow() => GetWindow<GameObjectsFinder>("GameObjects Finder");

        private void OnEnable()
        {
            // Values for GameObjectsFinderDemo scene.

            _saveToJson = true;

            _gameObjTag = "Player";
            _scriptName = "HardCodeDev.Examples.EmptyDemoScript";
            _JSONpath = "Assets/Data.json";
        }

        private void OnGUI()
        {
            GUILayout.BeginScrollView(_scrollView, true, false);

            GUILayout.Label("Base", EditorStyles.boldLabel);

            _enableExtraDebug = EditorGUILayout.Toggle(
                new GUIContent("Enable extra debugging", "Enables additional Debug logs. Turn it on if you need extra debugging (recommended)."),
                _enableExtraDebug);

            EditorGUILayout.Space(5);

            _saveToJson = EditorGUILayout.Toggle(
    new GUIContent("Save to JSON", "Automatically saves found GameObjects to JSON by their Global ID, their materials before finding and applied material."),
    _saveToJson);

            if (_saveToJson)
            {
                _JSONpath = EditorGUILayout.TextField(
                new GUIContent("JSON data save path ", "Example: Assets/Data.json"), _JSONpath);

                if (GUILayout.Button(
                    new GUIContent("Load JSON data")))
                    LoadFromJSON();
                if (GUILayout.Button("Clear JSON file")) ClearJSON();
            }

            EditorGUILayout.Space(5);

            _findedMaterial = (Material)EditorGUILayout.ObjectField(
                new GUIContent("Applied material", "Material to apply to found objects."),
                _findedMaterial, typeof(Material), true);

            EditorGUILayout.Space(10);

            GUILayout.Label("Search by Tag", EditorStyles.boldLabel);
            _gameObjTag = EditorGUILayout.TagField(
                new GUIContent("GameObjects tag", "Tag to search objects by"),
                _gameObjTag);

            EditorGUILayout.Space(10);

            GUILayout.Label("Search by Script", EditorStyles.boldLabel);
            _scriptName = EditorGUILayout.TextField(
                new GUIContent("GameObjects full script name", "Full name of the script (with namespace if needed). Example: MyNamespace.MyScript"),
                _scriptName);

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

            GUILayout.EndScrollView();
        }

        private void DrawList(TypeOfObjects type)
        {
            List<GameObject> finded = new();

            finded = type == TypeOfObjects.Tag ? _findedGameObjsByTag: _findedGameObjsByScript;
            var cleared = type == TypeOfObjects.Tag ? _clearedByTag : _clearedByScript;

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
                if (_gameObjTag != null)
                {
                    ClearMaterials(TypeOfObjects.Tag, true);
                    _clearedByTag = false;

                    try
                    {
                        var gameObjs = GameObject.FindGameObjectsWithTag(_gameObjTag);
                        foreach (var gameObj in gameObjs)
                        {
                            if (gameObj.TryGetComponent(out MeshRenderer renderer))
                            {
                                if (!_defaultMaterialsByTag.ContainsKey(gameObj)) _defaultMaterialsByTag[gameObj] = renderer.sharedMaterial;
                                renderer.sharedMaterial = _findedMaterial;
                            }
                            _findedGameObjsByTag.Add(gameObj);
                        }
                        if (_saveToJson) SaveToJSON();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"The tag does not exist! It may have been deleted. Error message: {ex.Message}");
                    }

                    if (_findedGameObjsByTag.Count > 0)
                    {
                        if (_enableExtraDebug) Debug.Log($"<color=green>{_findedGameObjsByTag.Count} objects found!");
                    }
                    else
                    {
                        if (_enableExtraDebug) Debug.Log("<color=yellow>No objects found!");
                    }
                }
                else Debug.LogError("No tag entered in GameObjects tag!"); 
            }

            if (type == TypeOfObjects.Script)
            {
                if (_scriptName != null)
                {
                    ClearMaterials(TypeOfObjects.Script, true);
                    _clearedByScript = false;

                    Type scriptType = Type.GetType(_scriptName);
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
                                    if (!_defaultMaterialsByScript.ContainsKey(gameObj)) _defaultMaterialsByScript[gameObj] = renderer.sharedMaterial;
                                    renderer.sharedMaterial = _findedMaterial;
                                }
                                _findedGameObjsByScript.Add(gameObj);
                            }
                        }
                        if(_saveToJson)SaveToJSON();
                    }
                    else
                    {
                        if (_enableExtraDebug) Debug.Log("<color=yellow>No objects found with this script.");
                    }

                    if (_findedGameObjsByScript.Count > 0)
                    {
                        if (_enableExtraDebug) Debug.Log($"<color=green>{_findedGameObjsByScript.Count} objects found!");
                    }

                    else
                    {
                        if (_enableExtraDebug) Debug.Log("<color=yellow>No objects found with this script.");
                    }
                }
                else Debug.LogError("No script name entered in GameObjects script name!"); 
            }
        }

        private void ClearMaterials(TypeOfObjects type, bool isInFinder = false)
        {
            _clearedByScript = type == TypeOfObjects.Tag ? false : true;
            _clearedByTag = type == TypeOfObjects.Tag ? true : false;

            List<GameObject> findedGameObjs = new();
            Dictionary<GameObject, Material> defaultMaterials = new();

            findedGameObjs = type == TypeOfObjects.Tag ? _findedGameObjsByTag : _findedGameObjsByScript;
            defaultMaterials = type == TypeOfObjects.Tag ? _defaultMaterialsByTag : _defaultMaterialsByScript;

            if (findedGameObjs.Count != 0)
            {
                foreach (var gameObj in findedGameObjs)
                {
                    if (defaultMaterials.TryGetValue(gameObj, out Material originalMat))
                    {
                        try
                        {
                            gameObj.GetComponent<MeshRenderer>().sharedMaterial = originalMat;
                            if (_enableExtraDebug && !isInFinder) Debug.Log("<color=green>Materials cleared successfully!");
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
                if (_enableExtraDebug && !isInFinder) Debug.Log("<color=yellow>No objects found for clearing.");
            }
        }
        #endregion

        #region JSON
        private void SaveToJSON() 
        {
            if (_JSONpath != null)
            {
                GlobalID global = new();

                GlobalID globalExisted = new();
                var jsonExisted = File.ReadAllText(_JSONpath);
                globalExisted = JsonUtility.FromJson<GlobalID>(jsonExisted);

                if (_findedGameObjsByScript != null)
                {
                    for (int i = 0; i < _findedGameObjsByScript.Count; i++)
                    {
                        GlobalIdBasic basic = new();    
                        var globalId = GlobalObjectId.GetGlobalObjectIdSlow(_findedGameObjsByScript[i]).ToString();
                        basic.globalId = globalId;


                        if (_findedGameObjsByScript[i].TryGetComponent(out MeshRenderer renderer)) 
                        {
                            var mat = _defaultMaterialsByScript[_findedGameObjsByScript[i]];
                            string path = AssetDatabase.GetAssetPath(mat);
                            basic.materialPath = path;
                        }

                        global.scriptObjects.Add(basic);
                    }
                    
                    global.findedMat = AssetDatabase.GetAssetPath(_findedMaterial);

                    if (_enableExtraDebug) Debug.Log("<color=green>Found by scrpt objects data saved to JSON successfully!");
                }

                if (_findedGameObjsByTag != null)
                {
                    for (int i = 0; i < _findedGameObjsByTag.Count; i++)
                    {
                        GlobalIdBasic basic = new();
                        var globalId = GlobalObjectId.GetGlobalObjectIdSlow(_findedGameObjsByTag[i]).ToString();
                        basic.globalId = globalId;


                        if (_findedGameObjsByTag[i].TryGetComponent(out MeshRenderer renderer))
                        {
                            var mat = _defaultMaterialsByTag[_findedGameObjsByTag[i]];
                            string path = AssetDatabase.GetAssetPath(mat);
                            basic.materialPath = path;
                        }

                        global.tagObjects.Add(basic);
                    }

                    global.findedMat = AssetDatabase.GetAssetPath(_findedMaterial);

                    if (_enableExtraDebug) Debug.Log("<color=green>Found by tag objects data saved to JSON successfully!");
                }

                var updatedJson = JsonUtility.ToJson(global, true);

                File.WriteAllText(_JSONpath, updatedJson);
                AssetDatabase.Refresh();
                if (_enableExtraDebug) Debug.Log("<color=yellow>No found GameObjects with tag/script.");
            }
            else Debug.LogError("No JSON path was found!");
        }

        private void LoadFromJSON()
        {
            if (_JSONpath != null)
            {
                GlobalID global = new();
                string json = File.ReadAllText(_JSONpath);

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

                            _findedGameObjsByScript.Add(go);

                            if (go.TryGetComponent(out MeshRenderer renderer)) renderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(global.findedMat);
                            var loadedMat = AssetDatabase.LoadAssetAtPath<Material>(basic.materialPath);
                            _defaultMaterialsByScript[go] = loadedMat;
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

                            _findedGameObjsByTag.Add(go);

                            if (go.TryGetComponent(out MeshRenderer renderer)) renderer.sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(global.findedMat);
                            var loadedMat = AssetDatabase.LoadAssetAtPath<Material>(basic.materialPath);
                            _defaultMaterialsByTag[go] = loadedMat;
                        }
                    }
                }

                if (_findedGameObjsByScript != null || _findedGameObjsByTag != null)
                {
                    if (_enableExtraDebug) Debug.Log("<color=green>Data loaded from JSON successfully!");
                }
                else
                {
                    if (_enableExtraDebug) Debug.Log("<color=yellow>No saved data found in JSON file.");
                }
            }
            else 
            { 
                if(_saveToJson) Debug.LogError("No JSON path was found!"); 
            }
        }

        private void ClearJSON()
        {
            if (_JSONpath != null && File.Exists(_JSONpath))
            {
                File.WriteAllText(_JSONpath, "{}");
                AssetDatabase.Refresh();
                if (_enableExtraDebug) Debug.Log("<color=green>JSON cleared successfully!");
            }
            else Debug.LogError("No JSON path was found or file on this path doesn't exist!"); 
        }
        #endregion
    }
    #endregion
}
#endif