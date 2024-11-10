using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheonJiWoon
{
    public class Enviroment : MonoBehaviour
    {
        public Transform startPoint;
        public Transform endPoint;
        public Transform moveObjs;
        float moveSpeed = 4.0f;

        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < moveObjs.childCount; i++)
            {
                Transform child = moveObjs.GetChild(i);
                child.position = RandomPosition(child);
                child.Rotate(Vector3.up * Random.Range(0.0f, 360.0f));
                StartCoroutine(Moving(child));
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator Moving(Transform tr)
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(1.5f, 4.5f));

                Vector3 targetPos = RandomPosition(tr);
                Vector3 dir = targetPos - tr.position;
                float dist = dir.magnitude;
                dir.Normalize();
                float rot = Vector3.Dot(tr.right, dir) < 0 ? -1.0f : 1.0f;
                tr.Rotate(Vector3.up * Vector3.Angle(tr.forward, dir) * rot);


                while (dist > 0.0f)
                {
                    float delta = moveSpeed * Time.deltaTime;
                    tr.Translate(tr.forward * delta, Space.World);
                    dist -= delta;
                    yield return null;
                }
            }
        }

        Vector3 RandomPosition(Transform orgin) { 
            return new Vector3(Random.Range(startPoint.position.x, endPoint.position.x), orgin.position.y, Random.Range(startPoint.position.z, endPoint.position.z));
        }

    }
}

