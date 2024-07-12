using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public class BubblePresenter : MonoBehaviour
    {
        [SerializeField] private BubbleModel _bubbleModel;
        [SerializeField] private BubbleView _bubbleView;
        [SerializeField] private Ease _sequenceEase = Ease.OutSine;
        [SerializeField]
        private List<Color> _bubbleTileColorList = new()
        {
            Color.yellow,
            Color.red,
            Color.blue,
            new Color(0.8f, 0.8f, 0.8f) // lightening color (밝은 회색)
        };

        public event Action<IReadOnlyCollection<Vector2Int>> onBubbleTileSetUpdated
        {
            add { _bubbleModel.onBubbleTileSetUpdated += value; }
            remove { _bubbleModel.onBubbleTileSetUpdated -= value; }
        }
        public event Action<Vector2Int> onBubbleTileAdded
        {
            add { _bubbleModel.onBubbleTileAdded += value; }
            remove { _bubbleModel.onBubbleTileAdded -= value; }
        }

        internal void UpdateFlowTileListDict(IReadOnlyDictionary<Color, LinkedList<IFlowTileModel>> colorTileListDict)
        {
            _bubbleModel.FillBubbleTileList(colorTileListDict, GetRandomBubbleTileColor);

            // 버블 생성 연출
            foreach (IBubbleTileModel tileModel in _bubbleModel.BubbleTileList)
            {
                BubbleTilePathNode headPathNode = tileModel.HeadPathNode;
                Color bubbleColor = tileModel.BubbleColor;
                BubbleTile bubbleTile = _bubbleView.CreateBubbleTile(bubbleColor, headPathNode.TilePosition);

                Sequence sequence = DOTween.Sequence();
                float moveDuration = _bubbleView.MoveDuration;
                float fadeDuration = moveDuration * .5f;
                foreach (BubbleTilePathNode node in tileModel.MovementPathNodeList)
                {
                    Vector2 tilePosition = node.TilePosition;
                    int turn = node.Turn;
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
            int randomIndex = UnityEngine.Random.Range(0, _bubbleTileColorList.Count);
            return _bubbleTileColorList[randomIndex];
        }

        public BubbleTile CreateBubbleTile(Vector2 tilePosition)
        {
            Color bubbleColor = GetRandomBubbleTileColor();
            BubbleTile bubbleTile = _bubbleView.CreateBubbleTile(bubbleColor, tilePosition);
            return bubbleTile;
        }

        internal void AddBubbleTile(Vector2Int tileIndex)
        {
            _bubbleModel.AddBubbleTile(tileIndex);
        }
    }
}
