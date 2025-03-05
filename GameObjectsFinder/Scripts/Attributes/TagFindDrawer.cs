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
    /// Source code for the TagFind attribute.
    /// </summary>
    [CustomPropertyDrawer(typeof(TagFindAttribute))]
    public class TagFindDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
            }
        }
    }
#endif
}