using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateFinalEvent : CustomStep
{
	public override IEnumerator Perform()
	{
		EventManager.ActivateEndEvent();
		yield break;
	}
}
