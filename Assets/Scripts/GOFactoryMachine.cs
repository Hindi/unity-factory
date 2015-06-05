using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GOF
{
    public class GOFactoryMachine
    {
        private string modelName;
        public string ModelName
        {
            set { modelName = value; }
        }

        private GameObject prefab;
        public GameObject Prefab
        {
            set { prefab = value; }
        }

        private List<GameObject> inUse;
        private List<GameObject> waiting;

        private Vector3 defaultPos;
        public Vector3 DefaultPos
        { set { defaultPos = value; } }

        private float inactivityTimeBeforeDestroy;
        public float InactivityTimeBeforeDestroy
        { set { inactivityTimeBeforeDestroy = value; } }

        private float activityTimeBeforeDestroy;
        public float ActivityTimeBeforeDestroy
        { set { activityTimeBeforeDestroy = value; } }

        private void Start()
        {
            inactivityTimeBeforeDestroy = 0;
            defaultPos = new Vector3(0, 0, 0);
        }

        public GOFactoryMachine()
        {
            inUse = new List<GameObject>();
            waiting = new List<GameObject>();
        }

        public GameObject createModel(string id)
        {
            return createModel(id, defaultPos);
        }

        public GameObject createModel(string id, Vector3 position)
        {
            GameObject model = null;
            if (waiting.Count > 0)
            {
                model = waiting[0];
                waiting.Remove(model);
                model.SetActive(true);
                model.transform.position = position;
                model.GetComponent<GOFactoryTracker>().Active = true;
            }
            else
            {
                if (prefab != null)
                    model = GameObject.Instantiate(prefab);
                else
                {
                    GameObject obj = Resources.Load(modelName) as GameObject;
                    if(obj != null)
                        model = GameObject.Instantiate(obj);
                    else
                        Debug.LogError("[GOF]Couldn't find any prefab named " + modelName + ". Returning null.");
                }

                model.transform.position = position;
                if (modelName != "")
                    model.name = modelName;

                GOFactoryTracker tracker = model.AddComponent<GOFactoryTracker>();
                tracker.Id = id;
                tracker.Machine = this;
                tracker.Active = true;
                tracker.InactivityTimeBeforeDestroy = inactivityTimeBeforeDestroy;
            }
            inUse.Add(model);
            return model;
        }

        public void putAway(GameObject obj)
        {
            var creepInList = inUse.Find(c => c.GetComponent<GOFactoryTracker>().Id == obj.GetComponent<GOFactoryTracker>().Id);
            inUse.Remove(creepInList);
            waiting.Add(creepInList);
            if (inactivityTimeBeforeDestroy > 0)
                StartCoroutine(destroyCoroutine());
        }

        public void remove(GameObject obj)
        {
            if (waiting.Contains(obj))
                waiting.Remove(obj);
            if (inUse.Contains(obj))
                inUse.Remove(obj);
            GameObject.Destroy(obj);
        }

        IEnumerator destroyCoroutine()
        {
            float coroutineStartTime = Time.time;
            while (!isActive)
            {
                if (Time.time - coroutineStartTime > inactivityTimeBeforeDestroy)
                {
                    machine.remove(gameObject);
                    break;
                }
                yield return new WaitForSeconds(inactivityTimeBeforeDestroy);
            }
        }
    }
}