using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] List<GameObject> pooledObjects;
    [SerializeField] GameObject objectToPool;
    [SerializeField] int amountToPool;
    [SerializeField]bool resizeable = true;
    [SerializeField] Transform objSpawnPos;
    

    void Start()
    {
        pooledObjects = new List<GameObject>();
        for(int i = 0;i<amountToPool;i++)
        {
            CreateNewObject();
        }
    }

    public GameObject GetPooledObject()
    {
        foreach(var obj in pooledObjects)
        {
            if(!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        if(resizeable)return CreateNewObject();
        return null;
    }

    private GameObject CreateNewObject()
    {
        GameObject newObject = Instantiate(objectToPool);
        if(objSpawnPos != null) newObject.transform.SetParent(objSpawnPos.transform);
        else  newObject.transform.SetParent(this.transform);

        newObject.SetActive(false);
        pooledObjects.Add(newObject);

        return newObject;

    }
}