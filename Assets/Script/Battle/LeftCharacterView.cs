using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class LeftCharacterView : BattleCharacterView
{
    public override async UniTask PlayAttack()
    {
        PlaySE(attackSE);

        await rect.DOAnchorPos(originalPos + new Vector2(120f, 0), 0.15f)
            .SetEase(Ease.OutQuad)
            .AsyncWaitForCompletion();

        await rect.DOAnchorPos(originalPos, 0.2f)
            .SetEase(Ease.OutBack)
            .AsyncWaitForCompletion();
    }
}