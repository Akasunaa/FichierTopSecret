using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

public class Ruler : MonoBehaviour
{
    
    [Header("Top Ruler infos")]
    [SerializeField] private RawImage rulerTopImage;
    [SerializeField] private float cameraGridOffsetX = -0.5f;
    [SerializeField] private float rawImageTopUVRectOffset = -0.1174f;
    [SerializeField] private float moveImageTopUVRectFactor = 0.02414f;
    
    [Header("Side Ruler Infos")]
    [SerializeField] private RawImage rulerSideImage;
    [SerializeField] private float cameraGridOffsetY = -0.5f;
    [SerializeField] private float rawImageSideUVRectOffset = -0.125f;
    [SerializeField] private float moveImageSideUVRectFactor = 0.04799f;
    
    private Vector3 _cameraPos = Vector3.zero;
    private Camera _mainCameraReference;
    private bool _haveCamera;

    private void Start()
    {

        Debug.Assert(Camera.main != null, "Ruler.cs : Camera.main != null");
        Debug.Assert(rulerTopImage != null, "Ruler.cs : rulerTopImage != null");
        Debug.Assert(rulerSideImage != null, "Ruler.cs : rulerSideImage != null");
        
        _mainCameraReference = Camera.main;
        _cameraPos = _mainCameraReference.transform.position;
        _haveCamera = true;
    }

    private void FixedUpdate()
    {
        MoveRuler();
    }
    
    private void MoveRuler()
    {
        if (!_haveCamera) return;
        var newCameraPos = _mainCameraReference.transform.position;
        
        // first we do the top ruler stuff (good soup)
        var topRIuvRect = rulerTopImage.uvRect;
        var topRectCalculus = (_cameraPos.x + cameraGridOffsetX) * moveImageTopUVRectFactor + rawImageTopUVRectOffset;
        rulerTopImage.uvRect = new Rect(topRectCalculus, topRIuvRect.y, topRIuvRect.width, topRIuvRect.height);
        
        // then we do the side ruler stuff (not so good soup rn)
        var sideRIuvRect = rulerSideImage.uvRect;
        var sideRectCalculus = (_cameraPos.y + cameraGridOffsetY) * moveImageSideUVRectFactor + rawImageSideUVRectOffset;
        rulerSideImage.uvRect = new Rect(sideRectCalculus, sideRIuvRect.y, sideRIuvRect.width, sideRIuvRect.height);
        
        _cameraPos = newCameraPos;
    }
    
}
