using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUpInScene : CustomStep
{
    [SerializeField] GameObject npc;
    [SerializeField] AnimationClip walkAnimation;
    [SerializeField] AnimationClip idleAnimation;
    [SerializeField] bool needToBeFlip;
    [SerializeField] float distanceToSpawn;
    float speed = 500;
    Vector3 startPosition;
    Vector3 endPosition;
    

    public override IEnumerator Perform()
    {
        
        startPosition = GameManager.GetInstance()._player.transform.position + (Vector3.right * distanceToSpawn);
        endPosition = GameManager.GetInstance()._player.transform.position + (Vector3.right * 2);
        GameObject eventSubject = Instantiate<GameObject>(npc, startPosition, Quaternion.identity);
        eventSubject.name = "EventSubject";
        
        eventSubject.GetComponent<SpriteRenderer>().flipX = true;
        eventSubject.GetComponent<Animator>().Play(walkAnimation.name);
        
        float delta = 0;
        while(delta < 1)
        {
            
            eventSubject.transform.position = Vector3.Lerp(startPosition, endPosition, delta);
            delta += Time.deltaTime*0.5f;
            yield return new WaitForEndOfFrame();
        }
        if (needToBeFlip)
        {
            GameManager.GetInstance()._player.GetComponent<CharacterMovement>().SwitchDirection();
        }
        eventSubject.GetComponent<Animator>().Play(idleAnimation.name);
        yield return new WaitForEndOfFrame();
    }
}
