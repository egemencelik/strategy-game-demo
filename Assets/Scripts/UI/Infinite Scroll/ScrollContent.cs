using UnityEngine;

namespace UI.Infinite_Scroll
{
    public class ScrollContent : MonoBehaviour
    {
        public float ItemSpacing => itemSpacing;
        public bool Horizontal => horizontal;
        public bool Vertical => vertical;
        public float Width { get; private set; }
        public float Height { get; private set; }
        public float ChildWidth { get; private set; }
        public float ChildHeight { get; private set; }

        private RectTransform rectTransform;
        private RectTransform[] rtChildren;

        [SerializeField]
        private float itemSpacing;

        [SerializeField]
        private float horizontalMargin, verticalMargin;

        [SerializeField]
        private bool horizontal, vertical;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            rtChildren = new RectTransform[rectTransform.childCount];

            for (int i = 0; i < rectTransform.childCount; i++)
            {
                rtChildren[i] = rectTransform.GetChild(i) as RectTransform;
            }

            Width = rectTransform.rect.width - (2 * horizontalMargin);

            Height = rectTransform.rect.height - (2 * verticalMargin);

            ChildWidth = rtChildren[0].rect.width;
            ChildHeight = rtChildren[0].rect.height;

            horizontal = !vertical;
            if (vertical)
                InitializeContentVertical();
            else
                InitializeContentHorizontal();
        }

        private void InitializeContentHorizontal()
        {
            float originX = 0 - (Width * 0.5f);
            float posOffset = ChildWidth * 0.5f;
            for (int i = 0; i < rtChildren.Length; i++)
            {
                Vector2 childPos = rtChildren[i].localPosition;
                childPos.x = originX + posOffset + i * (ChildWidth + itemSpacing);
                rtChildren[i].localPosition = childPos;
            }
        }

        private void InitializeContentVertical()
        {
            float originY = 0 - (Height * 0.5f);
            float posOffset = ChildHeight * 0.5f;
            for (int i = 0; i < rtChildren.Length; i++)
            {
                Vector2 childPos = rtChildren[i].localPosition;
                childPos.y = originY + posOffset + i * (ChildHeight + itemSpacing);
                rtChildren[i].localPosition = childPos;
            }
        }
    }
}