using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [SerializeField] Animator playerAnimator;
    [SerializeField] RuntimeAnimatorController controller;
    [SerializeField] int defaultAnimators = 4;
    [SerializeField] bool allowAddAnimators;
    AnimatorOverrideController playerOverride;
    List<Animator> animators = new List<Animator>();
    List<AnimatorOverrideController> overrides = new List<AnimatorOverrideController>();

    void Awake()
    {
        playerOverride = new AnimatorOverrideController(playerAnimator.runtimeAnimatorController);
        playerAnimator.runtimeAnimatorController = playerOverride;

		for (int i = 0; i < defaultAnimators; i++)
		{
            GameObject obj = new GameObject("Animator");
            obj.transform.parent = transform;
            obj.transform.localScale *= 0.095f;
            obj.AddComponent<SpriteRenderer>();

            Animator animator = obj.AddComponent<Animator>();
            animators.Add(animator);
            AnimatorOverrideController overrideCtrl = new AnimatorOverrideController(controller);
            overrides.Add(overrideCtrl);
            animator.runtimeAnimatorController = overrideCtrl;
		}
    }

	public IEnumerator PlayAnimation(Animation[] anims)
    {
        foreach (Animation anim in anims)
		{
            Animator animator;
            AnimatorOverrideController overrideCtrl;
            int index = -1;

            if (anim.mainCharacter)
            {
                animator = playerAnimator;
                overrideCtrl = playerOverride;

                Vector3 scale = animator.transform.parent.localScale;
                scale.x *= anim.flip ? -1f : 1f;
                animator.transform.parent.localScale = scale;
			}
            else
            {
                index = FindAnimator();
                if (index == -1)
                {
                    Debug.LogError("Cannot find an animator to play Event animation, consider setting 'allowAddAnimators' to TRUE", anim.animationClip);
                    yield break;
                }
                else
                {
                    animator = animators[index];
                    overrideCtrl = overrides[index];

                    animator.transform.position = anim.position;
                    Vector3 scale = animator.transform.localScale;
                    scale.x *= anim.flip ? -1f : 1f; ;
                    animator.transform.localScale = scale;
                }
            }

            overrideCtrl["override"] = anim.animationClip;
            animator.SetFloat("Speed", anim.playbackSpeed);
            animator.SetTrigger("Override");

            if (anim.applyMotion)
            {
                animator.SetBool("Loop", true);
                if (anim.waitForEnd)
                    yield return StartCoroutine(Move(index, anim));
                else
                {
                    StartCoroutine(Move(index, anim));
                    while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Override"))
                        yield return null;
                }
            }
            else if (anim.loop)
            {
                animator.SetBool("Loop", true);

                if (anim.stopAtEnd)
                    EventManager.SetStopAnimation(index);
                else
                    EventManager.SetStopAnimation(index, anim.stopAtStep);

                yield return null;
            }
            else if (anim.waitForEnd)
            {
                yield return new WaitForSeconds(anim.animationClip.length / anim.playbackSpeed);
            }                
            else
            {
                while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Override"))
                    yield return null;
            }                
        }
    }

    int FindAnimator()
    {
        for (int i = 0; i < animators.Count; i++)
        {
            if (animators[i].GetCurrentAnimatorStateInfo(0).IsName("Start"))
                return i;
        }

        if (allowAddAnimators)
        {
            GameObject obj = new GameObject("Animator");
            obj.transform.parent = transform;
            obj.transform.localScale *= 0.095f;
            obj.AddComponent<SpriteRenderer>();

            Animator animator = obj.AddComponent<Animator>();
            animators.Add(animator);
            AnimatorOverrideController overrideCtrl = new AnimatorOverrideController(controller);
            overrides.Add(overrideCtrl);
            animator.runtimeAnimatorController = overrideCtrl;

            return animators.Count - 1;
        }
        else
            return -1;
    }

    IEnumerator Move(int index, Animation anim)
    {
        Animator animator = index == -1 ? playerAnimator : animators[index];
        Transform transform = index == -1 ? playerAnimator.transform.parent : animator.transform;
        Vector3 initPos = transform.position;

        float delta = 0f;
        while (delta < 1f)
        {
            transform.position = Vector3.Lerp(initPos, anim.destination, delta);
            delta += Time.deltaTime / anim.travelTime;
            yield return null;
        }
            
        animator.SetBool("Loop", false);
	}

    public void StopLoop(int animatorIndex)
    {
        if (animatorIndex == -1)
            playerAnimator.SetBool("Loop", false);            
        else
            animators[animatorIndex].SetBool("Loop", false);
	}
}
