using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GOF
{
    public class GOFactoryMachine
    {
        private string machineName;
        public string MachineName
        {
            set { machineName = value; }
            get { return machineName; }
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

        private float inactiveLifeSpan;
        public float InactiveLifeSpan
        { set { inactiveLifeSpan = value; } }

        private float lifeSpan;
        public float LifeSpan
        { set { lifeSpan = value; } }

        private void Start()
        {
            inactiveLifeSpan = 0;
            lifeSpan = 0;
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
            GameObject obj = null;
            if (waiting.Count > 0)
            {
                obj = waiting[0];
                waiting.Remove(obj);
                obj.transform.position = position;
                obj.GetComponent<GOFactoryTracker>().resetObject();
            }
            else
            {
                if (prefab != null)
                    obj = GameObject.Instantiate(prefab);
                else
                {
                    GameObject model = Resources.Load(machineName) as GameObject;
                    if (model != null)
                        obj = GameObject.Instantiate(model);
                    else
                    {
                        Debug.LogError("[GOF]Couldn't find any prefab named " + machineName + ". Returning null.");
                        return null;
                    }
                }

                obj.transform.position = position;
                if (machineName != "")
                    obj.name = machineName;
                obj.transform.SetParent(factory.transform);

                GOFactoryTracker tracker = obj.AddComponent<GOFactoryTracker>();
                tracker.Id = id;
                tracker.Machine = this;
                tracker.recycle();
            }
            inUse.Add(id, obj);

            if (lifeSpan > 0)
                factory.startChildCoroutine(activeLifeSpanCoroutine(obj.GetComponent<GOFactoryTracker>(), lifeSpan));

            return obj;
        }

        public void putAway(GOFactoryTracker obj)
        {
            if (obj != null)
            {
                if (inUse.ContainsKey(obj.Id))
                {
                    var creepInList = inUse[obj.Id];
                    inUse.Remove(obj.Id);
                    waiting.Add(creepInList);
                    if (inactiveLifeSpan > 0)
                        factory.startChildCoroutine(inactiveLifeSpanCoroutine(obj, inactiveLifeSpan));
                }
            }
        }

        public void remove(GOFactoryTracker obj)
        {
            if(obj != null)
            {
                if (waiting.Contains(obj.gameObject))
                    waiting.Remove(obj.gameObject);
                if (inUse.ContainsKey(obj.Id))
                    inUse.Remove(obj.Id);
                GameObject.Destroy(obj.gameObject);
            }
        }

        public void clear()
        {
            foreach (GameObject g in waiting)
                GameObject.Destroy(g);
            foreach (KeyValuePair<string, GameObject> p in inUse)
                GameObject.Destroy(p.Value);
            waiting.Clear();
            inUse.Clear();
        }

        IEnumerator activeLifeSpanCoroutine(GOFactoryTracker obj, float time)
        {
            yield return new WaitForSeconds(time);
            remove(obj);
        }

        IEnumerator inactiveLifeSpanCoroutine(GOFactoryTracker obj, float time)
        {
            float coroutineStartTime = Time.time;
            while (obj != null && !obj.gameObject.activeSelf)
            {
                if (Time.time - coroutineStartTime > time)
                {
                    remove(obj);
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}