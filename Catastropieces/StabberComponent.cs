using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StabberComponent : Component
{
    [SerializeField]
    private bool mStabbed = false;
    private GameObject stabbedObject;
    public AnchorPoint anchor;

    public bool Stabbed
    {
        get
        {
            return mStabbed;
        }
    }


	protected override void Start ()
	{
        base.Start();
	}

    protected override void Update()
    {
        base.Update();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.contacts[0].thisCollider.Equals(GetComponent<SphereCollider>()))
            return;
        if (!mStabbed)
        {
            if (!collision.gameObject.GetComponent<Rigidbody>())
            {
                collision.gameObject.AddComponent<Rigidbody>();
                collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                collision.gameObject.GetComponent<Rigidbody>().useGravity = false;
            }
            collision.gameObject.AddComponent<FixedJoint>();
            collision.gameObject.GetComponent<FixedJoint>().connectedBody = GetComponent<Rigidbody>();
            stabbedObject = collision.gameObject;
            mStabbed = true;
            //anchor.gameObject.SetActive(true);
        }
    }

    public void Restart()
    {
        if (stabbedObject)
        {
            if (stabbedObject.GetComponent<FixedJoint>())
            {
                Destroy(stabbedObject.GetComponent<FixedJoint>());
            }
            stabbedObject = null;
        }
        mStabbed = false;
        //anchor.gameObject.SetActive(false);
    }

    public void Lodge()
    {
        // check if can lodge into the current anchor point
        // or maybe should do this in pickupbehav?
        //anchor.gameObject.SetActive(true);
        mStabbed = true;
    }

    public void Dislodge()
    {
        if(anchor.IsOccupied)
        {
            Debug.LogWarning("Can't dislodge stabber " + gameObject.ToString() + " as anchor point is occupied");
            return;
        }
        Restart();
        Debug.LogWarning("Dislodging stabber " + gameObject.ToString());
        transform.parent.parent = null;
    }

    public AnchorPoint DetectLodgeTarget()
    {
        Vector3 globalDir = transform.TransformVector(GetComponent<SphereCollider>().center);
        Ray ray = new Ray(transform.position, globalDir);
        RaycastHit hit;
        LayerMask mask = 1 << LayerMask.NameToLayer("AnchorPoint");
        if(Physics.Raycast(ray, out hit, 100, mask))
        {
            return hit.transform.gameObject.GetComponent<AnchorPoint>();
        }
        return null;
    }

    // WIP
    public void pointTowardsObject(GameObject targetAnchor){
    }
}
