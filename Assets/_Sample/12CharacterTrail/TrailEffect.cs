using UnityEngine;
using System.Collections;

namespace MyVfx
{
    /// <summary>
    /// 플레이어 동작에 따라 잔상 이펙트 효과 구현
    /// </summary>
    public class TrailEffect : MonoBehaviour
    {
        #region Variables
        //참조
        //private SpriteRenderer spriteRenderer;
        private SkinnedMeshRenderer[] skinnedMeshRenderers;

        //잔상 메터리얼
        public Material ghostMaterial;

        private Transform ghostTransform;   //고스트 오브젝트가 생성할 위치에 있는 오브젝트

        //잔상 효과
        private bool isTrailActive = false; //효과 활성/비활성

        [SerializeField] private float trailActiveTime = 2f; //효과 지속 시간
        [SerializeField] private float trailRefreshRate = 0.1f;  //잔상들의 발생 간격 시간
        [SerializeField] private float trailDestroyDelay = 1f;   //1초후에 킬 - 페이드 아웃 효과

        //잔상 페이드 아웃 효과
        private string shaderValueRef = "_Alpha";
        [SerializeField] private float shaderValueRate = 0.1f;       //알파값 감소 비율
        [SerializeField] private float shaderValueRefeshRate = 0.1f; //알파값이 감소 되는 시간 간격
        #endregion


        #region Unity Event Method
        private void Awake()
        {
            //참조
            //spriteRenderer = GetComponent<SpriteRenderer>();
            skinnedMeshRenderers = this.GetComponentsInChildren<SkinnedMeshRenderer>();
        }

        private void Update()
        {
            //마우스 우클릭하면 트래일 이펙트 시작
            if (Input.GetMouseButton(1))
            {
                StartTrailEffect();
            }
        }
        #endregion

        #region Custom Method
        //잔상 효과 플레이
        public void StartTrailEffect()
        {
            //현재 효과 진행중이면 리턴
            if (isTrailActive)
                return;

            StartCoroutine(ActiveTrail(trailActiveTime));
        }

        //매개변수 효과 지속시간
        IEnumerator ActiveTrail(float activeTime)
        {
            isTrailActive = true;

            while(activeTime > 0f)
            {
                activeTime -= trailRefreshRate;

                //잔상 게임 오브젝트 만들기 - 현재 플레이어의 위치에 skinnedMeshRenderers 숫자만큼 만든다
                for (int i = 0; i < skinnedMeshRenderers.Length; i++)
                {
                    //하이라키창에 빈 오브젝트 만들기
                    GameObject ghostObejct = new GameObject();

                    //트랜스폼 셋팅 - 플레이어의 트랜스폼과 동일
                    ghostTransform = skinnedMeshRenderers[i].transform;
                    ghostObejct.transform.SetPositionAndRotation(ghostTransform.position, ghostTransform.rotation);
                    ghostObejct.transform.localScale = ghostTransform.localScale;

                    //skinnedMeshRenderers 셋팅
                    MeshRenderer ghostRenderer = ghostObejct.AddComponent<MeshRenderer>();
                    MeshFilter meshFilter = ghostObejct.AddComponent<MeshFilter>();
                    Mesh mesh = new Mesh();
                    skinnedMeshRenderers[i].BakeMesh(mesh);
                    meshFilter.mesh = mesh;
                    ghostRenderer.material = ghostMaterial;

                    //페이드 아웃 효과
                    StartCoroutine(AnimateMaterialFloat(ghostRenderer.material, shaderValueRef, 0f, shaderValueRate, shaderValueRefeshRate));

                    //고스트오브젝트 킬
                    Destroy(ghostObejct, trailDestroyDelay);
                }

                //딜레이
                yield return new WaitForSeconds(trailRefreshRate);
            }

            //효과 해제
            isTrailActive = false;
        }

        //메터리얼 속성(알파) 값 감소
        IEnumerator AnimateMaterialFloat(Material material, string valueRef, float goal, float rate, float refreshRate)
        {
            float value = material.GetFloat(valueRef);

            while (value > goal)
            {
                value -= rate;
                if(value < goal)
                {
                    value = goal;
                }

                material.SetFloat(valueRef, value);

                yield return new WaitForSeconds(refreshRate);
            }
        }
        #endregion
    }
}