﻿using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Noneb.UI.Editor.Animation
{
    public class AnimatorParameterPickerItem : AdvancedDropdownItem
    {
        public readonly string ParameterName;
        public readonly AnimatorControllerParameterType Type;

        public AnimatorParameterPickerItem(string parameterName, AnimatorControllerParameterType type) : base(parameterName)
        {
            ParameterName = parameterName;
            Type = type;
        }
    }
}