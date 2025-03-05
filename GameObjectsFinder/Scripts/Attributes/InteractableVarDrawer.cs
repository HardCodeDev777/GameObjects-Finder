// Copyright (c) 2025 HardCodeDev
// This code is provided without restrictions.
// You may use, modify, and distribute it freely.
// Full license details can be found in LICENSE.txt.


#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace HardCodeDev.Attributes
{
    /// <summary>
    /// Source code for the InteractableVar attribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(InteractableVarAttribute))]
    public class InteractableVarDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InteractableVarAttribute canInteract = (InteractableVarAttribute)attribute;
            if (canInteract.isInteractable)
            {
                GUI.enabled = true;
                EditorGUI.PropertyField(position, property, label);
            }
            else
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
#endif
}