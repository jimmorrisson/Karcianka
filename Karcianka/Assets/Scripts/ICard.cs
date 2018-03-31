using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface ICard
{
    string CardName { get; set; }
    string Description { get; set; }
    Sprite Image { get; set; }

    void PlayCard();
    void Attack();
}