// Copyright (c) 2025 HardCodeDev
// This code is licensed under the HardCodeDev License (Modified MIT).
// You may use, modify, and distribute this software under the terms of the LICENSE file.
// Selling this script as a standalone product or as part of a script collection is prohibited.
// If modified and redistributed, it must be stated that the script is based on the original work of HardCodeDev.
// Full license details can be found in LICENSE.txt.

using System.Collections.Generic;
using UnityEngine;
using HardCodeDev.Attributes;

namespace HardCodeDev.GameObjectsFinderScript
{
    public class GameObjectsFinder : MonoBehaviour
    {
        #region Variables
        [Header("Search by Tag")]
        // Enable additional debugging?
        [SerializeField, Tooltip("Enables additional Debug logs. Turn it on if you need extra debugging (recommended).")]
        private bool enableExtraDebug;

        // Tag to search objects by
        [SerializeField, TagFind, Tooltip("Tag to search objects by.")]
        private string gameObjTag;

        // Material to apply to found objects
        [SerializeField, VarNull(1, 0, 0, 1), Tooltip("Material to apply to found objects.")]
        private Material findedMaterial;

        // Found objects by tag
        [SerializeField, InteractableVar(false), Tooltip("Objects found with the specified tag.")]
        private List<GameObject> findedGameObjsByTag = new List<GameObject>();

        // Original materials when searching by tag
        private Dictionary<GameObject, Material> defaultMaterialsByTag = new Dictionary<GameObject, Material>();

        //---------------------------------------------------------------------------------------------------------------------------

        // Script name to search objects by
        [Header("Search by Script")]
        [SerializeField, Tooltip("Script name to search objects by. If script in namespace, also write namespace with script name. Example: MyNamespace.MyScript")]
        private string scriptName;

        // Found objects by script
        [SerializeField, InteractableVar(false), Tooltip("Objects found with the specified script.")]
        private List<GameObject> findedGameObjsByScript = new List<GameObject>();

        // Original materials when searching by script
        private Dictionary<GameObject, Material> defaultMaterialsByScript = new Dictionary<GameObject, Material>();
        #endregion

        #region Objects by Tag
        /// <summary>
        /// Finds all objects with the selected tag.
        /// </summary>
        [FuncButton]
        public void FindObjectsByTag()
        {
            if (findedMaterial != null && gameObjTag != null)
            {
                // Clears materials initially to avoid bugs when switching tags without clearing first
                ClearMaterialsBase(findedGameObjsByTag, defaultMaterialsByTag, true);

                // In case the tag was deleted
                try
                {
                    var gameObjs = GameObject.FindGameObjectsWithTag(gameObjTag);
                    foreach (var gameObj in gameObjs)
                    {
                        // Attempts to get the MeshRenderer component from objects
                        if (gameObj.TryGetComponent(out MeshRenderer renderer))
                        {
                            // If the dictionary <GameObject, Material> does not contain the current object
                            if (!defaultMaterialsByTag.ContainsKey(gameObj))
                            {
                                // Add the pair gameObj - renderer.sharedMaterial to the dictionary
                                defaultMaterialsByTag[gameObj] = renderer.sharedMaterial;
                                findedGameObjsByTag.Add(gameObj);
                            }
                            renderer.sharedMaterial = findedMaterial;
                        }
                        else
                        {
                            Debug.Log("<color=yellow> The object(s) with this tag do not have a MeshRenderer component.");
                        }
                    }
                }
                catch (UnityException ex)
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
            else Debug.LogError("No material assigned in Finded Material or no tag selected! Set values in the inspector.");
        }

        [FuncButton]
        public void ClearAllMaterialsFromMeshByTag() => ClearMaterialsBase(findedGameObjsByTag, defaultMaterialsByTag);
        #endregion

        #region Objects by Script
        [FuncButton]
        public void FindObjectsByScript()
        {
            if (scriptName != null)
            {
                System.Type scriptType = System.Type.GetType(scriptName);
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
                                if (!defaultMaterialsByScript.ContainsKey(gameObj))
                                {
                                    defaultMaterialsByScript[gameObj] = renderer.sharedMaterial;
                                    findedGameObjsByScript.Add(gameObj);
                                }
                                renderer.sharedMaterial = findedMaterial;
                            }
                            else
                            {
                                Debug.Log("<color=yellow> The object(s) with this script do not have a MeshRenderer component.");
                            }
                        }
                    }
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
            else Debug.LogError("No script name entered in Script Name!");
        }

        [FuncButton]
        public void ClearAllMaterialsFromMeshByScript() => ClearMaterialsBase(findedGameObjsByScript, defaultMaterialsByScript);
        #endregion

        private void ClearMaterialsBase(List<GameObject> findedGameObjs, Dictionary<GameObject, Material> defaultMaterials, bool isInFinder = false)
        {
            if (findedGameObjs.Count != 0)
            {
                foreach (var gameObj in findedGameObjs)
                {
                    if (defaultMaterials.TryGetValue(gameObj, out Material originalMat))
                    {
                        try
                        {
                            gameObj.GetComponent<MeshRenderer>().sharedMaterial = originalMat;
                        }
                        catch
                        {
                            Debug.LogError("The MeshRenderer component was removed from the object(s) during material clearing!");
                        }
                    }
                }
                findedGameObjs.Clear();
                defaultMaterials.Clear();
                if (enableExtraDebug && !isInFinder) Debug.Log("<color=green>Materials cleared successfully.");
            }
            else
            {
                if (enableExtraDebug && !isInFinder) Debug.Log("<color=yellow>No objects found for clearing.");
            }
        }
    }

}