using System;
using UnityEngine;

[Serializable]
public class ObjectColor : MonoBehaviour {

    [SerializeField] Dimension.Color color;
    Renderer _renderer;
    MaterialPropertyBlock _propBlock;

    private void Awake() {
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
        _renderer.GetPropertyBlock(_propBlock);
    }
    public void SetColor(Dimension.Color color) {
        this.color = color;
        _propBlock.SetColor("_Color", Dimension.MaterialColor[color]);
        _renderer.SetPropertyBlock(_propBlock);
    }

    public Dimension.Color GetColor() {
        return color;
    }

}