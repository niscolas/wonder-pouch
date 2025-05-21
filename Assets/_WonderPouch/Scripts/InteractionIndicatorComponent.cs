using System.Collections.Generic;
using UnityEngine;

public class InteractionIndicatorComponent : MonoBehaviour
{
    [SerializeField] private Material _outlineMaterial;

    private List<Renderer> _meshRenderers;

    private void Start()
    {
        _meshRenderers = new List<Renderer>(GetComponentsInChildren<Renderer>());

    }

    public void ShowInteractionIndication()
    {
        if (!_outlineMaterial)
        {
            return;
        }

        foreach (var renderer in _meshRenderers)
        {
            var materialsList = new List<Material>(renderer.materials);
            if (!materialsList.Contains(_outlineMaterial))
            {
                materialsList.Add(_outlineMaterial);
                renderer.materials = materialsList.ToArray();
            }
        }
    }

    public void HideInteractionIndication()
    {
        if (!_outlineMaterial)
        {
            return;
        }

        foreach (var currentRenderer in _meshRenderers)
        {
            var materialsList = new List<Material>(currentRenderer.materials);
            materialsList.RemoveAt(1);
            currentRenderer.materials = materialsList.ToArray();
        }
    }
}
