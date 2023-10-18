#pragma warning disable IDE0051

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCtrl : MonoBehaviour
{
    private Animator anime;
    private Transform tr;
    private Vector3 moveDir;

    private PlayerInput pi;
    private InputActionMap iam;
    private InputAction movAction;
    private InputAction atkAction;

    private void Start()
    {
        anime = GetComponent<Animator>();
        tr = GetComponent<Transform>();
        pi = GetComponent<PlayerInput>();

        iam = pi.actions.FindActionMap("PlayerActions");

        movAction = iam.FindAction("Move");
        atkAction = iam.FindAction("Attack");

        movAction.performed += ctx =>
        {
            Vector2 dir = ctx.ReadValue<Vector2>();
            moveDir = new Vector3(dir.x, 0, dir.y);

            anime.SetFloat("Movement", dir.magnitude);
        };

        movAction.canceled += ctx =>
        {
            moveDir = Vector3.zero;
            anime.SetFloat("Movement", 0f);
        };

        atkAction.performed += ctx =>
        {
            anime.SetTrigger("Attack");
            Debug.Log("Attack");
        };
    }

    private void Update()
    {
        if (moveDir != Vector3.zero)
        {
            tr.rotation = Quaternion.LookRotation(moveDir);
            tr.Translate(4f * Time.deltaTime * Vector3.forward);
        }
    }

    #region SEND_MESSAGE
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
    #endregion

    #region UNITY_EVENTS
    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 dir = ctx.ReadValue<Vector2>();
        moveDir = new Vector3(dir.x, 0, dir.y);
        anime.SetFloat("Movement", dir.magnitude);
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        Debug.Log($"ctx.phase{ctx.phase}");

        if (ctx.performed)
        {
            Debug.Log("Attack");
            anime.SetTrigger("Attack");
        }
    }
    #endregion
}
