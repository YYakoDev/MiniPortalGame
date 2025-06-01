using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateItself : MonoBehaviour
{
    [SerializeField]float _deactivationTime = 0.5f;

    private IEnumerator Deactivation()
    {
        yield return new WaitForSeconds(_deactivationTime);
        gameObject.SetActive(false);
    }
    private void OnEnable() {
        StartCoroutine(Deactivation());
    }
    private void OnDisable() {
        StopAllCoroutines();
    }
}
