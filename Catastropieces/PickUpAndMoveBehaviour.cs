using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HeldObject))]
public class PickUpAndMoveBehaviour : MonoBehaviour
{
    /* This class defines the behaviour of the clicking and moving objects.
     * And the object rotation angle, might be moved to somewhere else in the future.
     * It also signals to the anchor points when to highlight. */

    private HeldObject heldObject;
    public GameObject image;
    public GameObject text;
    public GameObject textBG;
    public Texture2D mouseCursor;
    public Texture2D xCursor;
    private Text description;
    private Text objectTypeText;
    private Image backgroundImage;
    private Quaternion currentRot;
    private AnchorPoint anchorPoint;
    private List<GameObject> stackablePoints;
    private bool showingStackables = true;
    private bool? prevCanPlace = null;
    [HideInInspector]
    public GameObject previousObject;
    [HideInInspector]
    public bool objectisHeld;

    public GameController controller;

    private void Start()
    {
        heldObject = GetComponent<HeldObject>();
        GetAllStackableObjects();
        controller = FindObjectOfType<GameController>();
        objectisHeld = false;
        Cursor.SetCursor(mouseCursor, Vector2.zero, CursorMode.Auto);
    }

    private void Update()
    {
        if (controller.state != GameController.GameState.Build)
            return;

        // Pick up object only if the player is not holding another object
        if (Input.GetMouseButtonDown(0) && heldObject.TheHeldObject == null)
        {
            PickUpObject();
            //Hey there held object we're gonna play your pick up sound
            if (heldObject.TheHeldObject !=null)
            {
                if(heldObject.TheHeldObject.GetComponent<WWisePickUpTrigger>())
                    heldObject.TheHeldObject.GetComponent<WWisePickUpTrigger>().PickUp();
            }
        }
        // Drops an object if the player is holding an object.
        else if (Input.GetMouseButtonDown(0) && heldObject.TheHeldObject != null)
        {
            DropHeldObject();
        }
        // Stab stabber object
        /*else if (Input.GetKeyDown(controller.stabKey) && heldObject.TheHeldObject != null && heldObject.TheHeldObject.GetComponent<StabberComponent>() != null)
        {
            Stab();
        }*/
        // Move held object
        else if (heldObject.TheHeldObject != null)
        {
            MoveHeldObject();
            heldObject.TheHeldObject.GetComponent<BaseObject>().isHeld = true;
        }
        else
        {
            HighlightOutline();
        }
    }

    private void PickUpObject()
    {

        // HideAllStackableObjects();
        RaycastHit hit;
        // Shoot raycast based on mouse position.
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        LayerMask layermask = 1 << (int)BaseObject.Layers.IgnoreRaycast | 1 << (int)BaseObject.Layers.AnchorPoint;
        layermask = ~layermask;
        if (Physics.Raycast(ray, out hit, 1000, layermask))
        {
            GameObject target = hit.transform.gameObject;
            BaseObject usable = target.GetComponent<BaseObject>();
            StackableComponent stackable = target.GetComponent<StackableComponent>();
            StabberComponent stabber = target.GetComponent<StabberComponent>();
            // If the raycast hit an anchor point, check to make sure the anchor point is not occupied.

            // check if target is attached to pivot, if not, target is not movable
            if (!IsAttachedToPivot(target))
            {
                return;
            }

            // set highlight to white
            SetLayerRecursive(target.transform.parent.gameObject,(int)BaseObject.Layers.HoldingObject);

            if(stabber && stabber.Stabbed)
            {
                stabber.Dislodge();
            }

            if (stackable && stackable.isPickupable)
            {
                if (stackable.getSpawnedAnchorPoint() != null)
                {
                    if (stackable.getSpawnedAnchorPoint().GetComponent<AnchorPoint>().IsOccupied == true)
                    {
                        return;
                    }
                }
                previousObject = target;

                heldObject.previousPointPosition = hit.point;
                heldObject.PickUpObject(target);
                ShowAllStackableObjects();
                objectisHeld = true;
            }
            else if (usable && usable.isPickupable)
            {
                previousObject = target;

                heldObject.previousPointPosition = hit.point;
                heldObject.PickUpObject(target);

                ShowAllStackableObjects();
                objectisHeld = true;
            }
        }
    }

    private void DropHeldObject()
    {
        AnchorPoint CurrAnchorPoint = anchorPoint;
        if (CurrAnchorPoint)
        {
            if (!CurrAnchorPoint.IsOccupied && CurrAnchorPoint.canObjectBePlacedHere(heldObject.TheHeldObject) /*&& !heldObject.TheHeldObject.GetComponent<BaseObject>().isColliding*/)
            {
                if (heldObject.TheHeldObject.GetComponent<WWisePlaceTrigger>())
                    heldObject.TheHeldObject.GetComponent<WWisePlaceTrigger>().Place();
                else
                    print("Warning: WWISE not attatched to " + heldObject.TheHeldObject.name);
                    
                SetLayerRecursive(heldObject.TheHeldObject.transform.parent.gameObject, (int)BaseObject.Layers.Default);

                // for stabber
                StabberComponent stabber;
                if(stabber = heldObject.TheHeldObject.GetComponent<StabberComponent>())
                {
                    if (CurrAnchorPoint.Equals(stabber.DetectLodgeTarget()))
                    {
                        stabber.Lodge();
                    }
                }

                heldObject.TheHeldObject.GetComponent<BaseObject>().isHeld = false;
                heldObject.PlaceObject(CurrAnchorPoint.gameObject);
                previousObject = null;
                HideAllStackableObjects();
                objectisHeld = false;
                prevCanPlace = null;
                ChangeCursor(true);
            }
        }
    }

    private void MoveHeldObject()
    {
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        LayerMask layermask = 1 << (int)BaseObject.Layers.IgnoreRaycast | 1 << (int)BaseObject.Layers.HoldingObject;
        layermask = ~layermask;
        if (Physics.Raycast(ray, out hit, 10000, layermask))
        {
            GameObject target = hit.transform.gameObject;
            AnchorPoint CurrAnchorPoint = target.GetComponent<AnchorPoint>();
            if (CurrAnchorPoint)
            {
                DisplayCursor(CurrAnchorPoint.canObjectBePlacedHere(heldObject.TheHeldObject));
                anchorPoint = CurrAnchorPoint;
            }

            heldObject.MoveObject(target);
        }
    }



    private void HighlightOutline()
    {
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        GameObject target;
        LayerMask layermask = 1 << (int)BaseObject.Layers.IgnoreRaycast | 1 << (int)BaseObject.Layers.AnchorPoint;
        layermask = ~layermask;
        if (Physics.Raycast (ray, out hit, 10000, layermask) && IsAttachedToPivot (target = hit.transform.gameObject)) {
            if (!target.Equals (previousObject)) {   
                if (previousObject)
                {
                    SetLayerRecursive (previousObject.transform.parent.gameObject, (int)BaseObject.Layers.Default);
                }
                BaseObject usable = target.GetComponent<BaseObject>();
                if (usable)
                {
                    try
                    {
                        if (usable.isPickupable)
                        {
                            SetLayerRecursive(target.transform.parent.gameObject, (int)BaseObject.Layers.UsableObject);
                        }
                        else
                        {
                            SetLayerRecursive(target.transform.parent.gameObject, (int)BaseObject.Layers.UnusableObject);
                        }
                        ChangeCursor(usable.isPickupable);
                        previousObject = target;
                    }
                    catch
                    {
                        Debug.LogError(target.name + "is not a base object");
                    }
                }
                else
                    ChangeCursor(true);
            }
        }
        else
        {   // move away from object
            if (previousObject)
            {
                SetLayerRecursive(previousObject.transform.parent.gameObject, (int)BaseObject.Layers.Default);
                previousObject = null;
                ChangeCursor(true);
            }
        }
    }

    private void SetLayerRecursive(GameObject obj, int layer)
    {
        // Never set the anchor point!!!
        if (obj.tag.Equals("AnchorPoint"))
            return;
        if(layer==(int)BaseObject.Layers.Default)
        {
            obj.layer = layer;
        }
        else
        {
            obj.layer |= layer;
        }
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

    // to be discarded
    private void Stab()
    {
        StabberComponent stabber;
        if ( stabber = heldObject.TheHeldObject.GetComponent<StabberComponent>())
        {
            RaycastHit hit;
            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 10000))
            {
                GameObject target = hit.transform.gameObject;
                AnchorPoint CurrAnchorPoint = target.GetComponent<AnchorPoint>();
                if (CurrAnchorPoint)
                {
                    if (!CurrAnchorPoint.IsOccupied && CurrAnchorPoint.canObjectBePlacedHere(heldObject.TheHeldObject) && CurrAnchorPoint.Equals(stabber.DetectLodgeTarget()))
                    {
                        heldObject.TheHeldObject.GetComponent<BaseObject>().isHeld = false;
                        stabber.Lodge();
                        heldObject.PlaceObject(target);
                        previousObject = null;
                    }
                }
            }
        }
    }

    private void ShowAllStackableObjects()
    {

        if (showingStackables)
        {
            return;
        }
        foreach (GameObject ap in stackablePoints)
        {

            if (ap.transform.IsChildOf(heldObject.TheHeldObject.transform.parent))
            {
                ap.SetActive(false);
            }
            else
            {
                ap.SetActive(true);
                StabberComponent stabber = ap.transform.parent.GetComponentInChildren<StabberComponent>();
                if (stabber && !stabber.Stabbed)
                {
                    ap.SetActive(false);
                }

            }
        }
        showingStackables = true;
    }


    private void HideAllStackableObjects()
    {

        if (!showingStackables)
        {
            return;
        }

        foreach (GameObject ap in stackablePoints)
        {
            ap.SetActive(false);
        }

        showingStackables = false;

    }

    private void GetAllStackableObjects()
    {
        stackablePoints = new List<GameObject>();
        AnchorPoint[] anchorPoints = FindObjectsOfType(typeof(AnchorPoint)) as AnchorPoint[];

        foreach (AnchorPoint ap in anchorPoints)
        {

            if (ap.isStackable)
            {
                stackablePoints.Add(ap.gameObject);
            }
            if (ap.transform.parent.GetComponentInChildren<StabberComponent>())
            {
                Debug.LogWarning("stabber anchor is in the list");
            }
        }
        HideAllStackableObjects();
    }

    private bool IsAttachedToPivot(GameObject obj)
    {
        if (obj.transform.parent == null)
        {
            return false;
        }

        return obj.transform.parent.tag.Equals("Pivot");
    }

    public void DropCurrentHeldObject()
    {
        if (heldObject.TheHeldObject != null)
        {
            SetLayerRecursive(heldObject.TheHeldObject, (int)BaseObject.Layers.Default);
            heldObject.DropObject();
        }
    }

    private void DisplayCursor(bool canPlace)
    {
        if (!prevCanPlace.HasValue)
            prevCanPlace = canPlace;
        else if(canPlace != prevCanPlace.Value)
        {
            ChangeCursor(canPlace);
            prevCanPlace = canPlace;
        }
    }

    private void ChangeCursor(bool canPlace)
    {
        if (!canPlace)
        {
            Cursor.SetCursor(xCursor, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(mouseCursor, Vector2.zero, CursorMode.Auto);
        }
    }
}