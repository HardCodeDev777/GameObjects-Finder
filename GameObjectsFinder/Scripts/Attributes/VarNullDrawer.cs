// Copyright (c) 2025 HardCodeDev
// This code is provided without restrictions.
// You may use, modify, and distribute it freely.
// Full license details can be found in LICENSE.txt.


#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace HardCodeDev.Attributes
{
    /// <summary>
    /// Source code for the VarNull attribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(VarNullAttribute))]
    public class VarNullDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            VarNullAttribute varNull = (VarNullAttribute)attribute;
            if (property.objectReferenceValue == null) GUI.backgroundColor = new Color(varNull.r, varNull.g, varNull.b, varNull.a);
            EditorGUI.PropertyField(position, property, label);
            GUI.backgroundColor = Color.white;
        }
    }
#endif
}