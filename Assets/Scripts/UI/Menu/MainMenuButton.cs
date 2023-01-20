using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private MainMenuScript mainMenuScript;

    [SerializeField] private int n;

    private bool dragging = false;
    private Vector2 offset = Vector2.zero;

    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private EventSystem eventSystem;
    public UnityEvent onClick;

    void Start()
    {
        if (TryGetComponent(out Image imageComponent) && imageComponent.material != null)
        {
            imageComponent.material.SetInteger("_state", 0);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 1)
        {
            Select();
        }
        else if (eventData.clickCount >= 2)
        {
            onClick.Invoke();
        }
    }

    void Update()
    {
        if (TryGetComponent(out Image imageComponent) && imageComponent.material != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PointerEventData m_PointerEventData = new PointerEventData(eventSystem);
                m_PointerEventData.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();
                
                graphicRaycaster.Raycast(m_PointerEventData, results);
                results = results.Where(res => res.gameObject.TryGetComponent(out MainMenuButton _)).ToList();
                if (results.Count > 0 && results[0].gameObject == gameObject)
                {
                    Vector2 localMousePos = imageComponent.rectTransform.InverseTransformPoint(Input.mousePosition);
                    offset = localMousePos;
                    dragging = true;
                }
                else
                {
                    UnSelect();
                }
            }
        }

        if (dragging)
        {
            transform.position = (Vector2) Input.mousePosition - offset;
            transform.position = new Vector2(Mathf.Clamp(transform.position.x, 0, Screen.width),
                Mathf.Clamp(transform.position.y, 0, Screen.height));

            if (Input.GetMouseButtonUp(0))
            {
                dragging = false;
                offset = Vector2.zero;
            }
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (TryGetComponent(out Image imageComponent) && imageComponent.material != null)
        {
            if (imageComponent.material.GetInteger("_state") == 0)
            {
                imageComponent.material.SetInteger("_state", 1);
            }
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (TryGetComponent(out Image imageComponent) && imageComponent.material != null)
        {
            if (imageComponent.material.GetInteger("_state") == 1)
            {
                imageComponent.material.SetInteger("_state", 0);
            }
        }
    }

    public void Select()
    {
        mainMenuScript.Selected(n);
        if (TryGetComponent(out Image imageComponent))
        {
            imageComponent.material.SetInteger("_state", 2);
        }
    }

    public void UnSelect()
    {
        if (TryGetComponent(out Image imageComponent))
        {
            if (imageComponent.material.GetInteger("_state") == 2)
            {
                imageComponent.material.SetInteger("_state", 0);
            }
        }
    }
}
