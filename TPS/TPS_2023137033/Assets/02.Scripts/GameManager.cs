using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Transform> points;

    public List<GameObject> monsterPool;
    public int maxMonsters = 10;

    public GameObject monster;
    public float createTime = 3f;

    public TMP_Text scoreText;
    private int totalScore = 0;

    private bool isGameOver;
    public bool IsGameOver
    {
        get
        {
            return isGameOver;
        }
        set
        {
            isGameOver = value;
            if (isGameOver)
            {
                CancelInvoke(nameof(CreateMonster));
            }
        }
    }

    public static GameManager Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        Application.targetFrameRate = 60;
    }

    void Start()
    {
        CreateMonsterPool();
        InvokeRepeating(nameof(CreateMonster), 2f, createTime);

        totalScore = PlayerPrefs.GetInt("TOTAL_SCORE", 0);
        DisplayScore(0);

        GameObject stageSpawnPoint = GameObject.Find("SpawnPointGroup");
        stageSpawnPoint.GetComponentsInChildren(false, points);
    }

    void CreateMonster()
    {
        int idx = Random.Range(0, points.Count);
        //Instantiate(monster, points[idx].position, points[idx].rotation);

        GameObject m = GetMonsterInPool();
        if (m != null)
        {
            m.transform.SetPositionAndRotation(points[idx].position, points[idx].rotation);
            m.SetActive(true);
        }
    }
    
    void CreateMonsterPool()
    {
        for (int i = 0; i < maxMonsters; i++)
        {
            var monster = Instantiate(this.monster);
            monster.name = $"Monster_{i:00}";
            monster.SetActive(false);
            monsterPool.Add(monster);
        }
    }

    GameObject GetMonsterInPool()
    {
        foreach (var m in monsterPool)
        {
            if (!m.activeSelf)
            {
                return m;
            }
        }
        return null;
    }

    public void DisplayScore(int score)
    {
        totalScore += score;
        scoreText.text = $"<color=#00ff00>SCORE :</color> <color=#ff0000>{totalScore:#,##0}</color>";

        PlayerPrefs.SetInt("TOTAL_SCORE", totalScore);
    }
}
