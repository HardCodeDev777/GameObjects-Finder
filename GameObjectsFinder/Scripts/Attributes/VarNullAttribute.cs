// Copyright (c) 2025 HardCodeDev
// This code is provided without restrictions.
// You may use, modify, and distribute it freely.
// Full license details can be found in LICENSE.txt.


namespace HardCodeDev.Attributes
{
    /// <summary>
    /// Attribute for highlighting an empty variable in the inspector with color. Takes a color. Here are some basic colors: <br/>
    /// black(0, 0, 0, 1)<br/>
    /// blue(0, 0, 1, 1)<br/>
    /// cyan(0, 1, 1, 1)<br/>
    /// gray(0.5, 0.5, 0.5, 1)<br/>
    /// green(0, 1, 0, 1)<br/>
    /// grey(0.5, 0.5, 0.5, 1)<br/>
    /// red(1, 0, 0, 1)<br/>
    /// yellow(1, 0.92, 0.016, 1)<br/>
    /// white(1, 1, 1, 1)
    /// </summary>
    public class VarNullAttribute : UnityEngine.PropertyAttribute
    {
        public int r, g, b, a;
        public VarNullAttribute(int R, int G, int B, int A)
        {
            r = R;
            g = G;
            b = B;
            a = A;
        }
    }
}