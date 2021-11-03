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
    public bool Transitting;

    void Start() {
        Init();
    }

    public void Init() {
        float rot = 0f;
        foreach (Dimension.Color bc in Dimension.BaseColor) {
            world.Dims[bc].TargetAngle = rot;
            rot += 120f;
        }
        world.Dims[Dimension.Color.WHITE].TargetAngle = 0f;
        material.SetFloat(dissolveName, 0.0f);
    }

    public IEnumerator SplitTransition() {
        Transitting = true;
        SetPhysics(false);

        // Fade out the main dimension
        yield return StartCoroutine(FadeMainDimension(false));

        // Perform actual splitting
        world.SplitObjects();
        world.DestoryObjects();

        // Set active of base color dimensions
        DimensionsSetActive(true);

        // Move the camera together
        StartCoroutine(cameraTransition.Transition());

        // Calculate the target position
        VecDic targetPos = new VecDic();
        foreach (Dimension.Color bc in Dimension.BaseColor) {
            Vector3 VecDic = new Vector3(0, 0, -world.Radius);
            Quaternion rot = Quaternion.AngleAxis(world.Dims[bc].TargetAngle, Vector3.up);
            targetPos.Add(bc, rot * VecDic);
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
        SetPhysics(true);
        Transitting = false;
    }

    public IEnumerator MergeTransition() {
        Transitting = true;
        SetPhysics(false);

        // Move the camera together
        StartCoroutine(cameraTransition.Transition());

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

        // Perform actual merge of objects
        world.Dims[Dimension.Color.WHITE].gameObject.SetActive(true);
        Physics.SyncTransforms();
        world.MergeObjects();
        world.DestoryObjects();

        // Fade in the main dimension
        yield return StartCoroutine(FadeMainDimension(true));
        SetPhysics(true);
        Transitting = false;
    }

    // Rotate the splitted dimensions
    public IEnumerator RotationTransition(int dir) {
        Transitting = true;
        SetPhysics(false);

        // Save the start status of dimension
        VecDic startPos = new VecDic();
        QuaDic startRot = new QuaDic();
        foreach (Dimension.Color bc in Dimension.BaseColor) {
            world.Dims[bc].TargetAngle = (world.Dims[bc].TargetAngle + dir * 120.0f) % 360.0f;
            startPos.Add(bc, world.Dims[bc].transform.position);
            startRot.Add(bc, world.Dims[bc].transform.rotation);
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
        SetPhysics(true);
        Transitting = false;
    }

    // Gradully fade in/out main dimension
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

    // Set base color dimensions to active
    void DimensionsSetActive(bool active) {
        foreach (Dimension.Color bc in Dimension.BaseColor) {
            world.Dims[bc].gameObject.SetActive(active);
        }
    }

    // Set physics simulation on or off
    void SetPhysics(bool state) {
        if (state)
            Physics.gravity = new Vector3(0f, -9.8f, 0f);
        else
            Physics.gravity = Vector3.zero;
        Physics.autoSimulation = state;
    }

}