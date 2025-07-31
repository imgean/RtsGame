using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private Animator animator;
    public bool isSelected = false;
    public GameObject selectionHighLight;

    [SerializeField]
    private float moveSpeed = 5f; 
    private bool isMoving = false;
    private Vector2 moveToPos;

    public void Select ()
    {
        isSelected = true;
        selectionHighLight.SetActive(true);
        Debug.Log("Unit selected");
    }
    public void Deselect ()
    {
        isSelected = false;
        selectionHighLight.SetActive(false);
        Debug.Log("Unit deselected");
    }

    // Start is called before the first frame update
    void Start()
    {
        selectionHighLight.SetActive(false);
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelected)
        {
            if (Input.GetMouseButtonDown(1)) // Right mouse button click
            {
                moveToPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                isMoving = true;
            }
        }
        if (isMoving)
        {
            if (Vector3.Distance(transform.position, moveToPos) < 0.1f)
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
                Debug.Log("Angle: " + angle);
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
    }
}
