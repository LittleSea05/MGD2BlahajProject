using UnityEngine;
using TMPro;

[ExecuteAlways]
[RequireComponent(typeof(TMP_Text))]
public class CurvedText : MonoBehaviour
{
    [Header("贴合圆盘（推荐）")]
    [Tooltip("把圆盘的RectTransform拖到这里，半径会自动读取圆盘的实际大小，文字弧度会精确贴合圆盘边缘")]
    public RectTransform discReference;

    [Tooltip("在圆盘半径基础上做微调：负数=往圆心方向缩一点，正数=往外扩一点")]
    public float radiusPadding = 0f;

    [Header("手动模式（discReference留空时使用）")]
    [Tooltip("整段文字总共要弯曲多少度")]
    [Range(0f, 180f)]
    public float arcAngleDegrees = 60f;

    [Header("方向")]
    [Tooltip("勾选=文字往上弯，不勾选=往下弯")]
    public bool curveUpward = true;

    [Tooltip("整体旋转角度偏移，用来把文字对准圆盘上的某个位置")]
    public float angleOffset = 0f;

    private TMP_Text textComponent;

    void OnEnable()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    void LateUpdate()
    {
        UpdateCurve();
    }

    float GetRadius(float totalWidth)
    {
        if (discReference != null)
        {
            // 用圆盘RectTransform的实际宽度算半径，保证跟圆盘圆周精确贴合
            float discRadius = discReference.rect.width * 0.5f * discReference.lossyScale.x;
            // 换算成文字所在Canvas的局部单位（避免不同缩放导致比例不对）
            float localScale = transform.lossyScale.x;
            if (localScale > 0.0001f)
            {
                discRadius /= localScale;
            }
            return Mathf.Max(discRadius + radiusPadding, 1f);
        }

        // 没拖圆盘引用时，退回手动角度模式
        float arcAngleRad = arcAngleDegrees * Mathf.Deg2Rad;
        return arcAngleRad > 0.001f ? totalWidth / arcAngleRad : 100000f;
    }

    void UpdateCurve()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TMP_Text>();
            if (textComponent == null) return;
        }

        textComponent.ForceMeshUpdate();
        TMP_TextInfo textInfo = textComponent.textInfo;

        int characterCount = textInfo.characterCount;
        if (characterCount == 0) return;

        float totalWidth = 0f;
        for (int i = 0; i < characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;
            totalWidth += textInfo.characterInfo[i].xAdvance;
        }

        if (totalWidth <= 0f) return;

        float radius = GetRadius(totalWidth);
        float totalAngleDeg = (totalWidth / radius) * Mathf.Rad2Deg;

        Bounds bounds = textComponent.bounds;
        float boundsMinX = bounds.min.x;

        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            float charMidX = (charInfo.topLeft.x + charInfo.topRight.x) * 0.5f;
            float t = (charMidX - boundsMinX) / totalWidth;

            float angle = (t - 0.5f) * totalAngleDeg + angleOffset;
            float sign = curveUpward ? 1f : -1f;
            float radians = angle * Mathf.Deg2Rad;

            Vector3 curveOffset = new Vector3(
                Mathf.Sin(radians) * radius,
                sign * (radius - Mathf.Cos(radians) * radius) * -1f,
                0f
            );

            Quaternion rotation = Quaternion.Euler(0f, 0f, -angle * sign);

            Vector3 baseline = new Vector3(charMidX, 0f, 0f);

            for (int j = 0; j < 4; j++)
            {
                Vector3 local = vertices[vertexIndex + j] - baseline;
                local = rotation * local;
                vertices[vertexIndex + j] = local + baseline + curveOffset;
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}