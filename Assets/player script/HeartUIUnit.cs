using UnityEngine;
using UnityEngine.UI;

public class HeartUIUnit : MonoBehaviour
{
    [Header("하트 순서 (1부터 시작)")]
    public int heartIndex = 1;

    private Image heartImage;

    private void Awake()
    {
        heartImage = GetComponent<Image>();
    }

    public void SetHeartImage(Sprite sprite)
    {
        if (heartImage != null)
        {
            heartImage.sprite = sprite;
        }
    }
}
