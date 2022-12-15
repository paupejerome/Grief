using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngryBoss : CustomStep
{
    [SerializeField] GameObject Boss;

    public override IEnumerator Perform()
    {
        GameObject gameObject = Instantiate<GameObject>(Boss, Vector3.zero, Quaternion.identity);
        yield return new WaitForEndOfFrame();

        
    }
}
