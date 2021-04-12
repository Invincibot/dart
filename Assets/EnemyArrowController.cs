using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArrowController : MonoBehaviour
{
    public GameObject target;

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        float angle = Mathf.Atan2(target.transform.position.y - transform.position.y,
            target.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        Debug.Log(angle);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}