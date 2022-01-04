using UnityEngine;
using System.Linq;
using System.Collections.Generic;


using Set = System.Collections.Generic.HashSet<Core.SplittableObject>;

namespace Core
{
    /// <summary>
    /// A pool of <c>SplittableObject</c>.
    /// </summary>
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
            }
            return null;
        }

        public SplittableObject InstantiateDefault(string name)
        {
            if (pool.ContainsKey(name))
            {
                var so = pool[name].FirstOrDefault();
                if (so != null)
                    return Object.Instantiate<SplittableObject>(pool[name].FirstOrDefault());
            }
            return null;
        }

    }

    /// <summary>
    /// A pool of active and inactive <c>SplittableObject</c>
    /// </summary>
    public class SplittableObjectPool
    {
        ObjectPool active;
        ObjectPool inactive;

        /// <summary>
        /// A <c>Dictionary</c> mapping from a string to a set of
        /// active <c>SplittableObject</c> with a same name as the key.
        /// </summary>
        public Dictionary<string, Set> ActiveObjectsPool {
            get => active.Pool;
        }

        /// <summary>
        /// A <c>Dictionary</c> mapping from a string to a set of
        /// inactive <c>SplittableObject</c> with a same name as the key.
        /// </summary>
        public Dictionary<string, Set> InactiveObjectsPool {
            get => inactive.Pool;
        }

        public SplittableObjectPool()
        {
            active = new ObjectPool();
            inactive = new ObjectPool();
        }

        /// <summary>
        /// Adds the given <c>SplittableObject</c> into <c>ActiveObjectsPool</c>,
        /// removes it from <c>InactiveObjectsPool</c> and set it's <c>GameObject</c> to active.
        /// </summary>
        /// <param name="so"> The <c>SplittableObject</c> to be set. </param>
        public void SetActive(SplittableObject so)
        {
            inactive.Remove(so);
            active.Add(so);
            so.gameObject.SetActive(true);
        }

        /// <summary>
        /// Adds the given <c>SplittableObject</c> into <c>InactiveObjectsPool</c>,
        /// removes it from <c>ActiveObjectsPool</c> and set it's <c>GameObject</c> to inactive.
        /// </summary>
        /// <param name="so"> The <c>SplittableObject</c> to be set. </param>
        public void SetInactive(SplittableObject so)
        {
            active.Remove(so);
            inactive.Add(so);
            so.gameObject.SetActive(false);
        }

        /// <summary>
        /// Gets a <c>SplittableObject</c> from the <c>InactiveObjectsPool</c> if any presents, or
        /// instantiates one from <c>ActiveObjectsPool</c> by the given name.
        /// </summary>
        /// <param name="name"> The name of the <c>SplittableObject</c> to be instantiated. </param>
        /// <returns> The instantiated <c>SplittableObject</c>. </returns>
        public SplittableObject Instantiate(string name) {
            SplittableObject so = inactive.GetOne(name);
            if (so == null)
            {
                so = active.InstantiateDefault(name);
            }
            if (so != null)
            {
                so.gameObject.name = name;
                SetActive(so);
            }
            return so;
        }

    }

}