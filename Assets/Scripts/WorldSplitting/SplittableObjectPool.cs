using UnityEngine;
using System.Linq;
using System.Collections.Generic;


using Set = System.Collections.Generic.HashSet<Core.SplittableObject>;

namespace Core
{

    public class ObjectPool
    {
        Dictionary<string, Set> pool;

        public ObjectPool()
        {
            pool = new Dictionary<string, Set>();
        }

        public void Add(SplittableObject so)
        {
            if (pool.ContainsKey(so.name))
            {
                pool[so.name].Add(so);
            }
            else
            {
                pool.Add(so.name, new Set(){so});
            }
        }

        public void Remove(SplittableObject so)
        {
            if (pool.ContainsKey(so.name))
            {
                pool[so.name].Remove(so);
            }
        }

        public SplittableObject GetOneOrInstantiate(string name)
        {
            if (pool.ContainsKey(name))
            {
                SplittableObject so = null;
                if (pool[name].Count > 1)
                {
                    so = pool[name].FirstOrDefault();
                    pool[name].Remove(so);
                }
                else if (pool[name].Count == 1)
                {
                    so = InstantiateDefault(name);
                }
                return so;
            }
            return null;
        }

        public SplittableObject InstantiateDefault(string name)
        {
            if (pool.ContainsKey(name))
            {
                return GameObject.Instantiate<SplittableObject>(pool[name].FirstOrDefault());
            }
            else
            {
                return null;
            }
        }

    }

    public class SplittableObjectPool
    {
        ObjectPool active;
        ObjectPool inactive;

        public SplittableObjectPool()
        {
            active = new ObjectPool();
            inactive = new ObjectPool();
        }

        public void SetActive(SplittableObject so)
        {
            inactive.Remove(so);
            active.Add(so);
            so.gameObject.SetActive(true);
        }

        public void SetInactive(SplittableObject so)
        {
            active.Remove(so);
            inactive.Add(so);
            so.gameObject.SetActive(false);
        }

        public SplittableObject Instantiate(string name) {
            SplittableObject so = inactive.GetOneOrInstantiate(name);
            if (so == null)
            {
                so = active.InstantiateDefault(name);
            }
            SetActive(so);
            return so;
        }

    }

}