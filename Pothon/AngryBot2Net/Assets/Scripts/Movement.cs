using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour, IPunObservable
{
    private CharacterController controller;
    private Animator animator;

    private Plane plane;
    private Ray ray;
    private Vector3 hitPoint;

    [SerializeField] private float moveSpeed = 10f;

    float H => Input.GetAxis("Horizontal");
    float V => Input.GetAxis("Vertical");

    private PhotonView pv;
    private CinemachineVirtualCamera virtualCamera;

    private Vector3 receivePos;
    private Quaternion receiveRot;

    public float damping = 10f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        pv = GetComponent<PhotonView>();
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        if (pv.IsMine)
        {
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }

        plane = new Plane(transform.up, transform.position);
    }

    private void Update()
    {
        if (pv.IsMine)
        {
            Move();
            Turn();
        }
        else
        {
            transform.SetPositionAndRotation(
                Vector3.Lerp(transform.position, receivePos, damping * Time.deltaTime),
                Quaternion.Slerp(transform.rotation, receiveRot, damping * Time.deltaTime));
        }
    }

    private void Move()
    {
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;

        Vector3 moveDir = (camForward * V) + (camRight * H);
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            receivePos = (Vector3)stream.ReceiveNext();
            receiveRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
