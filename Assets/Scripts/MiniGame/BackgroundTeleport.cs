using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTeleport : MonoBehaviour
{
    const float offset = 36f;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "MinigamePlayer")
        {
            transform.position = new Vector3(transform.position.x + offset, 0f, 0f);
        }
    }
}
