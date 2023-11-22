using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject effect;

    public int actorNumber;

    private void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 1000f);
        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        GameObject obj = Instantiate(effect, contact.point, Quaternion.LookRotation(-contact.normal));

        Destroy(obj, 2f);
        Destroy(gameObject);
    }
}
