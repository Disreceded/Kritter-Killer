using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//event handlers
public class KritterDeathEventArgs : EventArgs {
    public DateTime TimeOfDeath;
    public int Id;

    public GameObject Kritter;
}
// i havent even started work on this (as of 6/9/2024).
// time to start
public class Kritter : MonoBehaviour
{
    [Header("OBJECTS/COMPONENTS")]
    public Animator animator;
    [Header("VARIABLES")]

    // when kritters spawn they'll have a random color so they all don't look the same.

    public List<Color> colors;
    public int id { get; private set; }
    [Range(0.1f, 35)]
    public float biteRadius = 0.1f;

    [Range(0, 50)]
    public int minDamage;
    public int maxDamage;

    //public float checkCooldown;

    // the delay before the kritter bites the player (it takes time for the kritter to react and decide what to do)
    public float timeBeforeBiting = 0.5f;
    
    public bool JustBit { get; private set; } = false;
    public bool IsDead { get; private set; } = false;

    private float time = 0;

    public static event EventHandler<KritterDeathEventArgs> OnDeath;
    // Start is called before the first frame update
    
    void Start()
    {
        id = UnityEngine.Random.Range(0, 10000);
        Color chosenColor = colors[UnityEngine.Random.Range(0, colors.Count)];
        chosenColor.a = 1;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = chosenColor;
    }

    // Update is called once per frame
    void Update()
    {
        Bite();
    }

    public void Bite() {
        if (Time.time > time + timeBeforeBiting && !IsDead) {
            time = Time.time;
            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, biteRadius);
            
            foreach (Collider2D collider in nearbyColliders)
            {
                Player player = collider.gameObject.GetComponent<Player>();
                if (player == null) { return; }
                player.Damage(UnityEngine.Random.Range(minDamage, maxDamage));
                animator.SetTrigger("Bite");
            }
        }
    }
    // calling base() for this function is mandatory since this function by itself will handle all the animation
    public virtual void Die() {
        OnDeath?.Invoke(this, new KritterDeathEventArgs() { Id = id, Kritter = gameObject, TimeOfDeath = DateTime.Now});
        SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        renderer.sortingOrder = 100;
        animator.SetBool("Dead", false);
        //this is to get that death animation effect, thing, in like Sonic the Hedgehog (the game  in 1991) and SMB (Super Mario Bros)
        Rigidbody2D rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
        rigidbody2D.velocity += Vector2.up * 3;
        rigidbody2D.gravityScale = 3;

        Destroy(gameObject.GetComponent<Collider2D>());
        IsDead = true;
        animator.SetBool("Dead", IsDead);
    }

    private void OnBecameInvisible() {
        if (IsDead) { Destroy(gameObject); }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, biteRadius);
    }
}
