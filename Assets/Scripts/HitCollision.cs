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
        opponent = parent.opponent;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.IsChildOf(opponent.transform))
        {
            parent.Hited(col.tag);
        }
    }
}
