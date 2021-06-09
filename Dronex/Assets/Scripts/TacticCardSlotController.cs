using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TacticCardSlotController : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    public Material slotSelectedMaterial;
    public Material slotUnselectedMaterial;
    public Renderer slotRenderer;
    public TextMesh priorityTextMesh;
    public bool isSelected = false;

    private TacticCardController tcc;

    public void Start()
    {
        tcc = transform.parent.GetComponent<TacticCardController>();
    }

    public void Update()
    {
        if (isSelected)
        {
            slotRenderer.material = slotSelectedMaterial;
        }
        else
        {
            slotRenderer.material = slotUnselectedMaterial;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isSelected && tcc.unitMaxCountReached())
        {
            return;
        }

        isSelected = !isSelected;
        if (isSelected)
        {
            tcc.startPointerDrag(TacticCardController.DragActions.Select);
            onSelect();
        }
        else
        {
            tcc.startPointerDrag(TacticCardController.DragActions.Unselect);
            onDeselect();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        tcc.endPointerDrag();
    }

    public void onSelect()
    {
        tcc.addSelectedSlot(this);
    }

    public void onDeselect()
    {
        tcc.removeSelectedSlot(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected && tcc.unitMaxCountReached())
        {
            return;
        }

        if (tcc.pointerOnDrag())
        {
            if (tcc.getDragAction() == TacticCardController.DragActions.Select)
            {
                isSelected = true;
                onSelect();
            }
            else
            {
                isSelected = false;
                onDeselect();
            }
        }
    }

    public void setSlotPositionString(int pos)
    {
        priorityTextMesh.gameObject.SetActive(true);
        priorityTextMesh.text = pos.ToString();
    }

    public void deactiveSlotPositionString()
    {
        priorityTextMesh.gameObject.SetActive(false);
    }
}
