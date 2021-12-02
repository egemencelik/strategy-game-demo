using TMPro;
using UnityEngine;

namespace Helpers
{
    /// <summary>
    /// Static class that contains helper functions.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Creates textmeshpro in world space.
        /// </summary>
        /// <param name="text">TextMeshPro text.</param>
        /// <param name="parent">Gameobject parent.</param>
        /// <param name="localPosition">Local position of the object.</param>
        /// <param name="fontSize">Font size. Default is 40</param>
        /// <param name="color">Color of the text. Default is white.</param>
        /// <param name="width">Width of the rect transform.</param>
        /// <param name="height">Height of the rect transform.</param>
        /// <returns></returns>
        public static TextMeshPro CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), float fontSize = 40, Color? color = null,
            float width = 1.5f, float height = 1.5f)
        {
            if (color == null) color = Color.white;
            return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, width, height);
        }

        private static TextMeshPro CreateWorldText(Transform parent, string text, Vector3 localPosition, float fontSize, Color color, float width, float height)
        {
            GameObject gameObject = new GameObject("World_Text", typeof(TextMeshPro));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            TextMeshPro textMesh = gameObject.GetComponent<TextMeshPro>();
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;

            return textMesh;
        }

        /// <summary>
        /// Shows floating text in world space.
        /// </summary>
        /// <param name="txt">Text to show.</param>
        /// <param name="color">Color of the text.</param>
        /// <param name="pos">Position of object.</param>
        /// <returns></returns>
        public static FloatingText ShowFloatingText(string txt, string color, Vector3 pos)
        {
            var text = ObjectPool.Instance.SpawnFromPool(PoolType.Text).GetComponent<FloatingText>();
            text.SetText(txt, color, pos);
            return text;
        }

        /// <summary>
        /// Returns mouse position in world space.
        /// </summary>
        /// <returns>Mouse position in world space.</returns>
        public static Vector3 GetMouseWorldPosition() => GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main).SetZ(0);

        private static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
        {
            var worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }
    }
}