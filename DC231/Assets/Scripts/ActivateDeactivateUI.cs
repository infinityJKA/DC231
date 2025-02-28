using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateDeactivateUI : MonoBehaviour
{
    public GameObject targetObject;
    public void ActivateDeactivateGameObject()
    {
        targetObject.SetActive(!targetObject.activeInHierarchy);
    }
}
