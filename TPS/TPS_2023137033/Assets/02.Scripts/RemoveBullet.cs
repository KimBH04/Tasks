using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    public GameObject sparkEffect;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            // 첫 번째 충돌 지점의 정보 추출
            ContactPoint cp = collision.GetContact(0);

            // 충돌한 총알의 법선 벡터를 쿼터니언 타입으로 변환
            Quaternion rot = Quaternion.LookRotation(-cp.normal);

            GameObject spark = Instantiate(sparkEffect, cp.point, rot);

            Destroy(spark, .5f);
            Destroy(collision.gameObject);
        }
    }
}
