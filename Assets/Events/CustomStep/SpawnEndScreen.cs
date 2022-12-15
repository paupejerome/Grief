using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnEndScreen : CustomStep
{
    [SerializeField]
    GameObject EndScreen;

    private GameObject clone;


    private Color zero = new Color(1, 1, 1, 0);
    private Color one = new Color(1, 1, 1, 1);

    private float fractionTime = 0;

    public override IEnumerator Perform()
    {
        GameManager.GetInstance().HideTaskbar();
        clone = Instantiate(EndScreen);

        while (clone.GetComponent<SpriteRenderer>().color.a < 1)
        {
            clone.GetComponent<SpriteRenderer>().color = Color.Lerp(zero, one, fractionTime);

            fractionTime += 0.002f;

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(5);

        SceneManager.LoadScene(0);
    }
}