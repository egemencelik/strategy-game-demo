using TMPro;
using UnityEngine;

namespace Helpers
{
    public class FloatingText : MonoBehaviour
    {
        private Vector3 initialOffset, finalOffset;
        public float fadeDuration = 1;
        private TextMeshPro text;
        private float fadeStartTime;
        private bool textSet;

        private void Awake()
        {
            fadeStartTime = Time.time;
            text = GetComponent<TextMeshPro>();
        }

        private void Update()
        {
            if (!textSet) return;
            
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            var progress = (Time.time - fadeStartTime) / fadeDuration;
            if (progress <= 1)
            {
                transform.localPosition = Vector3.Lerp(initialOffset, finalOffset, progress);
                text.alpha = 1 - progress;
            }
            else
            {
                textSet = false;
                gameObject.SetActive(false);
            }
        }

        public void SetText(string txt, string color, Vector3 pos)
        {
            fadeStartTime = Time.time;
            initialOffset = pos;
            finalOffset = initialOffset + new Vector3(0, 5, 0);
            textSet = true;

            text.text = $"<color={color}>{txt}";
        }
    }
}