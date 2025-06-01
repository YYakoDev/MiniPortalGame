
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateOnBuild : MonoBehaviour
{
    
    void Awake()
    {
        gameObject.SetActive((Application.installMode == ApplicationInstallMode.Editor));
    }
    
}
