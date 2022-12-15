using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchPlayer : MonoBehaviour
{
    GameObject character;

    private void Start()
    {
        character = GameObject.FindGameObjectWithTag("Player");
        GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = character.transform;
    }

}
