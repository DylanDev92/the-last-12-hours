using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.XR;

public class Boss : Enemy
{
    private GameObject hand { get; set; }

    public GameObject Bullet;

    protected override void Start()
    {
        hand = transform.Find("Hand").gameObject;

        base.Start();
    }

    private void Update()
    {
        UpdateHand();
    }

    private void UpdateHand()
    {
        if (Player.Instance.transform is Transform transform)
        {
            hand.SetActive(isPlayerInAttackRange);
            if (!isPlayerInAttackRange)
            {
                return;
            }

            Vector3 mousePosition = transform.position;
            Vector3 direction = (mousePosition - this.transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            hand.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    protected override bool Attack()
    {
        GameObject.Instantiate(Bullet, this.transform.position, Quaternion.identity);

        return true;
    }
}
