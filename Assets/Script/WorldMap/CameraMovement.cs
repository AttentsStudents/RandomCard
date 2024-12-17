using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMap
{
    public class CameraMovement : MonoBehaviour
    {
        float moveSpeed = 30.0f;
        Vector2 limitHorizontal = new Vector2(-5.0f, 5.0f);
        Vector2 limitVertical = new Vector2(-45.0f, 35.0f);
        Vector2 limitZoom = new Vector2(12.0f, 30.0f);

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Vector3 cameraPos = transform.position;
            if (Input.mousePosition.x > Screen.width - 1.0f) cameraPos.x = Mathf.Min(cameraPos.x + moveSpeed * Time.deltaTime, limitHorizontal.y);
            else if (Input.mousePosition.x < 1.0f) cameraPos.x = Mathf.Max(cameraPos.x - moveSpeed * Time.deltaTime, limitHorizontal.x);

            if (Input.mousePosition.y > Screen.height - 1.0f) cameraPos.z = Mathf.Min(cameraPos.z + moveSpeed * Time.deltaTime, limitVertical.y);
            else if (Input.mousePosition.y < 1.0f) cameraPos.z = Mathf.Max(cameraPos.z - moveSpeed * Time.deltaTime, limitVertical.x);

            float wheel = Input.GetAxis("Mouse ScrollWheel");
            if (wheel > 0.0f) cameraPos.y = Mathf.Max(cameraPos.y - 1.0f, limitZoom.x);
            else if (wheel < 0.0f) cameraPos.y = Mathf.Min(cameraPos.y + 1.0f, limitZoom.y);

            transform.position = cameraPos;
        }
    }
}