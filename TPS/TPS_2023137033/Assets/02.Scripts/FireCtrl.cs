using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePos;

    public AudioClip fireSfx;
    private new AudioSource audio;

    private MeshRenderer muzzleFlash;

    void Start()
    {
        audio = GetComponent<AudioSource>();

        muzzleFlash = firePos.GetComponentInChildren<MeshRenderer>();
        muzzleFlash.enabled = false;
    }

    void Update()
    {
        Debug.DrawRay(firePos.position, firePos.forward * 10f, Color.red);

        if (Input.GetMouseButtonDown(0))
        {
            Fire();

            if (Physics.Raycast(firePos.position, firePos.forward, out RaycastHit hit, 10f, 1 << 6))
            {
                Debug.Log($"Hit : {hit.transform.name}");
                hit.transform.GetComponent<MonsterCtrl>().OnDamage(hit.point, hit.normal);
            }
        }
    }

    void Fire()
    {
        GameObject bull = Instantiate(bullet, firePos.position, firePos.rotation);
        Destroy(bull, 5f);
        audio.PlayOneShot(fireSfx, 1.0f);
        StartCoroutine(ShowMuzzleFlash());
    }

    IEnumerator ShowMuzzleFlash()
    {
        // 오프셋 좌푯값을 랜덤 함수로 생성
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        
        // 텍스처의 오프셋 값 설정
        muzzleFlash.material.mainTextureOffset = offset;
        
        // MuzzleFlash의 회전 변경
        float angle = Random.Range(0, 360);
        muzzleFlash.transform.localRotation = Quaternion.Euler(0, 0, angle);
        
        // MuzzleFlash의 크기 조절
        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;
        
        // MuzzleFlash 활성화
        muzzleFlash.enabled = true;
        
        // 0.2초 동안 대기(정지)하는 동안 메시지 루프로 제어권을 양보
        yield return new WaitForSeconds(0.1f);
        
        // MuzzleFlash 비활성화
        muzzleFlash.enabled = false;
    }
}