// Скрипт BushDetector.cs
using UnityEngine;

public class BushDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            Bush bush = other.GetComponent<Bush>();
            if (bush != null) bush.SetTransparent(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bush"))
        {
            Bush bush = other.GetComponent<Bush>();
            if (bush != null) bush.SetTransparent(false);
        }
    }
}