using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollision : MonoBehaviour
{
    public ActionSystem parent;
    public ActionSystem opponent;
    void Start()
    {
        parent = transform.GetComponentInParent<ActionSystem>();
    }

    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (parent != null)
        {
            opponent = parent.opponent;
            if (col.transform.IsChildOf(opponent.transform))
            {
                parent.Hited(col.tag);
            }
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (parent != null)
        {
            opponent = parent.opponent;
            if (parent.hurted && hit.point.y > .1f)
                opponent.pushDis = parent.velocity.x;
        }
    }
}
