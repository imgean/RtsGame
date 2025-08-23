using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HpBar : MonoBehaviour
{
    private Unit unit;
    private float originalScaleX;
    private Vector3 originalPositionOffset;

    void Start()
    {
        unit = gameObject.transform.parent.GetComponentInParent<Unit>();
        originalScaleX = transform.localScale.x;
        originalPositionOffset = transform.localPosition;
    }

    void Update()
    {
        float healthRatio = unit.health  / unit.unitStats.maxHealth;
        float newScaleX = healthRatio * originalScaleX;
        transform.localScale = new Vector3(newScaleX, transform.localScale.y, 1);
        float positionOffset = (originalScaleX - newScaleX) / 2;

        transform.localPosition = new Vector3(
            originalPositionOffset.x - positionOffset,
            originalPositionOffset.y,
            originalPositionOffset.z);
    }
}