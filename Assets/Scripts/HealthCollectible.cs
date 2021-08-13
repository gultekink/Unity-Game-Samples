using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;


public class HealthCollectible : MonoBehaviour
{
    public AudioClip collectedClip;

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController rubyController = other.GetComponent<RubyController>();
             
        if (rubyController != null && rubyController.health < rubyController.maxHealth)
        {
            rubyController.ChangeHealth(1);
            Destroy(gameObject);

            rubyController.PlayShot(collectedClip);
        }
    }
}
