using UnityEngine;

[ExecuteInEditMode]
public class CustomImageEffect : MonoBehaviour
{
    public Material EffectMaterial;
    private bool isAnimating;
    private float currentVal;
    private float targetVal;
    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        Graphics.Blit(src, dest, EffectMaterial);
    }

    public void ActivateEffect(bool isTrue){
        if (isTrue) {
            targetVal = 1;
        }
        else {
            targetVal = 0;
        }
    }

    public void Start(){
        EffectMaterial.SetFloat("_AnimationOffset", 0);
    }

    public void Update(){
        if (currentVal != targetVal) {
            currentVal = Mathf.Lerp(currentVal, targetVal, 0.1f);
            EffectMaterial.SetFloat("_AnimationOffset", currentVal);
        }
    }
}
