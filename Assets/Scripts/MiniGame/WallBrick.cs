using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBrick : MonoBehaviour
{
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public void DestroyWall()
    {
        rb.useGravity = true;
        int randX = Random.Range(150, 400);
        int randY = Random.Range(0, 200);
        int randZ = Random.Range(-200, 200);

        rb.AddForce(new Vector3(randX, randY, randZ));
    }
}
