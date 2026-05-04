using System;
using DG.Tweening;
using Nova;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
//This script manages the viusals and animations behind the dialogue block 
public class DialogueVisuals
{
    public UIBlock2D Background;
    public UIBlock2D Icon;
    public TextBlock DialogueText;
    public TextBlock NameText;
    public float PopinDuration = 0.35f;
    public float OriginalScale = 96f;

    public void Show()
    {
        Background.Visible = true;
        Background.transform.DOScale(OriginalScale, PopinDuration).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        Background.transform.DOScale(Vector3.zero, PopinDuration).SetEase(Ease.InBack).OnComplete(() =>
        {
            Background.Visible = false;
        });
    }
}
