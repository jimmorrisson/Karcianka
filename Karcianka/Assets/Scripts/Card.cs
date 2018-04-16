using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Draggable))]
public class Card : MonoBehaviour, IPointerClickHandler
{
    public CardCharacteristics card;

    public Text cardName;
    public Text description;
    public Image image;
    public Text health;
    public Text mana;
    public Text attack;
    public int cardHealth;
    public bool onTable = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CardsManager.Instance.selectedCard == null)
        {
            CardsManager.Instance.SelectCard(this);
        }
        else
        {
            if (CardsManager.Instance.selectedCard.onTable && this.onTable)
            {
                CardsManager.Instance.AttackCard(this);
                health.text = cardHealth.ToString();
            }
            Debug.Log("Can't attack card not on table");
            CardsManager.Instance.DiscardSelection();
        }
    }

    // Use this for initialization

    void OnValidate()
    {
        if (card == null) return;
        cardName.text = card.name;
        description.text = card.description;
        image.sprite = card.image;
        health.text = card.health.ToString();
        mana.text = card.mana.ToString();
        attack.text = card.attack.ToString();
        cardHealth = card.health;
}

    private void Update()
    {
        if (cardHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

}
