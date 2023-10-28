using UnityEngine;

public class MobileInput : MonoBehaviour
{
    private bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
    private Vector2 swipeDelta, startTourch;

    private float DEADZONE = 100f;

    public static MobileInput Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        //reseting all booleans
        tap = swipeLeft = swipeRight = swipeDown = swipeUp = false;

        //let check for input
        #region Standalone Input
        if (Input.GetMouseButtonDown(0))
        {
            tap = true;
            startTourch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            startTourch = swipeDelta = Vector2.zero;
        }
        #endregion

        #region Mobile Input
        if(Input.touchCount > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                tap = true;
                startTourch = Input.mousePosition;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                startTourch = swipeDelta = Vector2.zero;
            }
        }
        #endregion

        //calculate distance
        swipeDelta = Vector2.zero;
        if (startTourch != Vector2.zero)
        {
            //check mobile
            if (Input.touches.Length != 0)
            {
                swipeDelta = Input.touches[0].position - startTourch;
            }
            //check standalone
            else if (Input.GetMouseButton(0))
            {
                swipeDelta = (Vector2)Input.mousePosition - startTourch;
            }
        }

        //check if we're beyond the deadzone
        if (swipeDelta.magnitude > DEADZONE)
        {
            float x = swipeDelta.x;
            float y = swipeDelta.y;

            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                //Left or right
                if (x < 0)
                {
                    swipeLeft = true;
                }
                else
                {
                    swipeRight = true;
                }
            }
            else
            {
                //up or down
                if (y < 0)
                {
                    swipeDown = true;
                }
                else
                {
                    swipeUp = true;
                }
            }

            startTourch = swipeDelta = Vector2.zero;
        }
    }

    public bool Tap => tap;
    public bool SwipeDown => swipeDown;
    public bool SwipeUp => swipeUp;
    public bool SwipeLeft => swipeLeft;
    public bool SwipeRight => swipeRight;
    public Vector2 SwipeDelta => swipeDelta;
}
