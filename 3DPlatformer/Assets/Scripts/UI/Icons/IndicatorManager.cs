using System;
using DG.Tweening;
using Nova;
using UnityEngine;

public class IndicatorManager : MonoBehaviour
{
    [SerializeField] private float scaleDuration = 0.5f;
    [SerializeField] private float height = 1.0f;
    [SerializeField] private bool floating = true;
    
    [HideInInspector]
    public Sprite icon;
    [HideInInspector]
    public Material iconMaterial;
    
    private SpriteRenderer renderer;
    private Vector3 startScale;

    private void Awake()
    {
        startScale = transform.localScale;
        transform.localScale = Vector3.zero;
        renderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        transform.localScale = Vector3.zero;
        if (floating)
        {
            transform.DOLocalMoveY(height, 1f).SetLoops(-1,  LoopType.Yoyo).SetEase(Ease.InOutQuad);
        }
    }

    public void ShowIndictor()
    {
        if (icon != null)
        {
            renderer.sprite = icon;
        }

        if (iconMaterial != null)
        {
            renderer.material = iconMaterial;
        }
        transform.DOScale(startScale, scaleDuration).SetEase(Ease.OutCubic);
    }

    public void HideIndictor()
    {
        transform.DOScale(Vector3.zero, scaleDuration).SetEase(Ease.OutCubic);
    }

}
