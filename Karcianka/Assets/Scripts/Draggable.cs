using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    Vector2 offset;
    [HideInInspector]
    public Transform parentToReturnTo = null;
    [HideInInspector]
    public Transform placeHolderParent = null;
    GameObject placeHolder;

    public PhotonView pView;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (pView.isMine)
        {
            offset = this.transform.position - (Vector3)eventData.position;

            placeHolder = new GameObject();
            placeHolder.transform.SetParent(this.transform.parent);
            RectTransform rt = placeHolder.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(this.transform.GetComponent<RectTransform>().sizeDelta.x, this.transform.GetComponent<RectTransform>().sizeDelta.y);

            placeHolder.transform.SetSiblingIndex(this.transform.GetSiblingIndex());

            parentToReturnTo = this.transform.parent;
            placeHolderParent = parentToReturnTo;
            this.transform.SetParent(this.transform.root);

            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (pView.isMine)
        {
            this.transform.position = eventData.position + offset;

            if (placeHolder.transform.parent != placeHolderParent)
                placeHolder.transform.SetParent(placeHolderParent);

            int newSiblingIndex = placeHolderParent.childCount;
            for (int i = 0; i < placeHolderParent.childCount; i++)
            {
                if (this.transform.position.x < placeHolderParent.GetChild(i).position.x)
                {
                    newSiblingIndex = i;
                    if (placeHolder.transform.GetSiblingIndex() < newSiblingIndex)
                        newSiblingIndex--;
                    break;
                }
            }
            placeHolder.transform.SetSiblingIndex(newSiblingIndex);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (pView.isMine)
        {
            this.transform.SetParent(parentToReturnTo);
            this.transform.SetSiblingIndex(placeHolder.transform.GetSiblingIndex());
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            Destroy(placeHolder);
        }
    }
}
