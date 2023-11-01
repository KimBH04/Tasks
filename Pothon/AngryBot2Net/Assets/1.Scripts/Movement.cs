using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;

    private Plane plane;
    private Ray ray;
    private Vector3 hitPoint;

    [SerializeField] private float moveSpeed = 10f;

    float h => Input.GetAxis("Horizontal");
    float v => Input.GetAxis("Vertical");

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        plane = new Plane(transform.up, transform.position);
    }

    private void Update()
    {
        Move();
        Turn();
    }

    private void Move()
    {
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;

        Vector3 moveDir = (camForward * v) + (camRight * h);
        moveDir.Set(moveDir.x, 0f, moveDir.z);

        controller.SimpleMove(moveDir * moveSpeed);

        float forward = Vector3.Dot(moveDir, transform.forward);
        float strafe = Vector3.Dot(moveDir, transform.right);

        animator.SetFloat("Forward", forward);
        animator.SetFloat("Strafe", strafe);
    }

    private void Turn()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float enter))
        {
            hitPoint = ray.GetPoint(enter);

            Vector3 lookDir = hitPoint - transform.position;
            lookDir.y = 0f;

            transform.localRotation = Quaternion.LookRotation(lookDir);
        }
    }
}
