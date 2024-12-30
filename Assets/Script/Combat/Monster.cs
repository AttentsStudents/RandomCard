using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Combat
{
    public class Monster : VisibleHpBar
    {
        public MonsterData data;
        public int level { get; set; }
        public bool isBattle { get; set; }
        Vector3 originPos;
        GameObject target { get => BattleManager.inst.player.gameObject; }
        public UnityAction ActionEndAlarm { get; set; }
        float moveSpeed = 15.0f;

        void Awake()
        {
            
        }
        void Start()
        {
            if (isBattle)
            {
                battleStat = GameData.targetNode.type == WorldMap.Node.Type.END ?
                    new BattleStat(data.maxHP * 4, data.armor * 2, data.attack * 2) : new BattleStat(data.maxHP, data.armor, data.attack);
                AddHpBar();
                GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/EnemyCollider"), CanvasCustom.main.transform);
                if (obj.TryGetComponent<EnemyCollider>(out EnemyCollider enemyCollider))
                {
                    enemyCollider.target = gameObject;
                    DeathAlarm += () => obj.SetActive(false);
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
            originPos = transform.position;
            Vector3 targetPos = target.transform.position - transform.forward;

            yield return StartCoroutine(Move(targetPos));
            anim.SetTrigger(AnimParams.OnAttack);
        }

        public void OnComeBack()
        {
            StartCoroutine(ComeBack());
        }

        public void OnAttack()
        {
            if (target.TryGetComponent<BattleSystem>(out BattleSystem battleSystem))
            {
                battleSystem.OnDamage(data.attack);
            }
        }

        void Rotate(Vector3 dir)
        {
            float angle = Vector3.Angle(transform.forward, dir);
            if (Vector3.Dot(transform.right, dir) < 0.0f) angle *= -1.0f;
            transform.Rotate(Vector3.up * angle);
        }

        IEnumerator Move(Vector3 targetPos)
        {
            Vector3 dir = targetPos - transform.position;
            float dist = dir.magnitude;
            dir.Normalize();
            Rotate(dir);
            anim.SetBool(AnimParams.IsMoving, true);
            while (dist > 0.0f)
            {
                float delta = Time.deltaTime * moveSpeed;
                if (delta > dist) delta = dist;
                transform.Translate(dir * delta, Space.World);
                dist -= delta;
                yield return null;
            }
            anim.SetBool(AnimParams.IsMoving, false);
        }

        IEnumerator ComeBack()
        {
            yield return StartCoroutine(Move(originPos));

            Rotate(target.transform.position - transform.position);
            ActionEndAlarm?.Invoke();
        }
    }
}

