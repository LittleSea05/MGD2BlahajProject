using UnityEngine;
using UnityEngine.EventSystems;

public class RushButton : MonoBehaviour,IPointerDownHandler, IPointerUpHandler
{
    public static bool IsRushing { get; private set; } = false;

    void Update()
    {
        //Debug.Log("RushButton Update running, IsRushing = " + IsRushing);
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            IsRushing = true;
        }

        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            IsRushing=false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Rush button pressed!");
        IsRushing = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsRushing = false;
    }


}
