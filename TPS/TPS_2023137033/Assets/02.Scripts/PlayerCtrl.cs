using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnplayerDie;

    public float currHp;
    public float moveSpeed = 10f;
    public float turnSpeed = 750f;
    private Transform tr;
    //private Animation anime;

    private Animator ani;
    private readonly int hashX = Animator.StringToHash("X");
    private readonly int hashY = Animator.StringToHash("Y");

    private readonly float initHp = 100f;

    private Image hpBar;

    IEnumerator Start()
    {
        tr = GetComponent<Transform>();
        //anime = GetComponent<Animation>();
        ani = GetComponent<Animator>();

        hpBar = GameObject.FindGameObjectWithTag("HP_Bar").GetComponent<Image>();
        DisplayHealth();

        //anime.Play("Idle");

        float temp = turnSpeed;
        turnSpeed = 0f;
        yield return new WaitForSeconds(.3f);
        turnSpeed = temp;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float r = Input.GetAxis("Mouse X");

        //Debug.Log("h = " + h);
        //Debug.Log("v = " + v);

        float normal = MathF.Sqrt(MathF.Abs(h) + MathF.Abs(v));
        normal /= normal * normal;

        if (float.IsNaN(normal))
        {
            normal = 0f;
        }

        Vector3 moveDir = (normal * v * Vector3.forward) + (normal * h * Vector3.right);
        tr.Translate(moveSpeed * Time.deltaTime * moveDir);

        tr.Rotate(r * Time.deltaTime * turnSpeed * Vector3.up);

        PlayerAnime(h, v);
    }

    void OnTriggerEnter(Collider other)
    {
        if (currHp >= 0f && other.CompareTag("Punch"))
        {
            currHp -= 1f;
            //Debug.Log($"Player hp = {currHp / initHp}");
            DisplayHealth();

            if (currHp <= 0f)
            {
                PlayerDie();
            }
        }
    }

    void DisplayHealth()
    {
        hpBar.fillAmount = currHp / initHp;
    }

    void PlayerDie()
    {
        Debug.Log("Player die!");

        /*GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsters)
        {
            monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        }*/
        OnplayerDie();

        GameManager.Instance.IsGameOver = true;
    }

    void PlayerAnime(float h, float v)
    {
        ani.SetFloat(hashX, h);
        ani.SetFloat(hashY, v);

        /*
        if (v >= 0.1f)
        {
            anime.CrossFade("RunF", 0.25f);
        }
        else if (v <= -0.1f)
        {
            anime.CrossFade("RunB", 0.25f);
        }
        else if (h >= 0.1f)
        {
            anime.CrossFade("RunR", 0.25f);
        }
        else if (h <= -0.1f)
        {
            anime.CrossFade("RunL", 0.25f);
        }
        else
        {
            anime.CrossFade("Idle", 0.25f);
        }*/
    }
}
