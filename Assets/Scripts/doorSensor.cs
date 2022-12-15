using System.Collections;
using UnityEngine;

public class doorSensor : MonoBehaviour
{
    [SerializeField] GameObject sensor;

    public bool IsDark;

    Animator anim;

    void Start()
    {
        anim = sensor.GetComponent<Animator>();
        //anim.SetBool("IsOpening", true);

        if (IsDark)
        {
            anim.SetBool("IsDark", true);
        }
        else
        {
            anim.SetBool("IsDark", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("IsOpening", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("IsClosing", true);

            StartCoroutine(Timer());
        }
    }

    IEnumerator Timer() 
    {
        yield return new WaitForSeconds(0.5f);
        
        anim.SetBool("IsOpening", false);
        anim.SetBool("IsClosing", false);

    }
}
