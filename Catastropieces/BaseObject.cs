using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class BaseObject : MonoBehaviour
{
    /* This script should go onto a usable object. It allows the player to pick up
     * and move the object by changing the objects layer from the default to the ignore
     * raycast layer. It also manages which anchor point is "holding" the object */
    public string description = "";

    public bool isXRotatable = true;
    public bool isYRotatable = true;
    public bool isPickupable = true;
    public bool isKinematic = false;
    public bool snapOnInitialPickUp = false;
    public bool isTrigger = false;

    protected GameController controller;

    private PickUpAndMoveBehaviour pickUpBehavior;

    // outdated, not in use for now
    public float ROTATION
    {
        get
        {
            return (int)controller.rotationAngle;
        }
    }

    public int Layer
    {
        get
        {
            return gameObject.layer;
        }
        set
        {
            gameObject.layer = value;
            foreach (Transform childTrans in GetComponentInChildren<Transform>())
            {
                childTrans.gameObject.layer = value;
            }
        }
    }

    public bool IsColliding
    {
        get
        {
            var colliders = transform.parent.GetComponentsInChildren<Collider>();
            LayerMask layermask = 1 << (int)Layers.IgnoreRaycast | 1 << (int)Layers.HoldingObject | 1 << (int)Layers.AnchorPoint;
            layermask = ~layermask;
            foreach(var coll in colliders)
            {
                if (coll.gameObject.tag.Equals("AnchorPoint"))
                    continue;
                var hits = Physics.OverlapBox(coll.bounds.center, coll.bounds.size / 2.0f, coll.gameObject.transform.rotation, layermask);
                if (hits.Length != 0)
                {
                    Debug.LogWarning("Colliding: " + hits[0].ToString());
                    return true;
                }
            }
            return false;
        }
    }

    [HideInInspector]
    public bool isHeld = false;

    [HideInInspector]
    public AnchorPoint anchorPoint;
    protected bool isPaused = false;
    public enum Layers : int{
        Default = 0,
        TransparentFX = 1,
        IgnoreRaycast = 2,
        AnchorPoint = 8,
        ImmovableObject = 10,
        UsableObject = 12,
        UnusableObject = 13,
        HoldingObject = 14
    }
    protected Vector3 startingPosition;
    protected Quaternion startingRotation;
    protected Vector3 startingPivotPosition;
    protected Quaternion startingPivotRotation;
    protected Vector3 playPosition;
    protected Quaternion playRotation;
    protected Vector3 playPivotPosition;
    protected Quaternion playPivotRotation;

    [Tooltip("Object type label.")]
    public string ObjectType;

    public string GetDescription()
    {
        // Debug.Log(description);
        return description;
    }

    public string GetObjectType()
    {
        return ObjectType;
    }

    public virtual void Start()
    {
        // For now some objects are not properly attached to pivot
        if (transform.parent != null && transform.parent.gameObject.tag.Equals("Pivot"))
        {
            startingPivotPosition = transform.parent.transform.position;
            startingPivotRotation = transform.parent.transform.rotation;
        }

        startingPosition = transform.position;
        startingRotation = transform.rotation;

        GetComponent<Rigidbody>().isKinematic = true;
        pickUpBehavior = FindObjectOfType<PickUpAndMoveBehaviour>();
        controller = FindObjectOfType<GameController>();

        Deactivate();
            
        snapOnInitialPickUp = true; // set at start of game

    }

    public virtual void Update()
    {
        // Rotation is handled here
        if (isHeld && Input.GetMouseButton(1))
        {
            float dotUp = Vector3.Dot(transform.up, controller.localZAxis);
            float dotForward = Vector3.Dot(transform.forward, controller.localZAxis);
            float dotRight = Vector3.Dot(transform.right, controller.localZAxis);
            Vector3 localZ;
            float absMax = Mathf.Max(Mathf.Abs(dotUp), Mathf.Abs(dotForward), Mathf.Abs(dotRight));
            if (absMax == Mathf.Abs(dotUp))
            {
                localZ = dotUp > 0.0f ? transform.up : transform.up * -1.0f;
            }
            else if (absMax == Mathf.Abs(dotForward))
            {
                localZ = dotForward > 0.0f ? transform.forward : transform.forward * -1.0f;
            }
            else
            {
                localZ = dotRight > 0.0f ? transform.right : transform.right * -1.0f;
            }

            int count = transform.parent.GetComponentsInChildren<BaseObject>().Length; // only rotate around z axis if has no children
            if (Input.GetKeyDown(controller.zPositive) && isXRotatable & count==1)
            {
                var parentTransform = gameObject.transform.parent.transform;
                parentTransform.Rotate(localZ, 90.0f, Space.World);
            }
            if (Input.GetKeyDown(controller.zNegative) && isXRotatable & count==1)
            {
                var parentTransform = gameObject.transform.parent.transform;
                parentTransform.Rotate(localZ, -90.0f, Space.World);
            }
            if (Input.GetKeyDown(controller.yPositive) && isYRotatable)
            {
                var parentTransform = gameObject.transform.parent.transform;
                parentTransform.Rotate(controller.globalYAxis, 45.0f, Space.World);

            }
            if (Input.GetKeyDown(controller.yNegative) && isYRotatable)
            {
                var parentTransform = gameObject.transform.parent.transform;
                parentTransform.Rotate(controller.globalYAxis, -45.0f, Space.World);
            }
        }
    }

    public virtual void Play()
    {
        playPosition = transform.position;
        playRotation = transform.rotation;
        if(transform.parent != null && transform.parent.gameObject.tag.Equals("Pivot"))
        {
            playPivotPosition = transform.parent.position;
            playPivotRotation = transform.parent.rotation;
        }

        StartCoroutine(ShortWait());

        if (isKinematic)
            Deactivate();
    }

    public virtual void Activate()
    {
        if(!isTrigger)
        {
            GetComponent<Collider>().isTrigger = false;
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Collider>())
                {
                    child.GetComponent<Collider>().isTrigger = false;
                }
                    
            }
        }

        if (!isKinematic)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Rigidbody>())
                {
                    child.GetComponent<Rigidbody>().isKinematic = false;
                }
            }
        }
    }

    public virtual void Deactivate()
    {
        //GetComponent<Collider>().isTrigger = isKinematic ? false : true;

        if (!isTrigger && !isKinematic)
        {
            GetComponent<Collider>().isTrigger = true;
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Collider>())
                {
                    child.GetComponent<Collider>().isTrigger = true;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isKinematic)
        {
            if (other.GetComponent<Rigidbody>())
            {
                if (other.GetComponent<Rigidbody>().velocity.magnitude > 0.1)
                {
                    Activate();
                }
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        // Not fully implemented, only do this when key is pressed
        if(!Input.GetKey(KeyCode.BackQuote)){
            return;
        }
        if(transform.IsChildOf(other.transform)||other.transform.IsChildOf(transform)){
            return;
        }
        Bounds b0=GetComponent<Collider>().bounds;
        Transform t;
        var rb = other.GetComponent<Rigidbody>();
        if(rb){
            Bounds b1=other.bounds;
            if(b0.min.y>b1.min.y){
                t=transform;
            }else{
                t=other.transform;
            }
        }else{
            rb = other.GetComponentInParent<Rigidbody>();
            if(rb){
                Bounds b1=rb.GetComponent<Collider>().bounds;
                if(b0.min.y>b1.min.y){
                    t=transform;
                }else{
                    t=rb.transform;
                }
            }else{
                t=transform;
            }
        }
        Vector3 pos0 = t.position;
        pos0.y+=0.0002f;
        t.position = pos0;
    }

    public virtual void LatePlay()
    {

    }

    IEnumerator ShortWait()
    {
        yield return new WaitForSeconds(.5f);
        LatePlay();
    }

    public virtual void Pause()
    {
        GetComponent<Rigidbody>().isKinematic = true;
        Deactivate();
    }

    public virtual void Restart()
    {
        // For now some objects are not properly pivoted
        if (transform.parent != null && transform.parent.gameObject.tag.Equals("Pivot"))
        {
            transform.parent.transform.position = startingPivotPosition;
            transform.parent.transform.rotation = startingPivotRotation;
        }
        transform.position = startingPosition;
        transform.rotation = startingRotation;

        GetComponent<Rigidbody>().isKinematic = true;
        try
        {
            transform.parent.parent = null;
        }
        catch
        {
            Debug.LogError(gameObject.name + " doesn't have parent!");
        }
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Rigidbody>())
            {
                child.GetComponent<Rigidbody>().isKinematic = true;
            }
        }

        if (GetComponent<StabberComponent>())
        {
            GetComponent<StabberComponent>().Restart();
        }
            
        snapOnInitialPickUp = true; // reset when reset game
    }

    public virtual void GoBack()
    {
        // For now some objects are not properly pivoted
        if (transform.parent != null && transform.parent.gameObject.tag.Equals("Pivot"))
        {
            transform.parent.transform.position = playPivotPosition;
            transform.parent.transform.rotation = playPivotRotation;
        }
        transform.position = playPosition;
        transform.rotation = playRotation;

        GetComponent<Rigidbody>().isKinematic = true;

        foreach (Transform child in transform)
        {
            if (child.GetComponent<Rigidbody>())
            {
                child.GetComponent<Rigidbody>().isKinematic = true;
            }
        }

        if (GetComponent<StabberComponent>())
        {
            GetComponent<StabberComponent>().Restart();
        }
        Deactivate();
    }

    public virtual void PickUp()
    {
        gameObject.transform.parent.parent = null;
        if (anchorPoint)
        {
            anchorPoint.IsOccupied = false;
            anchorPoint = null;
        }
    }

    public void ResetRotation()
    {
        transform.rotation = startingRotation;
    }

    public virtual void Place(GameObject dropLocation, AnchorPoint anchorPoint)
    {
        this.anchorPoint = anchorPoint;

        Layer = (int)Layers.Default;

        float objectSize = 0.0f;
        foreach(var collider in GetComponents<Collider>())
        {
            if (collider.gameObject.transform.parent.tag.Equals("Pivot"))
                objectSize = collider.bounds.size.y;
        }
        transform.parent.position = dropLocation.GetComponent<AnchorPoint>().GetPosition(objectSize);
        transform.parent.parent = anchorPoint.transform.parent;
    }
}