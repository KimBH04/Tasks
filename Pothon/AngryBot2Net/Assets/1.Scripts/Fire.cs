using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public Transform firePos;
    public GameObject bulletPrefab;
    public ParticleSystem muzzleFlash;

    private PhotonView pv;
    private bool IsMouseClick => Input.GetMouseButtonDown(0);

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        muzzleFlash = firePos.Find("MuzzleFlash").GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (pv.IsMine && IsMouseClick)
        {
            FireBullet();
            pv.RPC("FireBullet", RpcTarget.Others, null);
        }
    }

    [PunRPC]
    private void FireBullet()
    {
        if (!muzzleFlash.isPlaying)
        {
            muzzleFlash.Play(true);
        }

        GameObject bullet = Instantiate(bulletPrefab, firePos.position, firePos.rotation);
    }
}
