using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    [SerializeField] float spawnChance = 0.15f;

    private void Start()
    {
        float rand = Random.Range(0f, 1f);

        if (rand >= spawnChance)
            gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "MinigamePlayer")
        {
            other.gameObject.GetComponent<Player>().IncrementCoins();
            Destroy(gameObject);
        }
    }
}
