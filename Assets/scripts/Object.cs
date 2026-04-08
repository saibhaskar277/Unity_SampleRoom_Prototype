using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Object : MonoBehaviour
{
    public int itemID;
    [SerializeField] Button button;
    [SerializeField] Transform camPos;
    private Camera cam;

    private Quaternion baseAngle;

    private void Awake()
    {
        camPos.LookAt(transform.position);
    }

    void Start()
    {
        baseAngle = camPos.rotation;
        cam = Camera.main;
        button.onClick.AddListener(MoveCam);
    }

    bool isInObjectCam=false;
    private void OnEnable()
    {
        Manager.OnStateChanged += OnStateChangedListner;
    }

    private void OnStateChangedListner(VirtualCam cam)
    {
        if (cam ==VirtualCam.PlayerCam)
        {
            WaitUntilObjRotation(0, false);
        }
        else
        {
            WaitUntilObjRotation(0.7f,true);
        }
    }

    async void WaitUntilObjRotation(float delayTime, bool status)
    {
        await System.Threading.Tasks.Task.Delay((int)delayTime*1000);
        isInObjectCam = status;
        button.gameObject.SetActive(status);
    }

    void MoveCam()
    {
        Manager.Instance.SetVirtualCam(VirtualCam.ObjectCam,camPos);
        Manager.Instance.ItemClicked(this);
        currentMouseDelta = Vector2.zero;
        rotation = Vector2.zero;
        StartCoroutine(SmoothLookAt(transform));
        //camPos.LookAt(transform);
    }

    IEnumerator SmoothLookAt(Transform target)
    {
        float duration = 0.7f; 
        float elapsed = 0f;

        Quaternion startRot = camPos.rotation;
        Quaternion targetRot = Quaternion.LookRotation(target.position - camPos.position, Vector3.up);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0, 1, t);

            camPos.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        camPos.rotation = targetRot;
    }

    bool isMoving = false;

    void Update()
    {
        if (isInObjectCam)
        {
            button.transform.rotation = Quaternion.LookRotation(button.transform.position - cam.transform.position);
            if (Input.GetMouseButton(0))
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
            HandleMouseLookAlternative();
            //HandleMouseLook();
        }
    }

    private Vector2 currentMouseDelta;
    public float lookSensitivity = 2f;
    private Vector2 currentMouseDeltaVelocity;
    public float lookSmoothTime = 0.05f;
    private Vector2 rotation;
    Quaternion rotY;
    Quaternion rotX;
    Vector3 targetRotationVector;

    void HandleMouseLook()
    {
        if (!isMoving)
            return;

        float mouseX = Input.GetAxisRaw("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * lookSensitivity;

        Vector2 targetMouseDelta = new Vector2(mouseX, mouseY);

        currentMouseDelta = Vector2.SmoothDamp(
            currentMouseDelta,
            targetMouseDelta,
            ref currentMouseDeltaVelocity,
            lookSmoothTime
        );

        if (targetMouseDelta.sqrMagnitude > 0.0001f)
        {
            rotation.y += currentMouseDelta.x;
            rotation.x -= currentMouseDelta.y;
            rotation.x = Mathf.Clamp(rotation.x, -45f, 20f);
        }

        rotY = Quaternion.AngleAxis(rotation.y, camPos.up);
        rotX = Quaternion.AngleAxis(rotation.x, camPos.right);

        Quaternion targetRotation = rotY * rotX * Quaternion.Euler(baseAngle.eulerAngles.x, baseAngle.eulerAngles.y, 0);
        targetRotationVector = targetRotation.eulerAngles;
        targetRotationVector.z = 0;

        camPos.rotation = Quaternion.Euler(targetRotationVector);
    }

    Vector2 targetMouseDelta;
    Vector3 baseEuler, newEuler;

    void HandleMouseLookAlternative()
    {
        if (!isMoving || !isInObjectCam)
            return;

        float mouseX = Input.GetAxisRaw("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * lookSensitivity;

        targetMouseDelta = new Vector2(mouseX, mouseY);

        currentMouseDelta = Vector2.SmoothDamp(
            currentMouseDelta,
            targetMouseDelta,
            ref currentMouseDeltaVelocity,
            lookSmoothTime
        );

        if (targetMouseDelta.sqrMagnitude > 0.0001f)
        {
            rotation.y += currentMouseDelta.x;
            rotation.x -= currentMouseDelta.y;
            rotation.x = Mathf.Clamp(rotation.x, -45f, 20f);
        }

        baseEuler = baseAngle.eulerAngles;

        newEuler = new Vector3(
            baseEuler.x + rotation.x,
            baseEuler.y + rotation.y,
            baseEuler.z
        );

        camPos.rotation = Quaternion.Euler(newEuler);
    }


    public void ApplyMaterial(Material material)
    {
        GetComponentInChildren<Renderer>().material = material;
    }

    private void OnDisable()
    {
        Manager.OnStateChanged -= OnStateChangedListner;
    }
}
