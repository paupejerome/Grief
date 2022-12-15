using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TaskIndicator : MonoBehaviour
{
    Text taskText;
    string upcomingText;
    float fadingSpeed = 1.2f;
    private void Awake()
    {
        taskText = transform.GetChild(0).GetComponent<Text>();
    }
    
    public void ChangeText(string text)
    {
        upcomingText = text;
        StartCoroutine(Fader());
    }
    IEnumerator Fader()
    {
        while(taskText.color.a > 0)
        {
            taskText.color -= new Color(0, 0, 0, fadingSpeed*Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        taskText.text = upcomingText;
        while (taskText.color.a < 1)
        {
            taskText.color += new Color(0, 0, 0, fadingSpeed*Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

}
