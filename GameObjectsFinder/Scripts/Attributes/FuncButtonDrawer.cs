// Copyright (c) 2025 HardCodeDev
// This code is provided without restrictions.
// You may use, modify, and distribute it freely.
// Full license details can be found in LICENSE.txt.


#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HardCodeDev.Attributes
{
    /// <summary>
    /// Source code for the FuncButton attribute.
    /// </summary>
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class FuncButtonDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            MonoBehaviour script = (MonoBehaviour)target;
            MethodInfo[] methodInfos = script.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var method in methodInfos)
            {
                if (System.Attribute.IsDefined(method, typeof(FuncButtonAttribute)))
                {
                    if (GUILayout.Button(method.Name))
                    {
                        method.Invoke(script, null);
                    }
                }
            }
        }
    }
#endif
}