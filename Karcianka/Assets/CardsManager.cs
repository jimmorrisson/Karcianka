using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardsManager : Photon.PunBehaviour
{
    private Vector3 attackedCardPosition;
    [SerializeField]
    private bool attacking = false;
    [SerializeField]
    private Vector3 startPos;

    public GameObject player1Hand;
    public GameObject player2Hand;
    public GameObject player1Table;
    public GameObject player2Table;
    public Card selectedCard;
    public GameObject cardPrefab;
    public List<GameObject> deck = new List<GameObject>();

    #region Singleton
    public static CardsManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            deck.Add(Instantiate(cardPrefab) as GameObject);
        }
    }

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
        if (card == selectedCard)
        {
            return;
        }
        card.cardHealth -= selectedCard.card.attack;
        DiscardSelection();
        Debug.Log(card.card.health);
    }

    public void DrawCard(int turn)
    {
        if (deck.Count <= 0)
        {
            return;
        }
        if (turn % 2 == 1)
        {
            GameObject card = Instantiate(deck[deck.Count - 1], player1Hand.transform);
            deck.Remove(card);
        }
        else
        {
            GameObject card = Instantiate(deck[deck.Count - 1], player2Hand.transform);
            deck.Remove(card);
        }
    }

    public void DisablePanel(int turn)
    {
        if (turn % 2 == 1)
        {
            DisableDropZone(player2Hand);
            EnableDropZone(player1Hand);
            DisableDropZone(player2Table);
            EnableDropZone(player1Table);
        }
        else
        {
            DisableDropZone(player1Hand);
            EnableDropZone(player2Hand);
            DisableDropZone(player1Table);
            EnableDropZone(player2Table);
        }
    }
    private void DisableDropZone(GameObject droppablePanel)
    {
        if (droppablePanel.GetComponent<DropZone>() != null)
        {
            droppablePanel.GetComponent<DropZone>().enabled = false;
        }
    }

    private void EnableDropZone(GameObject droppablePanel)
    {
        if (droppablePanel.GetComponent<DropZone>() != null)
        {
            droppablePanel.GetComponent<DropZone>().enabled = true;
        }
    }
}
