using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class worker : MonoBehaviour
{
    private Coroutine currentHarvestCoroutine;
    private Vector3 harvestPosition; // 자원 채취 위치
    [SerializeField]
    private float harvestSpeed = 1f; // 자원 채취속도
    [SerializeField]
    private GameObject smashParticle;
    Unit unit;
    // Start is called before the first frame update
    void Start()
    {
        unit = GetComponent<Unit>();
    }


    public void harvest(string thing, Vector3 pos) // 자원 채취하는 함수
    {
        harvestPosition = pos; // 채취 위치 설정
        if (gameObject.tag == "worker" || gameObject.tag == "master")
        {
            Debug.Log(string.Format("유닛({0})이(가) {1}을(를) 채취합니다.", gameObject.tag, thing));

            // 🔧 이전 코루틴 멈추고 새 코루틴 시작
            if (currentHarvestCoroutine != null)
            {
                StopCoroutine(currentHarvestCoroutine);
                currentHarvestCoroutine = null;
            }
            currentHarvestCoroutine = StartCoroutine(HarvestCouroutine(thing));
        }
        else
        {
            Debug.Log(string.Format("이 유닛({0})은 자원을 채취할 수 없습니다.", gameObject.tag));
        }
    }
    public void StopHarvestCoroutine() // 자원 채취 코루틴 중지 함수
    {
        Debug.Log("자원 채취 코루틴 중지");
        // 🔧 이전 코루틴 종료  
        if (currentHarvestCoroutine != null)
        {
            StopCoroutine(currentHarvestCoroutine);
            currentHarvestCoroutine = null;
        }
        Animator animator = GetComponent<Animator>();
        animator.SetBool("isSmash", false);
    }

    IEnumerator HarvestCouroutine(string thing)
    {
        Debug.Log("코루틴 시작");
        Unit unit = GetComponent<Unit>();
        Animator animator = GetComponent<Animator>();
        while (true)
        {
            if (Vector3.Distance(transform.position, harvestPosition) < 1.5f)
            {

                unit.isMoving = false;
                unit.resetAnimationBool(); // 애니메이션 초기화
                animator.SetBool("isSmash", true);
                yield return new WaitForSeconds(1 / harvestSpeed); // 채취 속도에 따라 대기
                GameManager.instance.AddResource(thing); // 자원 추가 함수 호출
                Debug.Log(string.Format("{0}을(를) 채취했습니다.", thing));
                Instantiate(smashParticle, harvestPosition, Quaternion.identity); // 파티클 생성
            }
            else
            {
                unit.isMoving = true;
                unit.moveToPos = harvestPosition; // 이동할 위치 설정
                animator.SetBool("isSmash", false);
                yield return null; // 채취 위치에 도달하지 않았으면 대기
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (unit.isSelected)
        {
            if (Input.GetMouseButtonDown(1)) // 마우스 오른쪽 버튼 클릭 시
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.tag == "wood")
                    {
                        Debug.Log("나무 채취 시작");
                        harvest("wood", hit.collider.gameObject.transform.position);
                    }
                    else if (hit.collider.gameObject.tag == "stone")
                    {
                        harvest("stone", hit.collider.gameObject.transform.position);
                    }
                }
                else
                {
                    StopHarvestCoroutine(); // 채취 중지
                }
            }
        }
    }
}
