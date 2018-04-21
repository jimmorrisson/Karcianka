using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class ActivateCards
{
    public static void EnableCardScript(this Card card, bool enable)
    {
        card.GetComponent<Draggable>().enabled = enable;
    }

    public static void ResetHasAttackedProperty(this Card card)
    {
        card.HasAttacked = false;
    }
}

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
    public Text currentTurn;
    public delegate void UnitEventHandler(GameObject unit);
    public delegate void DamageEventHandler(int amountOfDamage);
    public event DamageEventHandler OnHeroAttacked;

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
        if (Input.GetMouseButtonDown(0))
        {
            AttackHeroOnClicked();
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
        if (card.HasAttacked)
        {
            return;
        }
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
        if (card == selectedCard || CheckIfCardIsYours(card) || selectedCard.HasAttacked)
        {
            return;
        }
        card.cardHealth -= selectedCard.card.attack;
        selectedCard.HasAttacked = true;
        DiscardSelection();
        Debug.Log(card.card.health);
    }

    private bool CheckIfCardIsYours(Card card)
    {
        var selectedParent = Instance.selectedCard.transform.parent;
        var cardParent = card.transform.parent;

        if (selectedParent == cardParent)
        {
            return true;
        }
        return false;
    }
    public void PlayerTurn(int turn)
    {
        if (turn % 2 == 1)
        {
            currentTurn.text = "Player's 1 turn";
        }
        else
        {
            currentTurn.text = "Player's 2 turn";
        }
    }
    public void DrawCard(int turn)
    {
        if (deck.Count <= 0 || player1Hand.transform.childCount > 5 || player2Hand.transform.childCount > 5)
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

            var cardsToDisable = GetCardsInHand(player2Hand);
            var cardsToEnable = GetCardsInHand(player1Hand);
            var cardsToReset = GetCardsInHand(player1Table);

            FlipCards(cardsToDisable, false);
            FlipCards(cardsToEnable, true);

            ActivateCards(cardsToDisable, false);
            ActivateCards(cardsToEnable, true);

            ResetHasAttackedList(cardsToReset);
        }
        else
        {
            DisableDropZone(player1Hand);
            EnableDropZone(player2Hand);
            DisableDropZone(player1Table);
            EnableDropZone(player2Table);

            var cardsToEnable = GetCardsInHand(player2Hand);
            var cardsToDisable = GetCardsInHand(player1Hand);
            var cardsToReset = GetCardsInHand(player2Table);

            FlipCards(cardsToEnable, true);
            FlipCards(cardsToDisable, false);

            ActivateCards(cardsToDisable, false);
            ActivateCards(cardsToEnable, true);

            ResetHasAttackedList(cardsToReset);
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

    private List<Card> GetCardsInHand(GameObject otherHand)
    {
        List<Card> cards = new List<Card>();

        var list = otherHand.GetComponentsInChildren<Card>();

        foreach (var card in list)
        {
            cards.Add(card);
            Debug.Log(card.cardName.text);
        }

        return cards;
    }

    private void ResetHasAttackedList(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            card.ResetHasAttackedProperty();
        }
    }
    private void ActivateCards(List<Card> cards, bool active)
    {
        foreach (Card card in cards)
        {
            card.EnableCardScript(active);
        }
    }
    private void FlipCards(List<Card> cards, bool active)
    {
        foreach (Card card in cards)
        {
            card.transform.GetChild(0).gameObject.SetActive(active);
            if (card.transform.childCount == 2)
            {
                var backCard = card.transform.GetChild(1);
                backCard.gameObject.SetActive(!active);
            }
        }
    }

    private void AttackHeroOnClicked()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            LocalPlayer player = hit.collider.GetComponent<LocalPlayer>();
            if (player != null && selectedCard != null && selectedCard.HasAttacked == false && selectedCard.onTable == true)
            {
                player.TakeDamage(selectedCard.card.attack);
                selectedCard.HasAttacked = true;
                Instance.DiscardSelection();
            }
        }
    }

    private void HeroAttacked(int dmg)
    {
        if (selectedCard == null)
        {
            return;
        }
        if (OnHeroAttacked != null)
        {
            OnHeroAttacked(dmg);
        }
    }
}
