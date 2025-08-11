using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private Animator animator;
    public bool isSelected = false;
    public GameObject selectionHighLight;

    [SerializeField]
    private float moveSpeed = 5f; // 이동속도

    [SerializeField]
    public bool isMoving = false;
    public Vector2 moveToPos;
    [SerializeField]
    private bool contain; // 유닛이 선택된 상태인지 확인하는 변수

    public void Select()    // 유닛 선택하는 함수
    {
        isSelected = true;
        selectionHighLight.SetActive(true);
        Debug.Log("Unit selected");
    }

    public void Deselect()    // 유닛 선택취소하는 함수
    {
        isSelected = false;
        selectionHighLight.SetActive(false);
        Debug.Log("Unit deselected");
        contain = false; // 선택 취소 시 contain 초기화
    }



    private void stop()
    {
        isMoving = false;
        worker worker = GetComponent<worker>();
        // 🔧 실행 중인 코루틴 멈추기
        if (worker != null)
        {
            worker.StopHarvestCoroutine(); // worker가 있다면 채취 코루틴 중지

        }

        animator.SetBool("isLeft", false);
        animator.SetBool("isDown", false);
        animator.SetBool("isBack", false);
        animator.SetBool("isSmash", false);
    }



    void Start()
    {
        selectionHighLight.SetActive(false);
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isSelected)
        {
            if (Input.GetMouseButtonDown(1)) // 오른쪽 마우스 클릭
            {
                moveToPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                isMoving = true;
            }
            foreach (Unit unit in GameManager.instance.slectedUnit)
            {
                if (this == unit)
                {
                    contain = true;
                    break;
                }
            }
            if (!contain)
            {
                Deselect(); // 선택된 유닛이 아니면 선택 취소
                contain = false; // contain 초기화
            }

        }

        if (isMoving)
        {
            if (Vector3.Distance(transform.position, moveToPos) < 0.3f)
            {
                isMoving = false;
                animator.SetBool("isLeft", false);
                animator.SetBool("isDown", false);
                animator.SetBool("isBack", false);
            }
            else
            {
                Vector3 direction = (moveToPos - (Vector2)transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                if (angle <= 45 && angle >= -45)
                {
                    gameObject.transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    animator.SetBool("isLeft", true);
                    animator.SetBool("isDown", false);
                    animator.SetBool("isBack", false);
                }
                if (angle > 45 && angle < 135)
                {
                    animator.SetBool("isLeft", false);
                    animator.SetBool("isDown", false);
                    animator.SetBool("isBack", true);
                }
                if (angle >= 135 || angle <= -135)
                {
                    gameObject.transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    animator.SetBool("isLeft", true);
                    animator.SetBool("isDown", false);
                    animator.SetBool("isBack", false);
                }
                if (angle > -135 && angle < -45)
                {
                    animator.SetBool("isLeft", false);
                    animator.SetBool("isDown", true);
                    animator.SetBool("isBack", false);
                }

                transform.position += direction * moveSpeed * Time.deltaTime;
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            stop();
        }
    }
}
