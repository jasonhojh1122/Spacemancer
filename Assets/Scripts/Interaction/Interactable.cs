using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction {

public abstract class Interactable : MonoBehaviour
{
    public abstract void Interact();
    public abstract void OnZoneEnter(Collider other);
    public abstract void OnZoneStay(Collider other);
    public abstract void OnZoneExit(Collider other);
}

}