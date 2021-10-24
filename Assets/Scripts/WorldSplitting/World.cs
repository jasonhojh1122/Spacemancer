using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class World : MonoBehaviour
{
    [SerializeField] Transform whiteRoot;
    [SerializeField] Transform redRoot;
    [SerializeField] Transform greenRoot;
    [SerializeField] Transform blueRoot;
    [SerializeField] float moveSpeed;
    [SerializeField] float trunSpeed;
    [SerializeField] Dimension whiteDimension;
    [SerializeField] List<Dimension> splittedDimensions;

    public UnityEvent OnMergingStart;
    public UnityEvent OnMergingEnd;
    public UnityEvent OnSplittingStart;
    public UnityEvent OnSplittingEnd;

    Dictionary<Dimension.Color, Transform> roots;

    bool splitted;


    void Start() {

        roots = new Dictionary<Dimension.Color, Transform>();
        roots.Add(Dimension.Color.WHITE, whiteRoot);
        roots.Add(Dimension.Color.RED, redRoot);
        roots.Add(Dimension.Color.GREEN, greenRoot);
        roots.Add(Dimension.Color.BLUE, blueRoot);

        // StartCoroutine(test());
        splitted = true;
    }

    IEnumerator test() {
        MergeDimensions();
        yield return new WaitForSeconds(5);
        SplitDimensions();
    }

    void Update() {

    }

    public void SplitDimensions() {
        foreach (Dimension dimension in splittedDimensions) {
            SplitColor(dimension.color);
        }
        StartCoroutine(Splitting());
        Destroy(roots[Dimension.Color.WHITE].gameObject);
    }

    void SplitColor(Dimension.Color color) {
        roots[color] = Instantiate(roots[Dimension.Color.WHITE]);
        roots[color].name = color.ToString();
        SplitableObject[] objects = roots[color].GetComponentsInChildren<SplitableObject>();
        foreach (SplitableObject so in objects) {
            if (so.GetColor() == Dimension.Color.BLACK)
                continue;
            else if ((so.GetColor() & color) == 0)
                Destroy(so.gameObject);
            else {
                so.Split();
                so.SetColor(color);
            }
        }
    }

    public void MergeDimensions() {
        StartCoroutine(Merging());
    }

    public void Merge() {
        roots[Dimension.Color.WHITE] = new GameObject(Dimension.Color.WHITE.ToString()).transform;
        roots[Dimension.Color.WHITE].position = whiteDimension.position;
        roots[Dimension.Color.WHITE].rotation = whiteDimension.rotation;

        foreach(Dimension spliited in splittedDimensions) {
            MergeColor(spliited.color);
        }
    }

    void MergeColor(Dimension.Color color) {
        SplitableObject[] objects = roots[color].GetComponentsInChildren<SplitableObject>();
        foreach (SplitableObject so in objects) {
            if (so == null || so.gameObject == null) continue;
            so.Merge(null, roots[Dimension.Color.WHITE]);
        }
        Destroy(roots[color].gameObject);
    }

    IEnumerator Splitting() {
        OnSplittingStart.Invoke();
        while (true) {
            int c = 0;

            foreach (Dimension dimension in splittedDimensions) {
                bool r = MoveDimension(roots[dimension.color], dimension.position, dimension.rotation);
                if (r) c++;
            }

            if (c == splittedDimensions.Count) break;
            else yield return null;
        }
        OnSplittingEnd.Invoke();
    }

    IEnumerator Merging() {
        OnMergingStart.Invoke();
        while (true) {
            int c = 0;

            foreach (Dimension dimension in splittedDimensions) {
                bool r = MoveDimension(roots[dimension.color], whiteDimension.position, whiteDimension.rotation);
                if (r) c++;
            }

            if (c == splittedDimensions.Count) break;
            else yield return null;
        }
        OnMergingEnd.Invoke();
    }

    bool MoveDimension(Transform t, Vector3 tarPos, Quaternion tarRot) {
        t.position = Vector3.MoveTowards(t.position, tarPos, moveSpeed * Time.deltaTime);
        t.rotation = Quaternion.RotateTowards(t.rotation, tarRot, trunSpeed * Time.deltaTime);
        return (((t.position - tarPos).magnitude < 0.1f) && (Quaternion.Angle(t.rotation, tarRot) < 0.1f));
    }

    public void Toggle() {
        if (splitted) {
            MergeDimensions();
            splitted = false;
        }
        else {
            SplitDimensions();
            splitted = true;
        }
    }

}
