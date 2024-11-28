using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Player : BattleSystem
{
    public Transform targetMonster;
    private NavMeshAgent navMeshAgent;
    public Animator anim;
    public float attackRange = 2.0f;
    private bool isTriggered = false;

    void Start()
    {
        OnReset();
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent가 필요합니다.");
        }
    }

    void Update()
    {
        if (isTriggered && targetMonster != null)
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            myAnim.SetFloat("x", x);
            myAnim.SetFloat("y", y);
            MoveToTargetAndAttack();
        }
    }

    public void TriggerAttack()
    {
        isTriggered = true;
    }

    private void MoveToTargetAndAttack() // 공격하러 갈 때
    {
        navMeshAgent.SetDestination(targetMonster.position);

        float distanceToTarget = Vector3.Distance(transform.position, targetMonster.position);
        if (distanceToTarget <= attackRange)
        {
            navMeshAgent.isStopped = true;
            OnAttack();
            isTriggered = false;
        }
        else
        {
            navMeshAgent.isStopped = false;
        }
    }

    public override void OnAttack() // 공격할 때
    {
        base.OnAttack();
        myAnim.SetTrigger(animData.OnAttack);

        if (targetMonster.TryGetComponent<IDamage>(out IDamage monsterDamage))
        {
            monsterDamage.OnDamage(battleStat.Attak);
        }
    }

    public new void OnDamage(float dmg) // 데미지 입을 때
    {
        float effectiveDmg = dmg;

        if (Armor > 0)
        {
            effectiveDmg -= Armor;
            Armor = Mathf.Max(0, Armor - dmg);
        }

        curHp -= effectiveDmg;

        if (curHp <= 0.0f)
        {
            myAnim.SetTrigger(animData.OnDead);
            OnDead();
        }
        else
        {
            myAnim.SetTrigger(animData.OnDamage);
        }
    }

    public void PlayAnimationAndApplyEffect(Card card, System.Action onEffectComplete)
    {
        StartCoroutine(PlayAnimationAndEffectCoroutine(card, onEffectComplete));
    }

    private IEnumerator PlayAnimationAndEffectCoroutine(Card card, System.Action onEffectComplete)
    {
        // 카드 타입에 따른 애니메이션 실행
        switch (card.cardType)
        {
            case CardType.Attack:
                myAnim.SetTrigger(animData.OnAttack);
                break;

            case CardType.Defense:
                myAnim.SetTrigger(animData.OnDamage);
                break;

            case CardType.Skill:
                myAnim.SetTrigger(animData.MyTurn);
                break;
        }

        // 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(myAnim.GetCurrentAnimatorStateInfo(0).length);

        // 카드 효과 적용 후 콜백 실행
        onEffectComplete?.Invoke();
    }
}
