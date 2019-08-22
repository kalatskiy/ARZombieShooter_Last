using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    private float _speed = 25f;

    void Update()
    {
        transform.Translate(new Vector3(-1,0,0) * _speed * Time.deltaTime);
        Destroy(this.gameObject, 3f);
    }
}
