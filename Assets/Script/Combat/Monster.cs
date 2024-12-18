using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Combat
{
    public class Monster : VisibleHpBar
    {
        public MonsterData data;
        public int level { get; set; }
        public bool isBattle { get; set; }
        GameObject target { get => Player.inst?.gameObject; }
        float moveSpeed = 10.0f;

        void Awake()
        {
            battleStat = new BattleStat(data.maxHP, data.armor, data.attack);
        }
        void Start()
        {
            if (isBattle)
            {
                AddHpBar();
                GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/EnemyCollider"), GameObject.FindWithTag("Canvas").transform);
                if (obj.TryGetComponent<EnemyCollider>(out EnemyCollider enemyCollider))
                {
                    enemyCollider.target = gameObject;
                    DeathAlarm += () => Destroy(obj);
                }
            }
        }

        public void OnMoveAttack()
        {
            if (target == null) return;
            StartCoroutine(MoveAttack());
        }

        IEnumerator MoveAttack()
        {
            Vector3 originPos = transform.position;
            Vector3 targetPos = target.transform.position + (target.transform.forward * 1.0f);

            yield return StartCoroutine(Move(targetPos));
            yield return StartCoroutine(Move(originPos));

            Rotate(targetPos - originPos);
        }

        void Rotate(Vector3 dir)
        {
            float angle = Vector3.Angle(transform.forward, dir);
            if(Vector3.Dot(transform.right, dir) < 0.0f) angle *= -1.0f;
            transform.Rotate(Vector3.up * angle);
        }

        IEnumerator Move(Vector3 targetPos)
        {
            Vector3 dir = targetPos - transform.position;
            float dist = dir.magnitude;
            dir.Normalize();
            Rotate(dir);

            while (dist > 0.0f)
            {
                float delta = Time.deltaTime * moveSpeed;
                if (delta > dist) delta = dist;
                transform.Translate(dir * delta, Space.World);
                dist -= delta;
                yield return null;
            }
        }
    }
}

