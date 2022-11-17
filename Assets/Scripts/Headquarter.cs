using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headquarter : MonoBehaviour
{
    public Sprite good, wreck;
    SpriteRenderer sprite;
    public Side side = Side.Null;
    // Start is called before the first frame update
    void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.sprite = good;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        sprite.sprite = wreck;
        if (side == Side.Player)
        {
            StartCoroutine(BattleManager.GetInstance().Loss());
        }
        if (side == Side.Enemy) {
            StartCoroutine(BattleManager.GetInstance().Win());
        }
    }
    public enum Side { Null,Player,Enemy}
}
