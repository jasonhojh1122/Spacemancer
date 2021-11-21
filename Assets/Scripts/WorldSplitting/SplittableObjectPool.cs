using UnityEngine;
using System.Linq;
using System.Collections.Generic;


using Set = System.Collections.Generic.HashSet<Core.SplittableObject>;

namespace Core
{

    public class ObjectPool
    {
        Dictionary<string, Set> pool;

        public Dictionary<string, Set> Pool {
            get => pool;
        }

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

        public SplittableObject GetOne(string name)
        {
            if (pool.ContainsKey(name))
            {
                return pool[name].FirstOrDefault();

                /* SplittableObject so = null;
                if (pool[name].Count > 0)
                {
                    so = pool[name].FirstOrDefault();
                    pool[name].Remove(so);
                }
                else if (pool[name].Count == 1)
                {
                    so = InstantiateDefault(name);
                } */
            }
            return null;
        }

        public SplittableObject InstantiateDefault(string name)
        {
            if (pool.ContainsKey(name))
            {
                var so = pool[name].FirstOrDefault();
                if (so != null)
                    return GameObject.Instantiate<SplittableObject>(pool[name].FirstOrDefault());
            }
            return null;
        }

    }

    public class SplittableObjectPool
    {
        ObjectPool active;
        ObjectPool inactive;

        public ObjectPool ActiveObjects {
            get => active;
        }
        public ObjectPool InactiveObjects {
            get => inactive;
        }

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
            SplittableObject so = inactive.GetOne(name);
            if (so == null)
            {
                so = active.InstantiateDefault(name);
            }
            if (so != null)
            {
                SetActive(so);
            }
            return so;
        }

    }

}