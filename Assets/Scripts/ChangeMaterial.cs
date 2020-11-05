using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    public Material realityMaterial;
    public Material labellingMaterial;

    public void FromRealToLabelling()
    {
        Renderer _renderer = GetComponent<Renderer>();
        if (_renderer.sharedMaterial == realityMaterial)
        {
            _renderer.sharedMaterial = labellingMaterial;
        }
    }

    private void FromRealToLabelling(Renderer _renderer)
    {
        _renderer.sharedMaterial = labellingMaterial;
    }

    public void FromLabellingToReal()
    {
        Renderer _renderer = GetComponent<Renderer>();
        if (_renderer.sharedMaterial == labellingMaterial)
        {
            _renderer.sharedMaterial = realityMaterial;
        }
    }

    private void FromLabellingToReal(Renderer _renderer)
    {
        _renderer.sharedMaterial = realityMaterial;
    }

    public void SwitchMaterial()
    {
        Renderer _renderer = GetComponent<Renderer>();
        
        if (_renderer.sharedMaterial == labellingMaterial)
        {
            FromLabellingToReal(_renderer);
        }

        else
        {
            FromRealToLabelling(_renderer);
        }
    }

    public void DontCastShadow()
    {
        MeshRenderer _meshRenderer = GetComponent<MeshRenderer>();

        if (_meshRenderer != null)
        {
            _meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }

    public void CastShadow()
    {
        MeshRenderer _meshRenderer = GetComponent<MeshRenderer>();

        if (_meshRenderer != null)
        {
            _meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }
}
