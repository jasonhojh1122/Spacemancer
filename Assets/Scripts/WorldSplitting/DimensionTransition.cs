using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using VecDic = System.Collections.Generic.Dictionary<Dimension.Color, UnityEngine.Vector3>;
using QuaDic = System.Collections.Generic.Dictionary<Dimension.Color, UnityEngine.Quaternion>;

public class DimensionTransition : MonoBehaviour{

    [SerializeField] World world;
    [SerializeField] CameraTransition cameraTransition;
    [SerializeField] Material material;
    static string dissolveName = "_Dissolve";

    void Start() {
        Init();
    }

    public void Init() {
        float rot = 0f;
        Vector3 pos = new Vector3(0f, 0f, -world.Radius);
        foreach (Dimension.Color bc in Dimension.BaseColor) {
            world.Dims[bc].TargetAngle = rot;
            rot += 120f;
        }
        world.Dims[Dimension.Color.WHITE].TargetAngle = 0f;
        material.SetFloat(dissolveName, 0.0f);
    }

    public IEnumerator SplitTransition() {
        yield return StartCoroutine(FadeMainDimension(false));
        world.SplitObjects();
        world.DestoryObjects();

        DimensionsSetActive(true);
        StartCoroutine(cameraTransition.Transition());

        VecDic targetPos = new VecDic();
        foreach (Dimension.Color bc in Dimension.BaseColor) {
            Vector3 VecDic = new Vector3(0, 0, -world.Radius);
            Quaternion rot = Quaternion.AngleAxis(world.Dims[bc].TargetAngle, Vector3.up);
            targetPos.Add(bc, rot * VecDic);
        }

        float t = 0.0f;
        while (t < world.TransitionDur) {
            t += Time.deltaTime;
            material.SetFloat(dissolveName, (world.TransitionDur - t) / world.TransitionDur);

            float p = t / world.TransitionDur;
            foreach (Dimension.Color bc in Dimension.BaseColor) {
                var dimTran = world.Dims[bc].transform;
                dimTran.position = targetPos[bc] * p;
                dimTran.rotation = Quaternion.identity;
                dimTran.RotateAround(dimTran.position, Vector3.up, p * world.Dims[bc].TargetAngle);
            }
            yield return null;
        }

        foreach (Dimension.Color bc in Dimension.BaseColor) {
            var dim = world.Dims[bc];
            dim.transform.position = targetPos[bc];
            dim.transform.rotation = Quaternion.identity;
            dim.transform.RotateAround(dim.transform.position, Vector3.up, dim.TargetAngle);
        }

        material.SetFloat(dissolveName, 0.0f);
        Physics.SyncTransforms();
    }

    public IEnumerator MergeTransition() {
        StartCoroutine(cameraTransition.Transition());
        VecDic startPos = new VecDic();
        QuaDic startRot = new QuaDic();
        foreach (Dimension.Color bc in Dimension.BaseColor) {
            startRot.Add(bc, world.Dims[bc].transform.rotation);
            startPos.Add(bc, world.Dims[bc].transform.position);
        }
        float t = 0.0f;
        while (t < world.TransitionDur) {
            t += Time.deltaTime;
            material.SetFloat(dissolveName, t / world.TransitionDur);
            float p = t / world.TransitionDur;
            float q = (world.TransitionDur - t) / world.TransitionDur;
            foreach (Dimension.Color bc in Dimension.BaseColor) {
                var dimTran = world.Dims[bc].transform;
                dimTran.position = startPos[bc] * q;
                dimTran.rotation = startRot[bc];
                dimTran.RotateAround(dimTran.position, Vector3.up, -p * world.Dims[bc].TargetAngle);
            }
            yield return null;
        }
        foreach (Dimension.Color bc in Dimension.BaseColor) {
            var dim = world.Dims[bc];
            dim.transform.position = Vector3.zero;
            dim.transform.rotation = Quaternion.identity;
        }

        world.Dims[Dimension.Color.WHITE].gameObject.SetActive(true);
        Physics.SyncTransforms();
        world.MergeObjects();
        world.DestoryObjects();
        t = 0.0f;

        yield return StartCoroutine(FadeMainDimension(true));
        material.SetFloat(dissolveName, 0.0f);
    }

    public IEnumerator RotationTransition(int dir) {
        VecDic startPos = new VecDic();
        QuaDic startRot = new QuaDic();
        foreach (Dimension.Color bc in Dimension.BaseColor) {
            world.Dims[bc].TargetAngle = (world.Dims[bc].TargetAngle + dir * 120.0f) % 360.0f;
            startPos.Add(bc, world.Dims[bc].transform.position);
            startRot.Add(bc, world.Dims[bc].transform.rotation);
        }

        float t = 0.0f;
        while (t < world.TransitionDur) {
            t += Time.deltaTime;
            float p = t / world.TransitionDur;
            foreach (Dimension.Color bc in Dimension.BaseColor) {
                var dimTran = world.Dims[bc].transform;
                dimTran.position = startPos[bc];
                dimTran.rotation = startRot[bc];
                dimTran.RotateAround(Vector3.zero, Vector3.up, dir * p * 120.0f);
            }
            yield return null;
        }
        foreach (Dimension.Color bc in Dimension.BaseColor) {
                var dimTran = world.Dims[bc].transform;
                dimTran.position = startPos[bc];
                dimTran.rotation = startRot[bc];
                dimTran.RotateAround(Vector3.zero, Vector3.up, dir * 120.0f);
            }

    }

    IEnumerator FadeMainDimension(bool active) {
        DimensionsSetActive(false);
        float t = 0.0f;
        float p;
        while (t < world.FadeDur) {
            t += Time.deltaTime;
            p = (active) ? (world.FadeDur - t) / world.FadeDur : t / world.FadeDur;
            material.SetFloat(dissolveName, p);
            yield return null;
        }
        p = (active) ? 0 : 1;
        material.SetFloat(dissolveName, p);
    }

    void DimensionsSetActive(bool active) {
        foreach (Dimension.Color bc in Dimension.BaseColor) {
            world.Dims[bc].gameObject.SetActive(active);
        }
    }

}