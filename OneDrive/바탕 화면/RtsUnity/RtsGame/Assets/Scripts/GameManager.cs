using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Unit[] slectedUnit;
    public bool isCameraMoving;

    [SerializeField] private float cameraMoveSpeed = 5f;

    // 자원
    public int wood = 0;
    private int stone = 0;

    // 드래그 선택 관련
    private Vector2 dragStartPos;
    private Vector2 dragEndPos;
    private bool isDragging = false;
    private float dragThreshold = 0.1f; // 드래그와 클릭 구분 거리

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        slectedUnit = new Unit[10];
    }

    public void AddResource(string thing)
    {
        if (thing == "wood")
        {
            wood++;
            Debug.Log("현재 나무: " + wood);
        }
        else if (thing == "stone")
        {
            stone++;
            Debug.Log("현재 돌: " + stone);
        }
        else
        {
            Debug.LogWarning("알 수 없는 물질: " + thing);
        }
    }

    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // --- 드래그/클릭 시작 ---
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = mouseWorldPos;
            isDragging = true;
        }

        // --- 드래그 중 ---
        if (Input.GetMouseButton(0) && isDragging)
        {
            dragEndPos = mouseWorldPos;
        }

        // --- 드래그/클릭 끝 ---
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;

            if (Vector2.Distance(dragStartPos, mouseWorldPos) < dragThreshold)
            {
                // 클릭 선택
                ClickSelect(mouseWorldPos);
            }
            else
            {
                // 드래그 선택
                DragSelect();
            }
        }

        // ESC로 선택 해제
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectAll();
        }

        HandleCameraMovement(mousePosition, mouseWorldPos);
    }

    void HandleCameraMovement(Vector2 mousePosition, Vector2 mouseWorldPos)
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        if (mousePosition.x <= 0 || mousePosition.x >= screenWidth ||
            mousePosition.y <= 0 || mousePosition.y >= screenHeight)
        {
            isCameraMoving = true;
        }
        else
        {
            isCameraMoving = false;
        }

        if (isCameraMoving)
        {
            Vector3 cameraPosition = Camera.main.transform.position;
            Vector2 direction = (mouseWorldPos - (Vector2)cameraPosition).normalized;

            cameraPosition.x += direction.x * Time.deltaTime * cameraMoveSpeed;
            cameraPosition.y += direction.y * Time.deltaTime * cameraMoveSpeed;

            Camera.main.transform.position = cameraPosition;
        }
    }

    void ClickSelect(Vector2 mouseWorldPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
        if (hit.collider != null)
        {
            Unit unit = hit.collider.GetComponent<Unit>();
            if (unit != null)
            {
                DeselectAll(); // 기존 선택 해제
                unit.Select();
                AddToSelectedUnits(unit);
            }
        }
        else
        {
            // 빈 곳 클릭 시 선택 해제
            DeselectAll();
        }
    }

    void DragSelect()
    {
        Vector2 min = Vector2.Min(dragStartPos, dragEndPos);
        Vector2 max = Vector2.Max(dragStartPos, dragEndPos);

        DeselectAll();

        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            Vector2 pos = unit.transform.position;
            if (pos.x >= min.x && pos.x <= max.x && pos.y >= min.y && pos.y <= max.y)
            {
                unit.Select();
                AddToSelectedUnits(unit);
            }
        }
    }

    void AddToSelectedUnits(Unit unit)
    {
        for (int i = 0; i < slectedUnit.Length; i++)
        {
            if (slectedUnit[i] == null)
            {
                slectedUnit[i] = unit;
                break;
            }
        }
    }

    void DeselectAll()
    {
        for (int i = 0; i < slectedUnit.Length; i++)
        {
            if (slectedUnit[i] != null)
            {
                slectedUnit[i].Deselect();
                slectedUnit[i] = null;
            }
        }
    }

    void OnGUI()
    {
        if (isDragging && Vector2.Distance(dragStartPos, dragEndPos) > dragThreshold)
        {
            Vector3 startScreen = Camera.main.WorldToScreenPoint(dragStartPos);
            Vector3 endScreen = Camera.main.WorldToScreenPoint(dragEndPos);

            startScreen.y = Screen.height - startScreen.y;
            endScreen.y = Screen.height - endScreen.y;

            Rect rect = new Rect(
                Mathf.Min(startScreen.x, endScreen.x),
                Mathf.Min(startScreen.y, endScreen.y),
                Mathf.Abs(startScreen.x - endScreen.x),
                Mathf.Abs(startScreen.y - endScreen.y)
            );

            GUI.color = new Color(0, 1, 0, 0.2f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = Color.green;
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 1), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.yMax, rect.width, 1), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, 1, rect.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax, rect.y, 1, rect.height), Texture2D.whiteTexture);
        }
    }
}
