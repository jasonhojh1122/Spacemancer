using UnityEngine;
using System.Collections.Generic;
using Core;

namespace Gameplay.Electronic
{
    public class SecuredObject : SimpleAnimatedDevice
    {
        [SerializeField] protected Dimension.Color activeColor = Dimension.Color.WHITE;
        [SerializeField] protected List<ObjectColor> indicators;

        new protected void Awake()
        {
            base.Awake();
        }

        protected void Start()
        {
            foreach (var i in indicators)
                i.Color = activeColor;
        }

    }
}