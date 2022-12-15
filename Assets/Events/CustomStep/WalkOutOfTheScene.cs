using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkOutOfTheScene : CustomStep
{
    GameObject eventSubject;
    Vector3 startPosition;
    Vector3 endPosition;
    [SerializeField]AnimationClip walkAnimation;
    [SerializeField] bool exitFromLeft;
    [SerializeField] float distanceToDespawn;

    public override IEnumerator Perform()
    {
        eventSubject = GameObject.Find("EventSubject");
        startPosition = eventSubject.transform.position;
        if (!exitFromLeft)
        {
            endPosition = eventSubject.transform.position + (Vector3.right * distanceToDespawn);
            eventSubject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            endPosition = eventSubject.transform.position + (Vector3.left * distanceToDespawn);
            eventSubject.GetComponent<SpriteRenderer>().flipX = true;
        }
        eventSubject.GetComponent<Animator>().Play(walkAnimation.name);
        float delta = 0;
        while (delta < 1)
        {
            eventSubject.transform.position = Vector3.Lerp(startPosition, endPosition, delta);
            delta += Time.deltaTime * 0.5f;
            yield return new WaitForEndOfFrame();
        }
        Destroy(eventSubject);
    }
}
