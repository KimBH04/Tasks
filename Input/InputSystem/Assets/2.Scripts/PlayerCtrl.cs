#pragma warning disable IDE0051

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCtrl : MonoBehaviour
{
    private Animator anime;
    private Transform tr;
    private Vector3 moveDir;

    private void Start()
    {
        anime = GetComponent<Animator>();
        tr = GetComponent<Transform>();
    }

    private void Update()
    {
        if (moveDir != Vector3.zero)
        {
            tr.rotation = Quaternion.LookRotation(moveDir);
            tr.Translate(4f * Time.deltaTime * Vector3.forward);
        }
    }

    private void OnMove(InputValue value)
    {
        Vector2 dir = value.Get<Vector2>();

        moveDir = new Vector3(dir.x, 0, dir.y);

        anime.SetFloat("Movement", dir.magnitude);
        Debug.Log($"Move = ({dir.x}, {dir.y})");
    }

    private void OnAttack()
    {
        anime.SetTrigger("Attack");
        Debug.Log("Attack");
    }
}
