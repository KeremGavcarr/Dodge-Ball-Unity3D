using UnityEngine;
using System.Collections.Generic;

public class ClothesManager : MonoBehaviour
{
    [Header("Kıyafet Listesi")]
    public List<GameObject> clothesList = new List<GameObject>();

    private SkinnedMeshRenderer mainRenderer;

    void Awake()
    {
        // Script karakterin üzerindeyse, otomatik olarak kendi SkinnedMeshRenderer'ını bulur
        mainRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        
        if (mainRenderer == null)
        {
            Debug.LogError(gameObject.name + " üzerinde ana iskelet bulunamadı!");
            return;
        }

        RefreshClothes();
    }

    public void RefreshClothes()
    {
        foreach (GameObject clothing in clothesList)
        {
            if (clothing == null || !clothing.activeSelf) continue;

            SkinnedMeshRenderer[] meshes = clothing.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer smr in meshes)
            {
                smr.bones = mainRenderer.bones;
                smr.rootBone = mainRenderer.rootBone;
            }

            if (clothing.TryGetComponent<Animator>(out Animator anim))
            {
                anim.enabled = false;
            }
        }
    }
}