using UnityEngine;

public class DepthSorter : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        // Y값이 작을수록 앞에 오도록 음수 곱해줌
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
}
