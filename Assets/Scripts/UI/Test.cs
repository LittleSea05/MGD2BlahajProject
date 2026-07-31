using UnityEngine;
using UnityEngine.UI;
public class Test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Slider testSlider;

    void Update()
    {
        testSlider.value = Mathf.PingPong(Time.time * 0.5f, 1f);
    }
}
