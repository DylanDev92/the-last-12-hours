using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mafia : Enemy
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    //public override int attack => 10;

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawLine(player.position, this.position, Color.magenta, 1);
        // 170 - 190
        // 350 - 10
        //Debug.Log(GetAngle(player.position, this.position));

        //Debug.Log("distance: " + Vector2.Distance(Player.Instance.position, this.position));
    }

    //public override int attack => 0; // testing purposes

    protected override void DropReward()
    {
        base.DropReward();
    }

    protected override bool Attack()
    {
        //
        // it's kind of weird to let the mafia attack at awkward angles (0 to 360)
        // so instead, i restricted it's attacks to specific angle ranges
        // if not within that range, it tries to adjust itself
        // so that it will reach that range
        //

        var angle = GetAngle(player.position, this.position);
        if (IsInAngleRange(340, 25, angle) || IsInAngleRange(165, 195, angle))
        {
            return base.Attack();
        }
        else
        {
            var direction = (player.position - this.position).normalized;
            if (angle >= 180 && angle <= 360) // player is below
            {
                //Debug.Log("Adjusting down");
                ApplyMovement(Vector2.down + direction * 0.01f);
            }
            else // (0 to 180)
            {
                //Debug.Log("Adjusting up");
                ApplyMovement(Vector2.up + direction * 0.01f);
            }
        }

        return false;
    }

}
