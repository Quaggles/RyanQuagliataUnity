using UnityEngine.UI;

namespace RyanQuagliataUnity.Extensions {
    public static class GraphicExtensions {
        public static void SetAlpha(this Graphic that, float alpha) {
            var newColour = that.color;
            newColour.a = alpha;
            that.color = newColour;
        }
    }
}