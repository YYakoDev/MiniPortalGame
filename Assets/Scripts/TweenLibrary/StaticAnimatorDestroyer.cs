using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticAnimatorDestroyer : MonoBehaviour
{
    private void OnDestroy()
    {
        HelperMethods.DestroyAnimators();
    }
}
