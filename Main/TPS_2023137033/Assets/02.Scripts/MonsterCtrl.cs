using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    public State state = State.IDLE;

    public float traceDist = 10.0f;

    public float attackDist = 2.0f;

    public bool isDie= false;

    private GameObject bloodEffect;

    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent agent;
    private Animator anime;

    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");

    private readonly int hashHit = Animator.StringToHash("Hit");

    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");

    private readonly int hashSpeed = Animator.StringToHash("Speed");

    private readonly int hashDie = Animator.StringToHash("Die");

    private int hp = 100;

    void Awake()
    {
        bloodEffect = Resources.Load<GameObject>("BloodSprayEffect");

        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        anime = GetComponent<Animator>();
    }

    void Update()
    {
        if (agent.remainingDistance >= 2f)
        {
            Vector3 direction = agent.desiredVelocity;
            if (direction.sqrMagnitude >= 0.01f)
            {
                Quaternion rot = Quaternion.LookRotation(direction);
                monsterTr.rotation = Quaternion.Slerp(monsterTr.rotation, rot, Time.deltaTime * 10f);
            }
        }
    }

    void OnEnable()
    {
        PlayerCtrl.OnplayerDie += OnPlayerDie;

        StartCoroutine(CheckMonsterState());
        StartCoroutine(MonsterAction());
    }

    void OnDisable()
    {
        PlayerCtrl.OnplayerDie -= OnPlayerDie;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet") && state != State.DIE)
        {
            Destroy(collision.gameObject);
        }
    }

    public void OnDamage(Vector3 pos, Vector3 normal)
    {
        anime.SetTrigger(hashHit);
        Quaternion rot = Quaternion.LookRotation(normal);

        ShowBloodEffect(pos, rot);

        hp -= 30;
        if (hp <= 0)
        {
            state = State.DIE;
            GameManager.Instance.DisplayScore(10);
        }
    }

    void ShowBloodEffect(Vector3 pos, Quaternion rot)
    {
        GameObject blood = Instantiate(bloodEffect, pos, rot, monsterTr);
        Destroy(blood, 1f);
    }

    void OnPlayerDie()
    {
        StopAllCoroutines();

        agent.isStopped = true;
        anime.SetFloat(hashSpeed, Random.Range(.8f, 1.2f));
        anime.SetTrigger(hashPlayerDie);
    }

    IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);
            if (state == State.DIE)
                yield break;

            float distance = Vector3.Distance(playerTr.position, monsterTr.position);

            if (distance <= attackDist)
            {
                state = State.ATTACK;
            }
            else if (distance <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.IDLE;
            }
        }
    }

    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    agent.isStopped = true;

                    anime.SetBool(hashTrace, false);
                    break;

                case State.TRACE:
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;

                    anime.SetBool(hashAttack, false);
                    anime.SetBool(hashTrace, true);
                    break;

                case State.ATTACK:
                    anime.SetBool(hashAttack, true);
                    break;

                case State.DIE:
                    //Debug.Log("Monster is die");

                    isDie = true;
                    agent.isStopped = true;
                    anime.SetTrigger(hashDie);
                    GetComponent<CapsuleCollider>().enabled = false;

                    yield return new WaitForSeconds(3f);

                    hp = 100;
                    isDie = false;
                    state = State.IDLE;

                    GetComponent<CapsuleCollider>().enabled = true;
                    gameObject.SetActive(false);
                    break;
            }
            yield return new WaitForSeconds(.3f);
        }
    }

    void OnDrawGizmos()
    {
        if (state == State.TRACE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, traceDist);
        }

        if (state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDist);
        }
    }
}

public enum State
{
    IDLE,
    TRACE,
    ATTACK,
    DIE
}