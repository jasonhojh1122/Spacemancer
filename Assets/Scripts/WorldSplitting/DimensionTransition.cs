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
    [SerializeField] PlayerController playerController;
    Camera cam;
    Dimension.Color activeColor;
    public bool Transitting;
    static string dissolveName = "_Dissolve";

    void Start() {
        Init();
    }

    public void Init() {
        activeColor = Dimension.Color.WHITE;
        float rot = 0f;
        foreach (Dimension.Color bc in Dimension.BaseColor) {
            world.Dims[bc].TargetAngle = rot;
            rot += 120f;
        }
        world.Dims[Dimension.Color.WHITE].TargetAngle = 0f;
        material.SetFloat(dissolveName, 0.0f);
        cam = Camera.main;
    }

    public IEnumerator SplitTransition() {
        OnTransitionStartEnd(true);

        // Fade out the main dimension
        yield return StartCoroutine(FadeMainDimension(false));

        // Perform actual splitting
        world.SplitObjects();
        world.DestoryObjects();
        world.Dims[Dimension.Color.WHITE].gameObject.SetActive(false);

        DimensionsSetActive(true);

        // Calculate the target position
        VecDic targetPos = new VecDic();
        foreach (Dimension.Color bc in Dimension.BaseColor) {
            Vector3 VecDic = new Vector3(0, 0, -world.Radius);
            Quaternion rot = Quaternion.AngleAxis(world.Dims[bc].TargetAngle, Vector3.up);
            targetPos.Add(bc, rot * VecDic);
            if (Fuzzy.CloseFloat(world.Dims[bc].TargetAngle, 0.0f)) {
                activeColor = bc;
                MoveCamera();
            }
        }

        // Gradully move the dimensions to target position
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
        // Set the final pos/rot of dimensions
        foreach (Dimension.Color bc in Dimension.BaseColor) {
            var dim = world.Dims[bc];
            dim.transform.position = targetPos[bc];
            dim.transform.rotation = Quaternion.identity;
            dim.transform.RotateAround(dim.transform.position, Vector3.up, dim.TargetAngle);
        }
        Physics.SyncTransforms();

        material.SetFloat(dissolveName, 0.0f);
        OnTransitionStartEnd(false);
    }

    public IEnumerator MergeTransition() {
        OnTransitionStartEnd(true);

        // Save the start status of dimension
        VecDic startPos = new VecDic();
        QuaDic startRot = new QuaDic();
        foreach (Dimension.Color bc in Dimension.BaseColor) {
            startRot.Add(bc, world.Dims[bc].transform.rotation);
            startPos.Add(bc, world.Dims[bc].transform.position);
        }

        // Gradully move the dimensions to center
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
        // Set the final pos/rot of dimensions
        foreach (Dimension.Color bc in Dimension.BaseColor) {
            var dim = world.Dims[bc];
            dim.transform.position = Vector3.zero;
            dim.transform.rotation = Quaternion.identity;
        }
        Physics.SyncTransforms();

        world.Dims[Dimension.Color.WHITE].gameObject.SetActive(true);
        activeColor = Dimension.Color.WHITE;
        MoveCamera();

        // Perform actual merge of objects
        world.MergeObjects();
        world.DestoryObjects();

        DimensionsSetActive(false);
        yield return StartCoroutine(FadeMainDimension(true));
        OnTransitionStartEnd(false);
    }

    // Rotate the splitted dimensions
    public IEnumerator RotationTransition(int dir) {
        OnTransitionStartEnd(true);

        // Save the start status of dimension
        VecDic startPos = new VecDic();
        QuaDic startRot = new QuaDic();
        foreach (Dimension.Color bc in Dimension.BaseColor) {
            world.Dims[bc].TargetAngle = (world.Dims[bc].TargetAngle + dir * 120.0f) % 360.0f;
            startPos.Add(bc, world.Dims[bc].transform.position);
            startRot.Add(bc, world.Dims[bc].transform.rotation);
            if (Fuzzy.CloseFloat(world.Dims[bc].TargetAngle, 0.0f)) {
                activeColor = bc;
            }
        }

        // Gradully rotate the dimensions
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

        // Set the final status of dimensions
        foreach (Dimension.Color bc in Dimension.BaseColor) {
            var dimTran = world.Dims[bc].transform;
            dimTran.position = startPos[bc];
            dimTran.rotation = startRot[bc];
            dimTran.RotateAround(Vector3.zero, Vector3.up, dir * 120.0f);
        }
        MoveCamera();
        OnTransitionStartEnd(false);
    }

    // Gradully fade in/out main dimension
    IEnumerator FadeMainDimension(bool active) {
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

    // Set base color dimensions to active
    void DimensionsSetActive(bool active) {
        foreach (Dimension.Color bc in Dimension.BaseColor) {
            world.Dims[bc].gameObject.SetActive(active);
        }
    }

    void OnTransitionStartEnd(bool isStart) {
        Physics.gravity = isStart ? Vector3.zero : new Vector3(0f, -9.8f, 0f);
        Physics.autoSimulation = !isStart;
        playerController.paused = isStart;
        Transitting = isStart;
    }

    void MoveCamera() {
        Vector3 localPos = cam.transform.localPosition;
        Quaternion localRot = cam.transform.localRotation;
        cam.transform.SetParent(world.Dims[activeColor].transform);
        cam.transform.localPosition = localPos;
        cam.transform.localRotation = localRot;
    }

}