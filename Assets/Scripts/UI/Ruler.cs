using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

public class Ruler : MonoBehaviour
{
    [Header("General Rulers infos")]
    [SerializeField] private float numbersDistance = 30f;
    [SerializeField] private float spriteWidth = 6000f;
    [SerializeField] private float spriteOffset = 3000f;

    [Header("Top Ruler infos")]
    [SerializeField] private RawImage rulerTopImage;
    [SerializeField] private float cameraGridOffsetX = -0.5f;
    
    [Header("Side Ruler Infos")]
    [SerializeField] private RawImage rulerSideImage;
    [SerializeField] private float cameraGridOffsetY = -0.5f;
    
    private Vector3 _cameraPos = Vector3.zero;
    private Camera _mainCameraReference;
    private bool _haveCamera;
    private float _deltaPTop; 

    private void Start()
    {
        Debug.Assert(Camera.main != null, "Ruler.cs : Camera.main != null");
        Debug.Assert(rulerTopImage != null, "Ruler.cs : rulerTopImage != null");
        Debug.Assert(rulerSideImage != null, "Ruler.cs : rulerSideImage != null");
        
        _mainCameraReference = Camera.main;
        _cameraPos = _mainCameraReference.transform.position;
        _haveCamera = true;

        UnityEngine.Debug.Log(_deltaPTop);
    }

    private void FixedUpdate()
    {
        _deltaPTop = Mathf.Abs(_mainCameraReference.WorldToScreenPoint(_cameraPos).x - _mainCameraReference.WorldToScreenPoint(_cameraPos + Vector3.left).x);
        MoveRuler();
    }
    
    private void MoveRuler()
    {
        if (!_haveCamera) return;
        var newCameraPos = _mainCameraReference.transform.position;
        
        // first we do the top ruler stuff (good soup)
        var w = rulerTopImage.rectTransform.rect.width * numbersDistance / (spriteWidth * _deltaPTop); 
        
        var x0 = (spriteOffset + w * spriteWidth / 2) / spriteWidth;
        var x = x0 + numbersDistance * (_cameraPos.x + cameraGridOffsetX) / spriteWidth;
        rulerTopImage.uvRect = new Rect(x, rulerTopImage.uvRect.y, w, rulerTopImage.uvRect.height);   
        
        
        
        // then we do the side ruler stuff (not so good soup rn)
        var sideRIuvRect = rulerSideImage.uvRect;
        
        /*var sideRectCalculus = (_cameraPos.y + cameraGridOffsetY) * moveImageSideUVRectFactor + rawImageSideUVRectOffset;
        rulerSideImage.uvRect = new Rect(sideRectCalculus, sideRIuvRect.y, sideRIuvRect.width, sideRIuvRect.height);*/

        var y0 = spriteOffset / spriteWidth + sideRIuvRect.width / 2;
        var y = y0 + numbersDistance * (_cameraPos.y + cameraGridOffsetY) / spriteWidth;
        rulerSideImage.uvRect = new Rect(new Vector2(y, rulerSideImage.uvRect.y), rulerSideImage.uvRect.size);
        
        _cameraPos = newCameraPos;
    }
    
}
