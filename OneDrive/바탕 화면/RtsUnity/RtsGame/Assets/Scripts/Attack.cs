using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private Unit unit; // 유닛 스크립트 참조
    private Animator animator; // 애니메이션 스크립트 참조
    private Coroutine currentAttackCoroutine; // 현재 공격 코루틴 참조
    
    private Coroutine currentAttackMoveCoroutine; // 현재 A땅 공격 코루틴 참조
    private bool isA_attack = false; // A땅 공격 모드 여부


    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<Unit>();
        animator = GetComponent<Animator>();
    }

    private void attack(Unit unit1, Unit unit2)
    {
        Debug.Log("공격시작");
        // 이전 공격 코루틴이 있다면 중지
        if (currentAttackCoroutine != null)
        {
            stopAttack();
        }

        // 새로운 공격 코루틴 시작
        currentAttackCoroutine = StartCoroutine(AttackCoroutine(unit1, unit2));
    }

    public void stopAttack()
    {
        if (currentAttackCoroutine != null)
        {
            Debug.Log("Attack coroutine stopped");
            StopCoroutine(currentAttackCoroutine);
            currentAttackCoroutine = null;
        }
        // 공격 애니메이션 종료
        animator.SetBool("isSmash", false);
    }

    IEnumerator AttackCoroutine(Unit unit1, Unit unit2)
    {
        Debug.Log("Attack Coroutine started"); 
        // 무한 루프를 사용하여 명시적으로 중지될 때까지 실행
        while (true)
        {
            // 유닛 간의 거리가 공격 사거리보다 짧으면 공격
            if (Vector3.Distance(transform.position, unit2.transform.position) < unit1.unitStats.attackRange)
            {
                unit1.isMoving = false;
                unit1.resetAnimationBool();
                animator.SetBool("isSmash", true);

                // 공격 속도에 맞게 정확한 시간 동안 대기

                yield return new WaitForSeconds(1f / unit1.unitStats.attackSpeed);
                unit2.health -= unit1.unitStats.damage; // 공격력만큼 체력 감소

            }
            // 사거리 밖이면 이동
            else
            {
                unit.isMoving = true;
                unit.moveToPos = unit2.transform.position; // 이동할 위치 설정
                animator.SetBool("isSmash", false);
                yield return null; // 다음 프레임까지 대기
            }
            if (unit2.health <= 0)
            {
                stopAttack(); // 공격 중지
                yield break; // 코루틴 종료
            }
        }
    }

    private void AttackMove(Unit unit1, Vector2 pos)
    {
        if (currentAttackMoveCoroutine != null)
        {
            stopAttack(); // 이전 A땅 공격 코루틴 중지
        }
        currentAttackMoveCoroutine = StartCoroutine(AttackMoveCoroutine(unit1, pos));
    }
    private void stopAttackMove()
    {
        if (currentAttackMoveCoroutine != null)
        {
            StopCoroutine(currentAttackMoveCoroutine);
            currentAttackMoveCoroutine = null;
        }
        animator.SetBool("isSmash", false); // 공격 애니메이션 종료
    }
    IEnumerator AttackMoveCoroutine(Unit unit1, Vector2 pos)
    {
        isA_attack = false; // A땅 공격 모드 해제
        while (true)
        {
            
            // 목적지 도착 → 이동 종료
            if (Vector3.Distance(transform.position, pos) < 0.3f)
            {
                unit1.isMoving = false;
                unit1.resetAnimationBool();
                yield break;
            }

            // 공격할 적 탐색
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 3f);
            Unit target = null;
            foreach (Collider2D hit in hits)
            {
                Unit enemy = hit.GetComponent<Unit>();
                if (enemy != null && enemy.teamID != unit1.teamID)
                {
                    target = enemy;
                    break;
                }
            }

            if (target != null) // 적 발견했으면
            {
                // 타겟이 살아있는 동안 공격/추격
                while (target != null && target.health > 0)
                {
                    float dist = Vector3.Distance(transform.position, target.transform.position);

                    if (dist < unit1.unitStats.attackRange) // 사거리 안 → 공격
                    {
                        unit1.isMoving = false;
                        unit1.resetAnimationBool();
                        animator.SetBool("isSmash", true);

                        yield return new WaitForSeconds(1f / unit1.unitStats.attackSpeed);
                        target.health -= unit1.unitStats.damage;
                    }
                    else // 사거리 밖 → 이동
                    {
                        unit1.isMoving = true;
                        unit1.moveToPos = target.transform.position;
                        animator.SetBool("isSmash", false);
                        yield return null;
                    }
                }
                // 적이 죽으면 루프 빠져나와서 다시 목적지 이동
            }

            // 적이 없으면 목적지로 이동
            unit1.isMoving = true;
            unit1.moveToPos = pos;
            animator.SetBool("isSmash", false);

            yield return null;
        }
    }






    // Update is called once per frame
    void Update()
    {
        if (unit.isSelected)
        {
            if (Input.GetMouseButtonDown(1) || (Input.GetMouseButtonDown(0) && isA_attack))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider != null && hit.collider.GetComponent<Unit>() != null) // 클릭한게 유닛인지 확인
                {
                    Unit hitUnit = hit.collider.GetComponent<Unit>();
                    if (unit != hitUnit && unit.teamID != hitUnit.teamID) // 자신이 아닌 유닛이고 팀이 다른 경우
                    {
                        // 이전 공격 코루틴 중지 및 새로운 공격 시작
                        attack(unit, hitUnit);
                    }
                    else
                    {
                        Debug.Log("자신 또는 같은 팀 유닛을 공격할 수 없습니다.");
                        // 같은 팀 유닛 클릭 시 공격 중지
                        stopAttack();
                    }
                }
                else
                {
                    stopAttackMove(); // 유닛이 아닌 곳 클릭 시 A땅 공격 중지
                    if (isA_attack)
                    {
                       AttackMove(unit, Camera.main.ScreenToWorldPoint(Input.mousePosition)); // A땅공격
                    }
                    // 유닛이 아니 고 A땅도 아니면 공격중지
                    stopAttack();
                }
                
            }
            if (Input.GetKeyDown(KeyCode.A) && !isA_attack)
            {
                isA_attack = true;

            }
            
        }

    }
}