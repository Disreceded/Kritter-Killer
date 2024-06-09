using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;
using UnityEngine.Rendering.Universal;

public class GunController : MonoBehaviour
{
    [Header("OBJECTS/COMPONENTS")]
    public GameObject gunFirePoint;
    public GameObject bulletPrefab;
    public GameObject gunStem;

    public GameObject gunshotFX;
    public Player player;
    public GameObject SpawnedBullet { get; private set; }
    [Header("VARIABLES")]
    [Range(1, 100)]
    public float gunShotForce;
    [Range(1, 10)]
    public float gunAndBulletDistanceThreshold = 3f;
    public float shootCooldown = 0.75f;
    private float shootTimer;

    public float Angle 
    {   get 
        { 
            Vector2 gunDirection = (Vector2)player.MouseWorldPosition - (Vector2)player.transform.position;
            return  Mathf.Atan2(gunDirection.y, gunDirection.x) * Mathf.Rad2Deg + -180;
        }
    }
    public bool canShoot = true;
    public bool justShot { get; private set; }

    private void Update() {
        if (justShot) {
            shootTimer = shootCooldown;
            shootTimer -= Time.deltaTime;
            canShoot = false;
            print("Can't shoot");
        }
        if (shootTimer <= 0) {
            canShoot = true;
            print("Can shoot again");
        }
    }
    // FixedUpdate is Update++
    void FixedUpdate()
    {
        



        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, Angle);


    }
    public void Shoot(InputAction.CallbackContext obj)
    {
        //this is to check if the gun is submerged in the ground
        RaycastHit2D hitData = Physics2D.Raycast(gunFirePoint.transform.position, player.MouseWorldPosition, Mathf.Infinity, player.groundLayer);

        bool hitObject = true;
        
        #pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'

        if (hitData == null) {hitObject = false;}
        // if (hitData.point == Vector2.zero) {hitObject = false;}

        #pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'

        // print($"Raycast found contact at {hitData.point}"); 
        // print($"The gun is at {player.gun.transform.position}");
        float distance = ((Vector3)hitData.point - player.gun.transform.position).magnitude;
        // print($"Distance {distance}");
        //if the distance between the gun and the ground (or object) is big enough
        if(hitObject && distance > 2.75f && canShoot){
            print("pew pew");
            shootTimer = shootCooldown;
            justShot = true;
            
            GameObject flare = Instantiate(gunshotFX, gunFirePoint.transform.position, Quaternion.Euler(transform.position.x, transform.position.y, Angle));
            /// attempted to get the light object and pseudoparented it to the gun fire point (didn't work and it's clunky)

            // GameObject lightObj = flare.GetComponentInChildren<Light2D>().gameObject;
            // Pseudoparent parenter = lightObj.AddComponent<Pseudoparent>();
            // parenter.parent = gunFirePoint;
            // parenter.type = PseudoparentType.Position;
            // lightObj.transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, Angle);

            //so what if i parented it to the gun fire point
            // it works and it looks cool
            flare.transform.SetParent(gunFirePoint.transform);
            

            GameObject bullet = Instantiate(bulletPrefab, gunFirePoint.transform.position, Quaternion.identity);
            SpawnedBullet = bullet;
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            Rigidbody2D bulletPhysics = bullet.GetComponent<Rigidbody2D>();

            bulletScript.hitTag = "Test";
            
            bulletPhysics.rotation = 90;

            bulletPhysics.velocity += (Vector2)(-gunFirePoint.transform.right) * gunShotForce;
            justShot = false;
        }
        else
            return;
    }

}
