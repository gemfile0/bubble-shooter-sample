using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample
{
    public class BubblePresenter : MonoBehaviour
    {
        [SerializeField] private BubbleModel _bubbleModel;
        [SerializeField] private BubbleView _bubbleView;
        [SerializeField] private GridView _gridView;
        [SerializeField] private Ease _sequenceEase = Ease.OutSine;

        internal void UpdateFlowTileListDict(IReadOnlyDictionary<Color, LinkedList<IFlowTileModel>> colorTileListDict)
        {
            _bubbleModel.FillBubbleTileList(colorTileListDict);

            foreach (IBubbleTileModel tileModel in _bubbleModel.BubbleTileList)
            {
                BubbleTilePathNode headPathNode = tileModel.HeadPathNode;
                BubbleTile bubbleTile = _bubbleView.CreateBubble(headPathNode.tilePosition);

                Sequence sequence = DOTween.Sequence();
                float moveDuration = _bubbleView.MoveDuration;
                float fadeDuration = moveDuration * .5f;
                foreach (BubbleTilePathNode node in tileModel.MovementPathNodeList)
                {
                    Vector2 tilePosition = node.tilePosition;
                    int turn = node.turn;
                    sequence.Insert(turn * moveDuration,
                                    bubbleTile.CachedTransform.DOMove(tilePosition, moveDuration).SetEase(Ease.Linear));
                    if (bubbleTile.SpriteRenderer.color.a == 0f)
                    {
                        sequence.Join(bubbleTile.SpriteRenderer.DOFade(1f, fadeDuration));
                    }
                }
                sequence.SetEase(_sequenceEase);
            }
        }
    }
}
