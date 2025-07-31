using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Unit slectedUnit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button click
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                Unit unit = hit.collider.GetComponent<Unit>();
                if (unit != null)
                {
                    if (unit.gameObject.tag == "unit")
                    {
                        if (!unit.isSelected)
                        {
                            unit.Select();
                            slectedUnit = unit;
                        }
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape)) // Escape key to deselect unit
        {
            if (slectedUnit != null)
            {
                slectedUnit.Deselect();
                slectedUnit = null;
            }
        }
    }
}
