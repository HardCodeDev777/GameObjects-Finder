// Copyright (c) 2025 HardCodeDev
// This code is provided without restrictions.
// You may use, modify, and distribute it freely.
// Full license details can be found in LICENSE.txt.


namespace HardCodeDev.Attributes
{
    /// <summary>
    /// Attribute for enabling/disabling interaction with a variable in the inspector. Accepts true or false
    /// and accordingly shows or hides the variable for interaction.
    /// </summary>
    public class InteractableVarAttribute : UnityEngine.PropertyAttribute
    {
        public bool isInteractable { get; }
        public InteractableVarAttribute(bool interaction) => isInteractable = interaction;
    }
}