using UnityEngine;

public class Bush : MonoBehaviour
{
    [SerializeField] private Material _opaqueMaterial; // Обычный материал
    [SerializeField] private Material _transparentMaterial; // Полупрозрачный
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.material = _opaqueMaterial;
    }

    public void SetTransparent(bool isTransparent)
    {
        _renderer.material = isTransparent ? _transparentMaterial : _opaqueMaterial;
    }
}