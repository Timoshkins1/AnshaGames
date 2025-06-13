using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera cam;

    private void Start() => cam = Camera.main;
    private void LateUpdate() => transform.LookAt(transform.position + cam.transform.forward);
}