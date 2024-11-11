using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CheonJiWoon;

namespace CheonJiWoon
{
    public class Player : MonoBehaviour
    {
        public LayerMask moveLayer;
        Node nowNode;
        float moveSpeed = 15.0f;
        bool isMoving = false;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, moveLayer))
                    hit.transform.GetComponent<IClickAction>().ClickAction.Invoke();
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

            float angle = Vector3.Angle(transform.forward, dir);
            if (Vector3.Dot(transform.right, dir) < 0.0f) angle *= -1.0f;
            transform.Rotate(Vector3.up * angle);

            while (dist > 0.0f)
            {
                float delta = Time.deltaTime * moveSpeed;
                if (delta > dist) delta = dist;
                transform.Translate(transform.forward * delta, Space.World);
                dist -= delta;

                yield return null;
            }

            nowNode = targetNode;
            isMoving = false;
        }
    }
}
