
using UnityEngine;
using System.Collections.Generic;

public class SplitableObject : MonoBehaviour {
    [SerializeField] Dimension.Color color;
    Collider col;
    bool merged;

    void Start() {
        col = GetComponent<Collider>();
        merged = false;
    }

    public void Split() {
        merged = false;
    }

    public void Merge(SplitableObject parent, Transform whiteRoot) {

        merged = true;

        if (parent != null && parent.GetColor() == Dimension.Color.BLACK) {
            color = Dimension.Color.BLACK;
        }

        Collider[] colliders = Physics.OverlapBox(col.bounds.center, col.bounds.extents * 0.9f, transform.rotation);
        foreach (Collider c in colliders) {
            if (c == null || c == col) continue;

            var so = c.gameObject.GetComponent<SplitableObject>();
            if (so.IsMerged()) continue;

            if (c.gameObject.name == gameObject.name && (c.transform.localPosition - transform.localPosition).magnitude < 0.1f) {
                color = Dimension.AddColor(color, so.GetColor());
                Destroy(so.gameObject);
            }
            else {
                color = Dimension.Color.BLACK;
                so.Merge(this, whiteRoot);
            }
        }

        if (color == Dimension.Color.RED || color == Dimension.Color.GREEN || color == Dimension.Color.BLUE) {
            Destroy(this.gameObject);
        }
        Vector3 localPos = transform.localPosition;
        Quaternion localRot = transform.localRotation;
        transform.SetParent(whiteRoot);
        transform.localPosition = localPos;
        transform.localRotation = localRot;

    }

    public Dimension.Color GetColor() {
        return color;
    }
    public void SetColor(Dimension.Color color) {
        this.color = color;
    }
    public bool IsMerged() {
        return merged;
    }

}