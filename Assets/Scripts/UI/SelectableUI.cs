using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{

    public class SelectableUI : MonoBehaviour, ISelectHandler
    {
        [SerializeField] UnityEngine.Events.UnityEvent OnSelected;

        public void OnSelect(BaseEventData eventData)
        {
            OnSelected.Invoke();
        }
    }
}