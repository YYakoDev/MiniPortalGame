using System;
using UnityEngine;

public class NotMonoObjectPool
{
    int _amountToPool;
    GameObject _objToPool;
    Transform _parent;
    GameObject[] _pooledObjs;
    bool _resizable;

    public GameObject[] PooledObjects => _pooledObjs;

    public NotMonoObjectPool(int amountToPool, GameObject objectToPool, Transform parent = null, bool isResizable = true)
    {
        _amountToPool = amountToPool;
        _objToPool = objectToPool;
        _parent = parent;
        _resizable = isResizable;
        _pooledObjs = new GameObject[amountToPool];
        PoolObjects();
    }

    void PoolObjects()
    {
        for (int i = 0; i < _amountToPool; i++)
        {
            CreateNew(i);
        }
    }

    public GameObject GetObjectFromPool()
    {
        for (int i = 0; i < _pooledObjs.Length; i++)
        {
            if(_pooledObjs[i] == null) continue;
            if(_pooledObjs[i].activeInHierarchy)continue;
            return _pooledObjs[i];
        }
        if(_resizable)
        {
            int currentLength = _pooledObjs.Length; // <- 5
            Array.Resize<GameObject>(ref _pooledObjs, currentLength+1); // <-6
            CreateNew(currentLength); // <-5
            return _pooledObjs[currentLength];
        }
        return null;
    }

    void CreateNew(int indexToAdd)
    {
        GameObject obj = GameObject.Instantiate(_objToPool, Vector3.zero, Quaternion.identity);
        obj.transform.SetParent(_parent);
        obj.SetActive(false);
        _pooledObjs[indexToAdd] = obj;
    }
}
