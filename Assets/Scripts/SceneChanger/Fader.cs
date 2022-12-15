using UnityEngine;

public class Fader : MonoBehaviour
{
    public Animator faderAnimator;

    private void Start()
    {
        FadeOut();
    }


    public void FadeIn()
    {
        faderAnimator.SetTrigger("FadeIn");
    }

    public void FadeOut()
    {
        faderAnimator.SetTrigger("FadeOut");
    }

    public float AnimLenght()
    {
        return  faderAnimator.GetCurrentAnimatorStateInfo(0).length;
    }
}
