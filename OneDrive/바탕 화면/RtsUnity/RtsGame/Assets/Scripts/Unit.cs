using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

// 이 클래스를 기반으로 에셋을 만들 수 있도록 메뉴에 추가
[CreateAssetMenu(fileName = "NewUnitStats", menuName = "Game Data/Unit Stats")]
public class UnitStats : ScriptableObject
{
    public float maxHealth;
    public float damage;
    public float moveSpeed;
    public float attackRange;
    public float attackSpeed; // 초당 공격 횟수
}

public class Unit : MonoBehaviour
{
    public UnitStats unitStats; // 유닛의 스탯을 저장하는 ScriptableObject
    public int teamID; // 팀 ID (0: Player, 1: Enemy 등으로 구분 가능)
    public float health;
    private Animator animator;
    public bool isSelected = false;
    public GameObject selectionHighLight;


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



    public void stop()
    {
        isMoving = false;
        worker worker = GetComponent<worker>();
        Attack attack = GetComponent<Attack>();
        // 🔧 실행 중인 코루틴 멈추기
        if (worker != null)
        {
            worker.StopHarvestCoroutine(); // worker가 있다면 채취 코루틴 중지

        }
        if (attack != null)
        {
            attack.stopAttack(); // attack이 있다면 공격 코루틴 중지
        }

        resetAnimationBool();
        animator.SetBool("isSmash", false);
    }

    private void SetDirection(string direction)
    {
        // 모든 방향 애니메이션 false로 초기화
        resetAnimationBool();

        // 해당 방향만 true로
        if (!string.IsNullOrEmpty(direction))
        {
            animator.SetBool(direction, true);
        }
    }
    public void resetAnimationBool(){
        animator.SetBool("isLeft", false);
        animator.SetBool("isDown", false);
        animator.SetBool("isBack", false);
    }

    void Start()
    {
        selectionHighLight.SetActive(false);
        health = unitStats.maxHealth; // 유닛의 체력을 설정
        animator = GetComponent<Animator>();
        
        if (GetComponent<HumanMaster>() != null)
        {
            teamID = GetComponent<HumanMaster>().teamID; // HumanMaster 컴포넌트가 있다면 teamID 설정
        }
        else
        {
            Debug.Log("기본유닛");
        }
        if (teamID == 0)
        {
            GetComponent<SpriteRenderer>().color = Color.white; // 플레이어 유닛 색상 설정
        }
        else if (teamID == 1)
        {
            GetComponent<SpriteRenderer>().color = Color.red; // 적 유닛 색상 설정
        }
    }

    void Update()
    {

        if (health <= 0)
        {
            Destroy(gameObject); // 체력이 0 이하이면 유닛 제거
        }


        if (isSelected)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                stop();
            }
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
                resetAnimationBool();
            }
            else
            {
                Vector3 direction = (moveToPos - (Vector2)transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                if (angle <= 45 && angle >= -45)
                {
                    gameObject.transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    SetDirection("isLeft");
                }
                if (angle > 45 && angle < 135)
                {
                    SetDirection("isBack");
                }
                if (angle >= 135 || angle <= -135)
                {
                    gameObject.transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    SetDirection("isLeft");
                }
                if (angle > -135 && angle < -45)
                {
                    SetDirection("isDown");
                }
                 
                transform.position += direction * unitStats.moveSpeed * Time.deltaTime;
            }
        }


    }
}
