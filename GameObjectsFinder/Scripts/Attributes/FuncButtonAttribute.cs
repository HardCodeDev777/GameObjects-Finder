// Copyright (c) 2025 HardCodeDev
// This code is provided without restrictions.
// You may use, modify, and distribute it freely.
// Full license details can be found in LICENSE.txt.


namespace HardCodeDev.Attributes
{
    /// <summary>
    /// Attribute for buttons with methods in the inspector.
    /// Example implementation:
    /// <code>
    /// using UnityEngine;
    /// 
    /// public class ExampleClass: MonoBehaviour 
    /// {
    ///     [FuncButton]
    ///     void MyFunction()
    ///     {
    ///         // Describes actions when the button is pressed.
    ///         // In this example, "Button" will be printed to the console.
    ///         Debug.Log("Button");
    ///     }
    /// }
    /// </code>
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class FuncButtonAttribute : UnityEngine.PropertyAttribute { }
}