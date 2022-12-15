using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "MinigamePlayer")
        other.gameObject.GetComponent<PlayerHealth>().TakeDamage();
    }
}
