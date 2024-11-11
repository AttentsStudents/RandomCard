using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CheonJiWoon
{
    public class Player : MonoBehaviour
    {
        public LayerMask moveLayer;
        public Transform model;
        Node nowNode;
        float moveSpeed = 15.0f;
        bool isMoving = false;

        // Start is called before the first frame update
        void Start()
        {
            if (nowNode == null) nowNode = Map.instance.firstNode;
            transform.position = nowNode.gameobject.transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, moveLayer))
                {
                    hit.transform.GetComponent<IClickAction>().ClickAction.Invoke();
                }
            }
        }

        public void OnMove(Node targetNode)
        {
            if (nowNode == null || isMoving) return;
            bool connect = false;

            foreach (Line line in nowNode.paths)
            {
                if (line.node == targetNode)
                {
                    connect = true;
                    break;
                }
            }

            if (connect) StartCoroutine(Moving(targetNode));
        }

        IEnumerator Moving(Node targetNode)
        {
            isMoving = true;
            Vector3 dir = targetNode.gameobject.transform.position - transform.position;
            float dist = dir.magnitude;
            dir.Normalize();

            float angle = Vector3.Angle(model.forward, dir);
            float rot = 1.0f;
            if (Vector3.Dot(model.right, dir) < 0.0f) rot = -1.0f;

            while (angle > 0.0f)
            {
                float delta = Time.deltaTime * 360.0f;
                if (delta > angle) delta = angle;
                model.Rotate(Vector3.up * delta * rot);
                angle -= delta;

                yield return null;
            }

            while (dist > 0.0f)
            {
                float delta = Time.deltaTime * moveSpeed;
                if (delta > dist) delta = dist;
                transform.Translate(dir * delta);
                dist -= delta;

                yield return null;
            }

            nowNode = targetNode;
            isMoving = false;
        }
    }
}
