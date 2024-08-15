using UnityEngine;

namespace DevConsole.Window
{
    internal static class ParentCanvasFinder
    {
        internal static Canvas FindParentCanvas(Transform current)
        {
            Canvas canvas = current.GetComponent<Canvas>();
            
            return canvas != null
                ? canvas
                : current.parent == null
                    ? null
                    : FindParentCanvas(current.parent);
        }
    }
}