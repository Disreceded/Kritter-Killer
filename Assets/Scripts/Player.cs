using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;


public class Player : MonoBehaviour
{
    private PlayerControls controls;
    [Header("OBJECT/COMPONENTS")]
    public GameObject gun;

    [Header("INPUT/PHYSICS")]
    public float speed;
    public float jumpForce = 75f;
    public Rigidbody2D body;

    public float groundRayDistance;

    public LayerMask groundLayer;

    public Vector2 GroundRayPos { get { return new Vector2(transform.position.x, transform.position.y + groundRayDistance); }  }
    public Vector2 Position { get { return transform.position; } }
    public Vector2 RigidbodyPosition { get { return body.position; } }

    public bool Grounded { get { return IsGrounded(); } }


    [Header("DEBUG")]
    public Vector2 input;
    public Vector2 multipliedInput;


    // PROPERTIES //
    public Camera Camera {  get { return Camera.main; } }
    public Vector2 MousePosition { get { return Input.mousePosition; } }
    public Vector3 MouseWorldPosition { get { return Camera.ScreenToWorldPoint(MousePosition); } }

    public GunController GunController
    {
        get
        {
            // if the gun even exists and has the gun script
            if (gun != null & gun.GetComponent<GunController>() != null)
            {
                // return that
                return gun.GetComponent<GunController>();
            }
            return null;
        }
    }
    // Awake is called before Start
    private void Awake()
    {

        #region Input
        // Initializing the controls
        controls = new PlayerControls();
        controls.Enable();

        controls.Controls.Jump.started += Jump;
        controls.Controls.Shoot.started += (ctx) => {
            GunController.Shoot(ctx);
            Bullet bullet = GunController.SpawnedBullet.GetComponent<Bullet>();
            if (bullet == null) { return;}
            bullet.BulletHit += (sender, args) =>
            {
                if (args.HitObject.name.ToLower().Contains("kritter")) {
                    print("Killed a kritter");
                    Destroy(args.HitObject);
                }
            };
        };
        #endregion
    }



    private void Jump(InputAction.CallbackContext ctx)
    {
        print(Grounded);
        if (!Grounded) { return; }
        body.velocity += Vector2.up * jumpForce;
    }

    public bool IsGrounded()
    {
        bool grounded = false;
        RaycastHit2D hit = Physics2D.Linecast(Position, GroundRayPos, groundLayer);
        #pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
        if (hit != null) { grounded = true; print(hit.collider.gameObject.name); }
        #pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
        return grounded;
    }
    private void Move()
    {
       
        // Get the direction the rigidbody will go in (which is the input vector multiplied by the speed)
        Vector2 direction = GetInput(speed);

        // Changing the velocity
        body.velocity += direction * Time.deltaTime;
    }

    /// <summary>
    /// Returns player input
    /// </summary>
    /// <returns>Vector2 input</returns>
    public Vector2 GetInput() { return controls.Controls.Movement.ReadValue<Vector2>(); }
    /// <summary>
    /// Returns player input
    /// </summary>
    /// <returns>Vector2 input, multiplied by speed (float)</returns>
    public Vector2 GetInput(float speed) { return controls.Controls.Movement.ReadValue<Vector2>() * speed; }

    // Start is called before the first frame update
    void Start()
    {
        
        // To let us know the player has loaded
        print(gameObject.name + " has loaded");
        if (GunController == null) { gun.AddComponent<GunController>(); }

        
    }

    // Update is called once per frame
    void Update()
    {
        #region Variable Setting
        input = GetInput();
        multipliedInput = GetInput(speed);
        #endregion
    }

    // FixedUpdate is Update++
    private void FixedUpdate()
    {
        Move();
    }
    // OnDisable is called when the script is disabled.
    private void OnDisable()
    {
        controls.Disable();
    }

    // OnDrawGizmos allows us to draw gizmos.
    private void OnDrawGizmos()
    {
        #region Ray Gizmos
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Position, GroundRayPos);
        #endregion
    }
}
