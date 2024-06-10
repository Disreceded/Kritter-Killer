using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletHitEventArgs : EventArgs
{
    public string TagUsed;
    public GameObject HitObject;
    public DateTime TimeHit;
}
public class BulletRicochetEventArgs : EventArgs
{
    public string TagUsed;
    public GameObject HitObject;
    public DateTime TimeHit;
    public int TimesReflected;
}
public class Bullet : MonoBehaviour
{

    public string hitTag;
    public bool autoDestroy = false;
    public bool autoDestroyOnTooManyRicochets = true;
    public int autoDestroyOnTooManyRicochetsLimit = 10;
    private int timesHit;
    public event EventHandler<BulletHitEventArgs> BulletHitTagObject;
    public event EventHandler<BulletHitEventArgs> BulletHit;
    public event EventHandler<BulletRicochetEventArgs> Ricochet;
    private int timesReflected;

    public bool isInView {  get;  private set;    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        #region event args
        BulletHitEventArgs eventArgs = new BulletHitEventArgs();
        eventArgs.TimeHit = DateTime.Now;
        eventArgs.TagUsed = hitTag;
        eventArgs.HitObject = collision.gameObject;
        #endregion event args
        if (collision.gameObject.tag == hitTag) { BulletHitTagObject?.Invoke(this, eventArgs); }

        else { BulletHit?.Invoke(this, eventArgs); }

        if (autoDestroy) { Destroy(gameObject); }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        #region event args
        BulletHitEventArgs eventArgs = new BulletHitEventArgs();
        eventArgs.TimeHit = DateTime.Now;
        eventArgs.TagUsed = hitTag;
        eventArgs.HitObject = collision.gameObject;
        #endregion

        if (collision.gameObject.tag == hitTag) { BulletHitTagObject?.Invoke(this, eventArgs); }

        BulletHit?.Invoke(this, eventArgs);

        if (autoDestroy) { Destroy(gameObject); }

        Vector2 reflectionDirection = Vector2.Reflect(transform.position, collision.GetContact(0).normal).normalized;
        Rigidbody2D bulletPhysics = GetComponent<Rigidbody2D>();

        bulletPhysics.velocity += reflectionDirection * 20;
        timesHit++;

        BulletRicochetEventArgs ricochetEventArgs = new BulletRicochetEventArgs() { 
            TimeHit = DateTime.Now,
            TagUsed = hitTag,
            HitObject = collision.gameObject,
            TimesReflected = timesHit,
        };

        
        Ricochet?.Invoke(this, ricochetEventArgs);
        if (timesHit > autoDestroyOnTooManyRicochetsLimit)
        {
            Destroy(gameObject);
        }
    }
    public void Destruct()
    {
        if (!isInView){
            Destroy(gameObject);
        }
    }
    private void OnBecameInvisible()
    {
        isInView = false;
        Invoke("Destruct", 2);
    }
    private void OnBecameVisible() {
        isInView = true;
    }
}
