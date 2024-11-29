using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CheonJiWoon
{
    public class Player : VisibleOnTheMap, IHpObserve, IBattleStat
    {
        public LayerMask moveLayer;
        public LayerMask crashMask;
        public Transform model;
        float moveSpeed = 15.0f;
        bool isMoving = false;

        public UnityAction<float> HpObserve { get; set; }
        public BattleStat battleStat { get => GameData.playerStat; }

        void Awake()
        {
            if (GameData.playerNode == null) GameData.playerNode = Node.firstNode;
        }

        void Start()
        {
            
            transform.position = GameData.playerNode.GetPos();
            ViewOnTheMap();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, moveLayer))
                {
                    hit.transform.GetComponent<IClickAction>()?.ClickAction?.Invoke();
                }
            }
        }

        public void OnMove(Node targetNode)
        {
            if (GameData.playerNode == null || isMoving) return;
            if (GameData.playerNode.children.Contains(targetNode.GetKey())) StartCoroutine(Moving(targetNode));
        }

        IEnumerator Moving(Node targetNode)
        {
            isMoving = true;
            Vector3 dir = targetNode.GetPos() - transform.position;
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
            isMoving = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((crashMask.value >> other.gameObject.layer & 1) != 0)
            {
                Island home = other.GetComponent<Island>();
                ICrashAction action = other.GetComponent<ICrashAction>();
                if (action != null)
                {
                    action.crashTarget = gameObject;
                    action.CrashAction?.Invoke();
                }
            }
        }
    }
}
