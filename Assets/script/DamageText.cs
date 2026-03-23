using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class DamageText : MonoBehaviour
{
    public TMP_Text textMesh;

    private Transform target;
    private Vector3 offset;
    public float duration = 1f;
    private float elapsed = 0f;

    private Vector3 moveDirection = new Vector3(1f, 1f, 0f); // 포물선 방향
    public float moveSpeed = 1.5f;
    public float arcStrength = 2f;

    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    /// <summary>
    /// 데미지 텍스트 초기화
    /// </summary>
    public void Setup(Transform target, float damage, Vector3 offset)
    {
        this.target = target;
        this.offset = offset;
        this.elapsed = 0f;

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        textMesh.text = damage.ToString("F0");
        canvasGroup.alpha = 1f;

        UpdatePosition(0f);
    }

    private void Update()
    {
        if (target == null || canvas == null || rectTransform == null) return;

        elapsed += Time.deltaTime;
        float t = elapsed / duration;

        UpdatePosition(t);

        canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

        if (elapsed >= duration)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 포물선 형태로 이동하며 위치 갱신
    /// </summary>
    private void UpdatePosition(float t)
    {
        Vector3 worldPos = GetWorldPosition() + offset;

        Vector3 curveOffset = moveDirection * moveSpeed * t;
        curveOffset.y += Mathf.Sin(t * Mathf.PI) * arcStrength;
        worldPos += curveOffset;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            null, // Overlay 모드일 땐 카메라 null
            out Vector2 localPoint
        );

        rectTransform.localPosition = localPoint;
    }

    /// <summary>
    /// 대상 오브젝트의 월드 기준 머리 위치 반환
    /// </summary>
    private Vector3 GetWorldPosition()
    {
        // 기준 오브젝트에 Renderer가 있다면 머리 위에 표시
        Renderer renderer = target.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Bounds bounds = renderer.bounds;
            return bounds.center + Vector3.up * bounds.extents.y;
        }

        // Renderer 없으면 단순 Transform 기준
        return target.position + Vector3.up * 2f;
    }
}
