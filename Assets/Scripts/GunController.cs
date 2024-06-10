using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;
using TMPro;
using Unity.PlasticSCM.Editor;



//im making my own exception just so i dont have to find a specific exception that matches a problem
public class LimitExceededException : Exception{
     public LimitExceededException(string message) : base(message) { }
}
public class NegativeNumberException : Exception {
    public NegativeNumberException(string message) : base(message) {}
}
//just to make things easier for me (ironic how normal integers are already limited)
public class LimitedInteger { 
    private int p_min;
    public int Min { get { return p_min; } set { if (p_min > value) { p_min = value; } } }

    private int p_max;
    public int Max { get { return p_max; } set { if (p_max < value) { p_max = value; } } }

    private int _value;
    public int Value {
        get { 
            return _value; 
        }
        set {
            if (value > Max) {
                throw new LimitExceededException("Cannot set Value property greater than the maximum integer");
            }
            else if (value < Min) {
                throw new LimitExceededException("Cannot set Value property less than the minimum integer");
            }


        }
    }


    // totally didn't copy and paste this online ;)
    public static implicit operator int(LimitedInteger x) { return x.Value; }
    public static implicit operator LimitedInteger(int x) => new LimitedInteger(int.MinValue, int.MaxValue, x);
 
    public static LimitedInteger operator+ (LimitedInteger x, LimitedInteger y) {
        // adding 2 limited integers extends the maximum value of the sum.
        return new LimitedInteger(x.Min, x.Value + y.Value, x.Value + y.Value);
    }
    public static LimitedInteger operator* (LimitedInteger x, LimitedInteger y) {
        // multiplying 2 limited integers extends the maximum value of the product.
        return new LimitedInteger(x.Min, x.Value * y.Value, x.Value * y.Value);
    }
    public static LimitedInteger operator- (LimitedInteger x, LimitedInteger y) {
        // subtracting 2 limited integers extends the minimum value of the difference.
        int biggestMaxNum = 0;
        if (x.Max > y.Max) { biggestMaxNum = x.Max;}
        else if (x.Max < y.Max) { biggestMaxNum = y.Max;}
        else if (x.Max == y.Max) { biggestMaxNum = x.Max;}

        int biggestMinNum = 0;
        if (x.Min < y.Min) { biggestMinNum = x.Min;}
        else if (y.Min < x.Min) { biggestMinNum = y.Min;}
        else if (x.Min == y.Min) { biggestMinNum = x.Min;}

        return new LimitedInteger(biggestMinNum, biggestMaxNum, x.Value - y.Value);
    }
        public static LimitedInteger operator/ (LimitedInteger x, LimitedInteger y) {
        // dividing 2 limited integers extends the minimum value of the quotient.
        int biggestMaxNum = 0;
        if (x.Max > y.Max) { biggestMaxNum = x.Max;}
        else if (x.Max < y.Max) { biggestMaxNum = y.Max;}
        else if (x.Max == y.Max) { biggestMaxNum = x.Max;}

        int biggestMinNum = 0;
        if (x.Min < y.Min) { biggestMinNum = x.Min;}
        else if (y.Min < x.Min) { biggestMinNum = y.Min;}
        else if (x.Min == y.Min) { biggestMinNum = x.Min;}

        return new LimitedInteger(biggestMinNum, biggestMaxNum, x.Value / y.Value);


    }
    public LimitedInteger(int min, int max) {
        this.Min = min;
        this.Max = max;
        
    }
    public LimitedInteger(int min, int max, int value) {
        this.Min = min;
        this.p_max = max;
        this._value = value;
        this.Value = value;
    }
}
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
    public TextMeshProUGUI ammoText;
    [Range(1, 100)]
    public float gunShotForce;
    [Range(1, 10)]
    public float reloadingTime;
    [Range(1, 10)]
    public float gunAndBulletDistanceThreshold = 3f;
    public float shootCooldown = 3f;

    public LimitedInteger Ammo;
    public int  maxAmmo;

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
    public bool isReloading { get; private set; }

    private float time;

    private void Start() {
        Ammo = new LimitedInteger(0, maxAmmo, maxAmmo);
        
    }
    public void ReloadFunc() {
        StartCoroutine(Reload());
    }
    public void ReloadFunc(bool maxAmmoDecreases) {
        StartCoroutine(Reload(maxAmmoDecreases));
    }
    public void ReloadFunc(float time) {
        StartCoroutine(Reload(time));
    }
    public IEnumerator Reload() {
        if (!isReloading) {
            isReloading = true;
            //jic (just in case)
            canShoot = false;
            yield return new WaitForSeconds(reloadingTime);
            Ammo = maxAmmo;
            isReloading = false;
            canShoot = true;
        }
        else {
            Debug.LogWarning("Reload function called when isReloading is already set to true");
        }
    }
    public IEnumerator Reload(bool maxAmmoDecreases) {
        if (!isReloading) {
            if (maxAmmoDecreases) {
                maxAmmo -= 2;
                Ammo.Max = maxAmmo;
                if (Ammo > 0) {
                    isReloading = true;
                    //jic (just in case)
                    canShoot = false;
                    yield return new WaitForSeconds(time);
                    isReloading = false;
                    maxAmmo--;
                    Ammo = maxAmmo;
                }
            }
            isReloading = true;
            //jic (just in case)
            canShoot = false;
            yield return new WaitForSeconds(reloadingTime);
            Ammo = maxAmmo;
            isReloading = false;
            canShoot = true;
            
        }
        else {
            Debug.LogWarning("Reload function called when isReloading is already set to true");
        }
    }
    public IEnumerator Reload(float time) {
        if (!isReloading) {
            if (Ammo > 0) {
                isReloading = true;
                //jic (just in case)
                canShoot = false;
                yield return new WaitForSeconds(time);
                isReloading = false;
                Ammo = maxAmmo;
            }
            isReloading = true;
            //jic (just in case)
            canShoot = false;
            yield return new WaitForSeconds(time);
            isReloading = false;
            Ammo = maxAmmo;
        }
        else {
            Debug.LogWarning("Reload function called when isReloading is already set to true");
        }
    }
    private void Update() {
        // if (justShot) {
        //     shootTimer -= Time.deltaTime;
        //     canShoot = false;
        //     print("Can't shoot");
        // }
        // if (shootTimer <= 0) {
        //     canShoot = true;
        //     print("Can shoot again");
        // }
        // if (Ammo == 0) { canShoot = false; print("no ammo"); }
        ammoText.text = $"{Ammo.Value}/{maxAmmo}";
        print($"Can shoot: {canShoot}");
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
        if(hitObject && distance > 2.75f && canShoot && Ammo != 0 && Time.time > time + shootCooldown){
    
            print("pew pew");
            time = Time.time;
            justShot = true;
            Ammo--;
            print(Ammo);
            
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
        else if (Ammo == 0) {
            StartCoroutine(Reload());
        }
        else
            return;
    }

}
