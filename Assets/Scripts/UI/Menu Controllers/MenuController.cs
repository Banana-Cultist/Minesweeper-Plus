using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class MenuController: MonoBehaviour
{
	public abstract void SetDelegate(MonoBehaviour value);
	public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}

