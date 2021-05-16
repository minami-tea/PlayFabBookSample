using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PlayFabBook
{
    public static class ColorConst
    {
        public static readonly Color White = ColorUtility.TryParseHtmlString("#FFFFFF", out var c) ? c : default;
        public static readonly Color Orange = ColorUtility.TryParseHtmlString("#F25022", out var c) ? c : default;
        public static readonly Color Green = ColorUtility.TryParseHtmlString("#7FBA00", out var c) ? c : default;
        public static readonly Color Blue = ColorUtility.TryParseHtmlString("#00A4EF", out var c) ? c : default;
        public static readonly Color Yellow = ColorUtility.TryParseHtmlString("#FFB900", out var c) ? c : default;
    }
}
