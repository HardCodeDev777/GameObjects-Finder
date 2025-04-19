// This script is for demonstration purposes only and is not part of the utility itself.
// You are free to use, modify, and distribute this code without restrictions.
// Full license details can be found in LICENSE.txt.

#if UNITY_EDITOR
namespace HardCodeDev.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class FuncButtonAttribute : UnityEngine.PropertyAttribute { }
}
#endif