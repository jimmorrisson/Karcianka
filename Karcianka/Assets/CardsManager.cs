using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardsManager : MonoBehaviour
{
    private Vector3 attackedCardPosition;
    [SerializeField]
    private bool attacking = false;
    [SerializeField]
    private Vector3 startPos;
    public Card selectedCard;

    #region Singleton
    public static CardsManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && selectedCard != null)
        {
            DiscardSelection();
        }
    }

    //private void AttackAnimation()
    //{
    //    Instance.selectedCard.transform.position = Vector3.Lerp(startPos, attackedCardPosition, Time.deltaTime);
    //   // Instance.selectedCard.transform.position = Vector3.Lerp(Instance.selectedCard.transform.position, startPos, Time.deltaTime);
    //}
    public void ChangeColorOfSelectedCard(Color color)
    {
        selectedCard.GetComponent<Image>().color = color;
    }

    public void SelectCard(Card card)
    {
        if (Instance.selectedCard != null)
        {
            DiscardSelection();
        }
        CardsManager.Instance.selectedCard = card;
        ChangeColorOfSelectedCard(Color.green);
    }

    public void DiscardSelection()
    {
        if (Instance.selectedCard != null)
        {
            ChangeColorOfSelectedCard(Color.black);
            Instance.selectedCard = null;
        }
    }
    public void AttackCard(Card card)
    {
        card.cardHealth -= selectedCard.card.attack;
        DiscardSelection();
        Debug.Log(card.card.health);
    }
}
