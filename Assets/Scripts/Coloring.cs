using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coloring : MonoBehaviour
{
    [SerializeField] private Color _color;
    [SerializeField] private Renderer _renderer;
    private const float threshhold = 0.1f;
    private const float mergeDistance = 1f;
    public Color color
    {
        get => _color;
        set
        {
            _color = value;
            foreach(Material mat in _renderer.materials)
            {
                mat.color = _color;
            }
        }
    }
    void OnMouseDown()
    {
        StartCoroutine(SplitNMerge());

    }
    IEnumerator SplitNMerge()
    {
        List<GameObject> rgbObject = Split();
        List<GameObject> r = new List<GameObject>();
        List<GameObject> g = new List<GameObject>();
        List<GameObject> b = new List<GameObject>();
        r.Add(rgbObject[0]);
        g.Add(rgbObject[1]);
        b.Add(rgbObject[2]);
        yield return new WaitForSeconds(3);
        Debug.Log("Merge");
        Merge(new List<GameObject>[] { r, g, b, });
    }

    public List<GameObject> Split() // returns list[3] : r,g,b object. null if no such color
    {
        bool split = false;
        List<GameObject> rgbObject = new List<GameObject>();
        Debug.Log(_color);
        if (_color.r > threshhold)
        {
            GameObject red = Instantiate(gameObject, gameObject.transform.position, Quaternion.identity);
            Coloring coloring = red.GetComponent(typeof(Coloring)) as Coloring;
            coloring.color = new Color(_color.r, 0, 0);
            red.transform.parent = gameObject.transform.parent;
            rgbObject.Add(red);
            split = true;
        }
        else
        {
            rgbObject.Add(null);
        }
        if (_color.g > threshhold)
        {
            GameObject green = Instantiate(gameObject, gameObject.transform.position, Quaternion.identity);
            Coloring coloring = green.GetComponent(typeof(Coloring)) as Coloring;
            coloring.color = new Color(0, _color.g, 0);
            green.transform.parent = gameObject.transform.parent;
            rgbObject.Add(green);
            split = true;
        }
        else
        {
            rgbObject.Add(null);
        }
        if (_color.b > threshhold)
        {
            GameObject blue = Instantiate(gameObject, gameObject.transform.position, Quaternion.identity);
            Coloring coloring = blue.GetComponent(typeof(Coloring)) as Coloring;
            coloring.color = new Color(0, 0, _color.b);
            blue.transform.parent = gameObject.transform.parent;
            rgbObject.Add(blue);
            split = true;
        }
        else
        {
            rgbObject.Add(null);
        }
        if (split)
        {
            gameObject.SetActive(false);
            //_renderer.enabled = false;
        }
        return rgbObject;
    }

    public List<GameObject> Merge(List<GameObject>[] objects) //merge objects from r,g,b scene 
    {
        for (int mergeType = 0; mergeType < 3; mergeType++) //merge r,g -> g,b -> b,r
        {
            int colorA = mergeType;
            int colorB = (mergeType + 1) % 3;
            for (int i = 0; i < objects[colorA].Count; i++)
            {
                for (int j = 0; j < objects[colorB].Count; j++)
                {
                    GameObject A = objects[colorA][i];
                    GameObject B = objects[colorB][j];
                    if (SameType(A, B) &&
                        Vector2.Distance(A.transform.position, B.transform.position) < mergeDistance)
                    {
                        // A becomes merged object; B destroyed
                        A.transform.position = (A.transform.position + B.transform.position) / 2;
                        Coloring Acoloring = A.GetComponent(typeof(Coloring)) as Coloring;
                        Coloring Bcoloring = B.GetComponent(typeof(Coloring)) as Coloring;
                        Acoloring.color += Bcoloring.color;
                        objects[colorB].RemoveAt(j);
                        Destroy(B);
                    }
                }
            }
        }
        objects[0].AddRange(objects[1]);
        objects[0].AddRange(objects[2]);
        return objects[0];
    }
    private bool SameType(GameObject a, GameObject b)
    {
        if (a == null || b == null)
            return false;
        return a.name == b.name;
    }
}
