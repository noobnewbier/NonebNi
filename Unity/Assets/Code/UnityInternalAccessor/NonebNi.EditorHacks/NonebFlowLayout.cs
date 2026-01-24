#nullable disable

using UnityEditor;
using UnityEngine;

namespace NonebNi.EditorHacks
{
    /// <summary>
    /// Copied from <see cref="FlowLayout" /> decompiled, if it works you lucked out
    /// </summary>
    internal class NonebFlowLayout : GUILayoutGroup
    {
        private LineInfo[] m_LineInfo;
        private int m_Lines;

        public override void CalcWidth()
        {
            var flag = minWidth != 0.0;
            base.CalcWidth();
            if (isVertical || flag)
                return;
            minWidth = 0.0f;
            foreach (var entry in entries)
                minWidth = Mathf.Max(m_ChildMinWidth, entry.minWidth);
        }

        public override void SetHorizontal(float x, float width)
        {
            base.SetHorizontal(x, width);
            if (resetCoords)
                x = 0.0f;
            if (isVertical)
            {
                Debug.LogError("Wordwrapped vertical group. Don't. Just Don't");
            }
            else
            {
                m_Lines = 0;
                var num = 0.0f;

                #region Logic Changed Regions

                var firstEntryX = 0f;
                var leftPaddingForIndent = 0f;
                for (var i = 0; i < entries.Count; i++)
                {
                    var entry = entries[i];
                    if (entry.rect.xMax - (double)num > x + (double)width)
                    {
                        num = entry.rect.x - entry.marginLeft;
                        ++m_Lines;
                    }

                    var childX = entry.rect.x - num;
                    switch (i)
                    {
                        case > 0:
                            if (firstEntryX > childX) leftPaddingForIndent = firstEntryX - childX;

                            break;
                        case 0:
                            firstEntryX = childX;
                            break;
                    }

                    childX += leftPaddingForIndent;

                    entry.SetHorizontal(childX, entry.rect.width);

                    entry.rect.y = m_Lines;
                }

                #endregion


                ++m_Lines;
            }
        }

        public override void CalcHeight()
        {
            if (entries.Count == 0)
            {
                maxHeight = minHeight = 0.0f;
            }
            else
            {
                m_ChildMinHeight = m_ChildMaxHeight = 0.0f;
                var num1 = 0;
                var num2 = 0;
                m_StretchableCountY = 0;
                if (!isVertical)
                {
                    m_LineInfo = new LineInfo[m_Lines];
                    for (var index = 0; index < m_Lines; ++index)
                    {
                        m_LineInfo[index].topBorder = 10000;
                        m_LineInfo[index].bottomBorder = 10000;
                    }

                    foreach (var entry in entries)
                    {
                        entry.CalcHeight();
                        var y = (int)entry.rect.y;
                        m_LineInfo[y].minSize = Mathf.Max(entry.minHeight, m_LineInfo[y].minSize);
                        m_LineInfo[y].maxSize = Mathf.Max(entry.maxHeight, m_LineInfo[y].maxSize);
                        m_LineInfo[y].topBorder = Mathf.Min(entry.marginTop, m_LineInfo[y].topBorder);
                        m_LineInfo[y].bottomBorder = Mathf.Min(entry.marginBottom, m_LineInfo[y].bottomBorder);
                    }

                    for (var index = 0; index < m_Lines; ++index)
                    {
                        m_ChildMinHeight += m_LineInfo[index].minSize;
                        m_ChildMaxHeight += m_LineInfo[index].maxSize;
                    }

                    for (var index = 1; index < m_Lines; ++index)
                    {
                        float num3 = Mathf.Max(m_LineInfo[index - 1].bottomBorder, m_LineInfo[index].topBorder);
                        m_ChildMinHeight += num3;
                        m_ChildMaxHeight += num3;
                    }

                    num1 = m_LineInfo[0].topBorder;
                    num2 = m_LineInfo[m_LineInfo.Length - 1].bottomBorder;
                }

                m_MarginTop = num1;
                m_MarginBottom = num2;
                float num4;
                var num5 = num4 = 0.0f;
                minHeight = Mathf.Max(minHeight, m_ChildMinHeight + num5 + num4);
                if (maxHeight == 0.0)
                {
                    stretchHeight += m_StretchableCountY + (style.stretchHeight ?
                        1 :
                        0);
                    maxHeight = m_ChildMaxHeight + num5 + num4;
                }
                else
                {
                    stretchHeight = 0;
                }

                maxHeight = Mathf.Max(maxHeight, minHeight);
            }
        }

        public override void SetVertical(float y, float height)
        {
            if (entries.Count == 0)
            {
                base.SetVertical(y, height);
            }
            else if (isVertical)
            {
                base.SetVertical(y, height);
            }
            else
            {
                if (resetCoords)
                    y = 0.0f;
                var num1 = y - marginTop;
                var num2 = y + marginVertical - spacing * (m_Lines - 1);
                var t = 0.0f;
                if (m_ChildMinHeight != (double)m_ChildMaxHeight)
                    t = Mathf.Clamp((float)((num2 - (double)m_ChildMinHeight) / (m_ChildMaxHeight - (double)m_ChildMinHeight)), 0.0f, 1f);
                var num3 = num1;
                for (var index = 0; index < m_Lines; ++index)
                {
                    if (index > 0)
                        num3 += Mathf.Max(m_LineInfo[index].topBorder, m_LineInfo[index - 1].bottomBorder);
                    m_LineInfo[index].start = num3;
                    m_LineInfo[index].size = Mathf.Lerp(m_LineInfo[index].minSize, m_LineInfo[index].maxSize, t);
                    num3 += m_LineInfo[index].size + spacing;
                }

                foreach (var entry in entries)
                {
                    var lineInfo = m_LineInfo[(int)entry.rect.y];
                    if (entry.stretchHeight != 0)
                        entry.SetVertical(lineInfo.start + entry.marginTop, lineInfo.size - entry.marginVertical);
                    else
                        entry.SetVertical(
                            lineInfo.start + entry.marginTop,
                            Mathf.Clamp(lineInfo.size - entry.marginVertical, entry.minHeight, entry.maxHeight)
                        );
                }
            }
        }

        private struct LineInfo
        {
            public float minSize;
            public float maxSize;
            public float start;
            public float size;
            public int topBorder;
            public int bottomBorder;
        }
    }
}