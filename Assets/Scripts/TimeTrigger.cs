using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class TimeTrigger : MonoBehaviour, IConditionalTrigger
{
    [SerializeField] bool morning;
    [SerializeField] bool evening;
    [SerializeField] bool activateOtherTrigger;
    [SerializeField] TimeTrigger triggertoActivate;
    Collider coll;

	private void Awake()
	{
        coll = GetComponent<Collider>();
        coll.isTrigger = true;
    }

	void Start()
    {
        GameState.Register(this);
        CheckToEnable();
    }

    public void CheckToEnable()
    {
        coll.enabled = (morning && GameState.CheckCondition(Conditions.Morning)) || (evening && GameState.CheckCondition(Conditions.Evening));
    }

	private void OnTriggerEnter(Collider other)
	{
        if (other.tag == "Player")
        {
            if (activateOtherTrigger)
                triggertoActivate.GetComponent<Collider>().enabled = true;
            else
            {
                GameState.AdvanceTime();
                switch (GameState.time)
                {
                    case 0:
                        ChangeTaskText("Go to work");
                        break;
                    case 1:
                        ChangeTaskText("Go back home");
                        break;
                }
            }

                      
        }            
	}

    void ChangeTaskText(string text)
    {
        GameManager.GetInstance()._taskBox.GetComponent<TaskIndicator>().ChangeText(text);
    }

	private void OnDestroy()
	{
        GameState.Unregister(this);
	}
}
