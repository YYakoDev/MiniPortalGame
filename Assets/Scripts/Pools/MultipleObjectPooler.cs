using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleObjectPooler : MonoBehaviour
{
    [SerializeField] GameObject[] objectsToPool;
    [SerializeField] int amountToPool;
    [SerializeField] Transform objectsParent;
    List<GameObject> pooledObjects = new List<GameObject>();

    void Start()
    {
        for(int i = 0;i<amountToPool;i++)
        {
            foreach(GameObject poolObj in objectsToPool)
            {
                CreateNewObject(poolObj);
            }
            
        }
    }
    public GameObject GetPooledObjectMatch(GameObject matchingGameObject)
    {
        string matchingObjectName = matchingGameObject.name + ("(Clone)");
        foreach(var obj in pooledObjects)
        {
            if(obj.activeInHierarchy)continue;
            if(obj.name == matchingObjectName)
            {
                //Debug.Log("found a matching object");
                obj.SetActive(true);
                return obj;
            }
            
        }
        return CreateNewObject(matchingGameObject);
    }

    private GameObject CreateNewObject(GameObject objectToCreate)
    {
        GameObject newObject = Instantiate(objectToCreate);

        if(objectsParent != null) newObject.transform.SetParent(objectsParent.transform);
        else  newObject.transform.SetParent(this.transform);

        newObject.SetActive(false);
        pooledObjects.Add(newObject);

        return newObject;

    }
}
