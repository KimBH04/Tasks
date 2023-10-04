using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;
    private Transform camTr;

    [Range(2.0f, 20.0f)]
    public float distance = 10.0f;

    [Range(0.0f, 10.0f)]
    public float height = 2.0f;
    public float damping = 10.0f;
    public float targetOffset = 2.0f;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        camTr = GetComponent<Transform>();
/*
        //커서 투명하게 하기
        Cursor.visible = false;

        //커서 고정
        Cursor.lockState = CursorLockMode.Locked;*/
    }

    private void Update()
    {
        //이스케이프(Esc) 누르면 종료
        if (Input.GetKey(KeyCode.Escape))
#if UNITY_EDITOR    //에디터면 컴파일
            //에디터 플레이 종료
            UnityEditor.EditorApplication.isPlaying = false;
#else               //에디터가 아니면 컴파일
            //애플리케이션 플레이 종료
            Application.Quit();
#endif
    }

    void LateUpdate()
    {
        Vector3 pos = targetTr.position + (-targetTr.forward * distance) + (Vector3.up * height);
        camTr.position = Vector3.SmoothDamp(camTr.position, pos, ref velocity, damping);

        camTr.LookAt(targetTr.position + (targetTr.up * targetOffset));
    }
}