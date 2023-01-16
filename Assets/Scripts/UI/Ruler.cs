using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

public class Ruler : MonoBehaviour
{
    public GameObject rulerCanvas;
    
    [Header("General Rulers infos")]
    [SerializeField] private float numbersDistance = 30f;
    [SerializeField] private float spriteWidth = 6000f;
    [SerializeField] private float spriteOffset = 3000f;

    [Header("Top Ruler infos")]
    [SerializeField] private RawImage rulerTopImage;
    [SerializeField] private Image topRulerImageFond;
    [SerializeField] private int topRulerNumbersShown = 8;
    [SerializeField] private float cameraGridOffsetX;
    
    [Header("Side Ruler Infos")]
    [SerializeField] private RawImage rulerSideImage;
    [SerializeField] private Image sideRulerImageFond;
    [SerializeField] private int sideRulerNumbersShown = 8;
    [SerializeField] private float cameraGridOffsetY;
    // [SerializeField] private float rulerSideWidthOffset = 0f;

    private Vector3 _cameraPos = Vector3.zero;
    private Camera _mainCameraReference;
    private bool _haveCamera;

    /*private GameObject _player;
    private Vector3 _initialTopRulerPosition;
    private Vector3 _initialTopRulerOnScreenPosition;*/
    
    private float _deltaPTop;
    private float _deltaPSide;

    private void Start()
    {
        Debug.Assert(Camera.main != null, "Ruler.cs : Camera.main != null");
        Debug.Assert(rulerTopImage != null, "Ruler.cs : rulerTopImage != null");
        Debug.Assert(rulerSideImage != null, "Ruler.cs : rulerSideImage != null");
        
        // we have to make referenced of these because we'll use them later and accessing these at run-time takes too muc
        _mainCameraReference = Camera.main;
        _cameraPos = _mainCameraReference.transform.position;
        _haveCamera = true;
        // _player = GameObject.FindGameObjectWithTag("Player");
        
        // _initialTopRulerPosition = rulerTopImage.transform.position;
    }

    private void FixedUpdate()
    {
        // check what space is between two graduations on the real world, converted to screen scale
        _deltaPTop = Mathf.Abs(_mainCameraReference.WorldToScreenPoint(_cameraPos).x - _mainCameraReference.WorldToScreenPoint(_cameraPos + Vector3.left).x);
        _deltaPSide = Mathf.Abs(_mainCameraReference.WorldToScreenPoint(_cameraPos).y - _mainCameraReference.WorldToScreenPoint(_cameraPos + Vector3.down).y);
        
        MoveRuler();
    }
    
    private void MoveRuler()
    {
        // if you dont have no camera just dont move the rulers lmao
        if (!_haveCamera) return;
        
        // take the new camera position
        var newCameraPos = _mainCameraReference.transform.position;
        
        var topRulerWidth = topRulerNumbersShown * _deltaPTop;
        rulerTopImage.rectTransform.sizeDelta = new Vector2(topRulerWidth, rulerTopImage.rectTransform.rect.height);

        /*var playerPos = _mainCameraReference.WorldToScreenPoint(_player.transform.position);
        _initialTopRulerOnScreenPosition = _mainCameraReference.WorldToScreenPoint(_initialTopRulerPosition);

        rulerTopImage.transform.position = new Vector3(_initialTopRulerPosition.x + _initialTopRulerOnScreenPosition.x - playerPos.x, rulerTopImage.transform.position.y, 0);*/

        // first we do the top ruler stuff (good soup)
        // calculate the width percentage for the top image uvRect, depends on (by order of apparition) :
        // - the top image width
        // - the distance between two numbers on the sprite
        // - the total sprite width
        // - the distance between two points on the grid scaled to the screen scale 
        var wTop = rulerTopImage.rectTransform.rect.width * numbersDistance / (spriteWidth * _deltaPTop); 
        
        // then we calculate what is the default position (when camera is at position 0,0) with : 
        // - the position of the 0 on the sprite
        // - the sprite width
        // - the width percentage we just calculated divided by two
        var x0 = spriteOffset / spriteWidth - wTop / 2;
        
        // after that we calculate the actual x we have to input into the RawImage for it to be centered
        // around the right coordinate at runtime, at any moment, with : 
        // - the default position we just calculated
        // - the distance between two numbers on the sprite
        // - the camera's x position 
        // - the grid offset (the center of a grid tile is not a round number) 
        // - the sprite width
        var x = x0 + numbersDistance * (_cameraPos.x + cameraGridOffsetX) / spriteWidth;
        // var x = x0 + numbersDistance * (_player.transform.position.x + cameraGridOffsetX) / spriteWidth;
        
        // finally we input the x and the width percentage we just calculated
        rulerTopImage.uvRect = new Rect(x, rulerTopImage.uvRect.y, wTop, rulerTopImage.uvRect.height);

        // set the fond to be at the size of the ruler
        topRulerImageFond.transform.position = rulerTopImage.transform.position;
        topRulerImageFond.rectTransform.sizeDelta = rulerTopImage.rectTransform.sizeDelta;

        
        // resize the side ruler using the screen height
        // sizeDelta = new Vector2(Screen.height - rulerSideWidthOffset, sizeDelta.y);
        // rulerSideImage.rectTransform.sizeDelta = sizeDelta;
        

        var sideRulerWidth = sideRulerNumbersShown * _deltaPSide;
        var rect = rulerSideImage.rectTransform.rect;
        rulerSideImage.rectTransform.sizeDelta = new Vector2(sideRulerWidth, rect.height);

        // then we do the side ruler stuff (good soup now, these are the same steps as right before)
        var wSide = rect.width * numbersDistance / (spriteWidth * _deltaPSide);
        var y0 = spriteOffset / spriteWidth - wSide/2;
        var y = y0 + numbersDistance * (_cameraPos.y + cameraGridOffsetY) / spriteWidth;
        rulerSideImage.uvRect = new Rect(y, rulerSideImage.uvRect.y, wSide, rulerSideImage.uvRect.height);

        
        // adjust size ruler fond
        sideRulerImageFond.transform.position = rulerSideImage.transform.position;
        sideRulerImageFond.rectTransform.sizeDelta = rulerSideImage.rectTransform.sizeDelta;
        
        _cameraPos = newCameraPos;
    }
    
}
