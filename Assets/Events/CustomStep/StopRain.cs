using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopRain : CustomStep
{
    public override IEnumerator Perform()
    {
        GameState.ToggleRain(false);
        yield return null;
        //throw new System.NotImplementedException();
    }

}
