using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    private Renderer[] renderers;

    private int initHP = 100;
    public int currHP = 100;

    private Animator animator;
    private CharacterController cc;

    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashRespawn = Animator.StringToHash("Respawn");

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();

        currHP = initHP;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currHP > 0 && collision.collider.CompareTag("BULLET"))
        {
            currHP -= 20;
            if (currHP <= 0)
            {
                StartCoroutine(PlayerDie());
            }
        }
    }

    private IEnumerator PlayerDie()
    {
        cc.enabled = false;
        animator.SetBool(hashRespawn, false);
        animator.SetTrigger(hashDie);

        yield return new WaitForSeconds(3f);

        animator.SetBool(hashRespawn, true);

        SetPlayerVisible(false);

        yield return new WaitForSeconds(1.5f);

        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(0, points.Length);
        transform.position = points[idx].position;

        currHP = 100;
        SetPlayerVisible(true);
        cc.enabled = true;

        void SetPlayerVisible(bool isVisible)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = isVisible;
            }
        }
    }
}
