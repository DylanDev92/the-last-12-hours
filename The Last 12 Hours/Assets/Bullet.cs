using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Entity
{
    public override int attack => throw new System.NotImplementedException();
    
    protected override void Start()
    {
        Rigidbody2D brb = GetComponent<Rigidbody2D>();
        Vector2 direction = (Player.Instance.transform.position - transform.position).normalized;
        brb.AddForce(direction * 10, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Instance.ReceiveAttack(this, 1);
        }

        Destroy(this.gameObject);
    }
}
