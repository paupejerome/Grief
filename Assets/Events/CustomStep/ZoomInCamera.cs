using System.Collections;
using UnityEngine;

public class ZoomInCamera : CustomStep
{
    private float fractionTime = 0;

    public override IEnumerator Perform()
    {
        while (FindObjectOfType<Camera>().fieldOfView < 40)
        {
            FindObjectOfType<Camera>().fieldOfView = Mathf.Lerp(1, 40, fractionTime);

            fractionTime += 0.002f;

            yield return new WaitForEndOfFrame();
        }
    }
}
