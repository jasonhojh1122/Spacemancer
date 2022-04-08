using UnityEngine;
using UnityEngine.VFX;
using System.Collections.Generic;

namespace Gameplay.Electronic
{

    public class Stabilizer : SimpleAnimatedDevice
    {
        [SerializeField] VisualEffect vfx;
        [SerializeField] List<Core.ObjectColor> toStabilized;
        [SerializeField] int phase;
        bool used = false;

        public override void TurnOn()
        {
            if (!used)
            {
                used = true;
                OnTurnOn.Invoke();
                StartCoroutine(Anim());
                Saving.GameSaveManager.Instance.GameSave.phase = phase;
            }
        }

        System.Collections.IEnumerator Anim()
        {
            vfx.Play();
            IsOpened = true;
            yield return new WaitForSeconds(1.5f);
            foreach (var oc in toStabilized)
                oc.Color = Core.Dimension.Color.WHITE;
        }

        public override void TurnOff()
        {
        }

        public override void Toggle()
        {
            TurnOn();
        }

        public override void OnColorChange()
        {
        }

        public override void OnDimensionChange()
        {
        }
    }
}