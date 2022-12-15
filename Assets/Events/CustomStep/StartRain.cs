using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRain : CustomStep
{
    public override IEnumerator Perform()
    {
        GameState.ToggleRain(true);      
        yield return null;
        //throw new System.NotImplementedException();
    }

}
