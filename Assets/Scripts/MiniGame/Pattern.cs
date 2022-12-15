using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pattern : MonoBehaviour
{
    [SerializeField] GameObject coins;

    public void ActiveCoins(bool isActive)
    {
        coins.SetActive(isActive);
    }
}
