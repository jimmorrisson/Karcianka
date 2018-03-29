using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Draggable))]
public class Card : MonoBehaviour {

    public CardCharacteristics card;

    public Text cardName;
    public Text description;
    public Image image;
    // Use this for initialization

    void OnValidate()
    {
        if (card == null) return;
        cardName.text = card.name;
        description.text = card.description;
        image.sprite = card.image;
    }

    
}
