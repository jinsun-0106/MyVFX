using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

namespace MyVfx
{
    public class Levelup : MonoBehaviour
    {
        #region Variables
        //참조
        private Animator animator;

        public VisualEffect levelup;    //레벨업 vfx 이펙트
        public Renderer bodyRenderer;   //메터리얼을 관리하는 랜더러
        public Material glowMaterial;   //레벨업하는 동안 바꿀 메터리얼

        private Material originMaterial;    //원래 메터리얼

        private bool isLevelup = false;     //레벨업 이펙트 플레이 체크

        private const string ParamLevelup = "Levelup";
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            //참조
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            //초기화
            isLevelup = false;
            originMaterial = bodyRenderer.material;
        }

        private void Update()
        {
            //마우스 우클릭하면 레벨업 치팅
            if(Input.GetMouseButtonDown(1) && isLevelup == false)
            {
                StartCoroutine(LevelupEffect());
            }
        }
        #endregion

        #region Custom Method
        IEnumerator LevelupEffect()
        {
            isLevelup = true;

            //레벨업 애니 및 vfx 이펙트 시작
            animator.SetTrigger(ParamLevelup);
            levelup.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(0.2f);

            //메터리얼 바꿔치기
            bodyRenderer.material = glowMaterial;
            yield return new WaitForSeconds(0.8f);

            bodyRenderer.material = originMaterial;
            yield return new WaitForSeconds(2f);

            //레벨업 이펙트 초기화
            levelup.gameObject.SetActive(false);
            isLevelup = false;
        }
        #endregion
    }
}