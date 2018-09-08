using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPooler
{
    //private class for organising different pools of gameobjects
    private class Pool
    {
        //List of all gameobjects in the pool
        private List<GameObject> pooledObjects = new List<GameObject>();

        //The prefab that new gameobjects will be instantiated from
        public GameObject prefab;

        public Pool(GameObject prefab)
        {
            //New pools must have a prefab
            this.prefab = prefab;
        }

        public GameObject GetPooledObject()
        {
            //Search for inactive object in pool
            foreach (GameObject o in pooledObjects)
            {
                if (!o.activeSelf)
                {
                    //Activate and return any object found
                    o.SetActive(true);
                    return o;
                }
            }

            //If no object was found, instantiate a new one
            GameObject obj = GameObject.Instantiate(prefab);
            //Set name to that of prefab for comparisons
            obj.name = prefab.name;
            //Organised under pooled object "maintenance" gameobject
            obj.transform.SetParent(poolObject.transform);
            //Add new gameobject to pool and return
            pooledObjects.Add(obj);
            return obj;
        }

        public void Purge()
        {
            //Destroy every gameobject in this pool
            for (int i = 0; i < pooledObjects.Count; i++)
                GameObject.Destroy(pooledObjects[i]);

            pooledObjects.Clear();
        }

        public void ReturnAll()
        {
            for (int i = 0; i < pooledObjects.Count; i++)
                pooledObjects[i].SetActive(false);
        }
    }

    //List of all object pools
    private static List<Pool> objectPools = new List<Pool>();
    //An empty gameobject for organising pooled objects in the scene
    private static GameObject poolObject;

	/// <summary>
	/// Gets a pooled object from one of the object pools, or creates a new pool if one does not exist for this prefab
	/// </summary>
	public static GameObject GetPooledObject(GameObject prefab)
    {
        //Make sure there is a gameobject for organising pooled objects in the scene
        if(!poolObject)
        {
            poolObject = GameObject.Find("PooledObjects");

            //If there was no pre-existing object to hold them, create a new one
            if (!poolObject)
                poolObject = new GameObject("PooledObjects");

            GameObject.DontDestroyOnLoad(poolObject);
        }

        //Pool starts as null, since one will either be found or created
        Pool pool = null;

        //Attempt to find a pool with the same prefab
        foreach (Pool p in objectPools)
            if (p.prefab == prefab)
                pool = p;

        //If no pool was found...
        if (pool == null)
        {
            //Create a new pool and add it to the list of pools
            pool = new Pool(prefab);
            objectPools.Add(pool);
        }

        //Get a pooled object from the pool and return it
        return pool.GetPooledObject();
    }

	/// <summary>
	/// Clears all object pools
	/// </summary>
	public static void PurgePools()
    {
        //Eacg pool handles its own purging
        foreach (Pool pool in objectPools)
            pool.Purge();

        //Once pools have been purged, clear them
        objectPools.Clear();
    }

    public static void ReturnAll()
    {
        foreach (Pool pool in objectPools)
            pool.ReturnAll();
    }

	/// <summary>
	/// Ensures there is a pool with the required amount available.
	/// </summary>
	public static void SetupPool(GameObject prefab, int count)
	{
		GameObject[] objs = new GameObject[count];

		//Spawn desired amount of GameObjects
		for(int i = 0; i < count; i++)
		{
			objs[i] = GetPooledObject(prefab);
		}

		//Immediately return all newly spawned to pool
		for (int i = 0; i < count; i++)
		{
			objs[i].SetActive(false);
		}
	}
}

public static class ObjectPoolerExtensions
{
	/// <summary>
	/// Spawns a GameObject from the pool. If there is none, it will be instantiated.
	/// </summary>
	/// <param name="position">World space position to spawn.</param>
	public static GameObject SpawnPooled(this GameObject self, Vector3 position)
	{
		if (!self) return null;

		GameObject obj = ObjectPooler.GetPooledObject(self);
		obj.transform.position = position;
		obj.transform.localRotation = self.transform.localRotation;

		obj.SetActive(true);

		return obj;
	}
}