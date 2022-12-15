using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    const float livingTime = 2f;
    [SerializeField] GameObject[] listBrick;

    private void OnTriggerEnter(Collider other)
    {
        foreach (GameObject brick in listBrick)
        {
            brick.GetComponent<WallBrick>().DestroyWall();
        }

        GetComponent<BoxCollider>().enabled = false;
        Destroy(gameObject, livingTime);
    }
}
