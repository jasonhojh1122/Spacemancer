using UnityEngine;
using System.Collections.Generic;
using Core;

namespace SpaceDevice
{
    public class DimensionSelector : MonoBehaviour
    {
        [SerializeField] SplitMergeMachine machine;
        [SerializeField] List<Animator> animators;

        /// <summary>
        /// Rotates the dimension color to the next one if world is not splitted;
        /// </summary>
        /// <param name="i"> The index of the dimension to change. </param>
        public void RotateColor(int i)
        {
            if (World.Instance.Splitted) return;
            RotateSelector(i);
            RotateColorId(i);
            if (i == World.Instance.ActiveDimId)
                AutoChangeDimColorActive(i);
            else
                AutoChangeDimColorOther(i);
            machine.UpdateDimColorIndicator();
        }

        void RotateColorId(int i)
        {
            if (machine.DimColorIds[i] < 0)
                machine.DimColorIds[i] = 0;
            else
                machine.DimColorIds[i] = (machine.DimColorIds[i] + 1) % Dimension.SplittedColor.Count;
        }

        void AutoChangeDimColorActive(int i)
        {
            var missingColor = Dimension.MissingColor(machine.GetColorByID(i));
            int missingId = 0;
            for (int j = 0; j < machine.DimColorIds.Count; j++)
            {
                var id = (i+j+1) % machine.DimColorIds.Count;
                if (id == i)
                    continue;
                else if (missingId >= missingColor.Count)
                    machine.DimColorIds[id] = -1;
                else
                {
                    machine.DimColorIds[id] = Dimension.ValidColorIndex[missingColor[missingId]];
                    missingId += 1;
                }
                RotateSelector(id);
            }
        }

        void AutoChangeDimColorOther(int i)
        {
            var missingColor = Dimension.MissingColor(machine.GetColorByID(i));
            machine.DimColorIds[World.Instance.ActiveDimId] = Dimension.ValidColorIndex[missingColor[0]];
            int missingId = 1;
            for (int j = 0; j < machine.DimColorIds.Count; j++)
            {
                var id = (i+j+1) % machine.DimColorIds.Count;
                if (id == World.Instance.ActiveDimId || id == i)
                    continue;
                else if (missingId >= missingColor.Count)
                    machine.DimColorIds[id] = -1;
                else
                {
                    machine.DimColorIds[id] = Dimension.ValidColorIndex[missingColor[missingId]];
                    missingId += 1;
                }
                RotateSelector(id);
            }
        }

        void RotateSelector(int i)
        {
            animators[i].SetTrigger("Rotate");
        }

    }
}