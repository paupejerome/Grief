using System.Collections;
using UnityEngine;

public class MoveObjects : MonoBehaviour
{
    [SerializeField]
    GameObject[] faders;

    [SerializeField]
    float moveX = 0;

    [SerializeField]
    float waitBetweenLines = 1;

    [SerializeField]
    float waitEnd = 2;

    void Start()
    {
        if (faders != null)
            StartCoroutine(Fading());
    }

    IEnumerator Fading()
    {
        for (int i = 0; i < faders.Length; i++)
        {
            yield return new WaitForSeconds(waitBetweenLines);

            LeanTween.move(faders[i], faders[i].transform.position + new Vector3(moveX, 0f, 0f), 2);
        }

        yield return new WaitForSeconds(waitEnd);

        GetComponentInParent<Animator>().SetTrigger("FadeAway");

        yield return new WaitForSeconds(waitEnd);

        gameObject.SetActive(false);
    }
}
