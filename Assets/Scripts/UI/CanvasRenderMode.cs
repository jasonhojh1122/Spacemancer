using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasRenderMode : MonoBehaviour
    {
        private void Awake()
        {
            var canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.sortingLayerName = "UI";
            canvas.sortingOrder = 0;
            canvas.planeDistance = 10.0f;
        }
    }
}
