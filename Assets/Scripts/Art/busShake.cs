using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class busShake : MonoBehaviour
{
    bool isShaking = false;
    [SerializeField] float minRange = -0.2f;
    [SerializeField] float maxRange = 0.2f;

    Vector3 realZero;

    private void Start()
    {
        realZero = transform.position;
    }

    void Update()
    {
        if (!isShaking)
            StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        isShaking = true;

        transform.position += new Vector3 (0, Random.Range(minRange, maxRange), 0);
        yield return new WaitForSeconds(Random.Range(0.01f, 0.5f));

        transform.position = realZero;
        yield return new WaitForSeconds(Random.Range(0.25f, 1f));

        transform.position -= new Vector3(0, Random.Range(minRange, maxRange), 0);
        yield return new WaitForSeconds(Random.Range(0.01f, 0.5f));

        transform.position = realZero;
        isShaking = false;
    }
}


