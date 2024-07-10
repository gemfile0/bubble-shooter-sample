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
        [SerializeField]
        private List<Color> _bubbleTileColorList = new()
        {
            Color.yellow,
            Color.red,
            Color.blue,
            new Color(0.8f, 0.8f, 0.8f) // lightening color (밝은 회색)
        };

        internal void UpdateFlowTileListDict(IReadOnlyDictionary<Color, LinkedList<IFlowTileModel>> colorTileListDict)
        {
            _bubbleModel.FillBubbleTileList(colorTileListDict, GetRandomBubbleTileColor);

            foreach (IBubbleTileModel tileModel in _bubbleModel.BubbleTileList)
            {
                BubbleTilePathNode headPathNode = tileModel.HeadPathNode;
                Color bubbleColor = tileModel.BubbleColor;
                BubbleTile bubbleTile = _bubbleView.CreateBubbleTile(bubbleColor, headPathNode.tilePosition);

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

        public Color GetRandomBubbleTileColor()
        {
            int randomIndex = Random.Range(0, _bubbleTileColorList.Count);
            return _bubbleTileColorList[randomIndex];
        }

        public BubbleTile CreateBubbleTile(Vector2 tilePosition)
        {
            Color bubbleColor = GetRandomBubbleTileColor();
            BubbleTile bubbleTile = _bubbleView.CreateBubbleTile(bubbleColor, tilePosition);
            return bubbleTile;
        }
    }
}
