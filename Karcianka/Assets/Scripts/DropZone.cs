using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(HorizontalLayoutGroup))]
public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

    public Color hoverColor = new Color(.6f, .6f, .6f, 0.5f);
    Color mainColor;
    Image image;

    void Reset()
    {
        HorizontalLayoutGroup h = GetComponent<HorizontalLayoutGroup>();
        h.childForceExpandHeight = false;
        h.childForceExpandWidth = false;
        h.childAlignment = TextAnchor.MiddleCenter;
    }

    void Start()
    {
        image = GetComponent<Image>();
        mainColor = image.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        image.color = hoverColor;

        if (eventData.pointerDrag == null) return;

        Draggable draggable = eventData.pointerDrag.GetComponent<Draggable>();
        if (draggable != null)
            draggable.placeHolderParent = this.transform;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Draggable draggable = eventData.pointerDrag.GetComponent<Draggable>();
        if (draggable != null)
            draggable.parentToReturnTo = this.transform;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = mainColor;

        if (eventData.pointerDrag == null) return;

        Draggable draggable = eventData.pointerDrag.GetComponent<Draggable>();
        if (draggable != null && draggable.placeHolderParent == this.transform)
            draggable.placeHolderParent = draggable.parentToReturnTo;
    }

}
