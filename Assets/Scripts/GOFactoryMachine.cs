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

        private Dictionary<string, GameObject> inUse;
        private List<GameObject> waiting;

        private GOFactory factory;
        public GOFactory Factory
        { set { factory = value; } }

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
          inUse = new Dictionary<string, GameObject>();
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
            inUse.Add(id, model);
            return model;
        }

        public void putAway(GOFactoryTracker obj)
        {
          if (inUse.ContainsKey(obj.Id))
          {
            var creepInList = inUse[obj.Id];
            inUse.Remove(obj.Id);
            waiting.Add(creepInList);
            if (inactivityTimeBeforeDestroy > 0)
              factory.startChildCoroutine(destroyCoroutine(obj));
          }
        }

        public void remove(GOFactoryTracker obj)
        {
            if (waiting.Contains(obj.gameObject))
              waiting.Remove(obj.gameObject);
            if (inUse.ContainsKey(obj.Id))
              inUse.Remove(obj.Id);
            GameObject.Destroy(obj.gameObject);
        }

        IEnumerator destroyCoroutine(GOFactoryTracker obj)
        {
            float coroutineStartTime = Time.time;
            while (!obj.Active)
            {
                if (Time.time - coroutineStartTime > inactivityTimeBeforeDestroy)
                {
                    remove(obj);
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}