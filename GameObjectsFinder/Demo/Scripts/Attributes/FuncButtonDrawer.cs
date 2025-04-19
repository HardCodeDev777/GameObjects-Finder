// This script is for demonstration purposes only and is not part of the utility itself.
// You are free to use, modify, and distribute this code without restrictions.
// Full license details can be found in LICENSE.txt.

using System.Reflection;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

namespace HardCodeDev.Attributes
{
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