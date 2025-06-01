using UnityEngine;

public class ActivateOnBuild : MonoBehaviour
{
    [SerializeField] private GameObject[] _objectsToActivate;

    void Awake()
    {
        if(Application.installMode == ApplicationInstallMode.Editor) return;
        Activate();
    }
    
    public void Activate()
    {
        Debug.LogWarning("This method should only be invoked on build");
        if (_objectsToActivate == null || _objectsToActivate.Length == 0) return;
        foreach (var item in _objectsToActivate)
        {
            item.SetActive(true);
        }
    }
}
