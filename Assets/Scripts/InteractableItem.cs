using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    [SerializeField]
    private int _highlightIntensity = 4;    
    private Outline _outline;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
    }

    public void SetFocus()
    {
        _outline.OutlineWidth = _highlightIntensity;
    }
    
    public void RemoveFocus()
    {
        _outline.OutlineWidth = 0;
    }
}