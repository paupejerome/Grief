using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomHint : CustomStep
{
	[SerializeField] Dialogue[] hints;

	public override IEnumerator Perform()
	{
		yield return new WaitForSeconds(1f);
		yield return EventManager.instance.StartCoroutine(FindObjectOfType<DialogueBox>().Print( new Dialogue[1] { hints[Random.Range(0, hints.Length)] } ));
		//yield return StartCoroutine(FindObjectOfType<DialogueBox>().Print( hints[Random.Range(0, hints.Length - 1)] ));
	}
}
