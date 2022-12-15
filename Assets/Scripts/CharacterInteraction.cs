using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInteraction : MonoBehaviour
{
    EventTrigger nearbyEventTrigger;
    bool inGameTrigger;

    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.performed && inGameTrigger)
        {
            FindObjectOfType<MiniGameLoader>().StartGame();
            inGameTrigger = false;
        }            
        else if (context.performed && nearbyEventTrigger)
        {
            nearbyEventTrigger.CallStartEvent();
            nearbyEventTrigger = null;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EventTrigger>())
        {
            if (other.GetComponent<EventTrigger>().thisEvent.requireInput)
            {
                nearbyEventTrigger = other.GetComponent<EventTrigger>();
            }
        }
        else if (other.GetComponent<MiniGameLoader>())
            inGameTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<EventTrigger>())
        {
            nearbyEventTrigger = null;
        }
        else if (other.GetComponent<MiniGameLoader>())
            inGameTrigger = false;
    }
}
