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

        public event Action<IReadOnlyDictionary<Vector2Int, IBubbleTileModel>> onBubbleTileDictUpdated
        {
            add { _bubbleModel.onBubbleTileDictUpdated += value; }
            remove { _bubbleModel.onBubbleTileDictUpdated -= value; }
        }
        public event Action<Vector2Int> onBubbleTileAdded
        {
            add { _bubbleModel.onBubbleTileAdded += value; }
            remove { _bubbleModel.onBubbleTileAdded -= value; }
        }
        public event Action onBubbleSequeceComplete;
        public event Func<Vector2Int, HashSet<Vector2Int>, IEnumerable<Vector2Int>> requestGettingNeighborIndexList;

        private List<Vector2Int> _sameBubbleTileIndexList;
        private HashSet<Vector2Int> _visitedIndexSet;
        private Queue<Vector2Int> _sameBubbleTileQueue;

        private void Awake()
        {
            _sameBubbleTileIndexList = new();
            _visitedIndexSet = new();
            _sameBubbleTileQueue = new();
        }

        internal void UpdateFlowTileListDict(IReadOnlyDictionary<Color, LinkedList<IFlowTileModel>> colorTileListDict)
        {
            _bubbleModel.FillBubbleTileList(colorTileListDict, GetRandomBubbleTileColor);

            // A-1. 버블 생성 연출
            Sequence totalSequence = DOTween.Sequence();
            foreach (IBubbleTileModel tileModel in _bubbleModel.BubbleTileList)
            {
                BubbleTilePathNode headPathNode = tileModel.HeadPathNode;
                Color bubbleColor = tileModel.BubbleColor;
                Vector2Int tileIndex = tileModel.TileIndex;
                BubbleTile bubbleTile = _bubbleView.CreateBubbleTile(tileIndex, bubbleColor, headPathNode.TilePosition);

                Sequence sequence = DOTween.Sequence();
                float moveDuration = _bubbleView.MoveDuration;
                float fadeDuration = moveDuration * .5f;
                foreach (BubbleTilePathNode node in tileModel.MovementPathNodeList)
                {
                    // A-2. 생성된 지점에서는 연출 생략 (bubbleTile.SpriteRenderer.color.a 값이 0 인 상태로 존재)
                    if (bubbleTile.CachedTransform.position == node.TilePosition)
                    {
                        continue;
                    }

                    Vector2 tilePosition = node.TilePosition;
                    int turn = node.Turn;
                    sequence.Insert(turn * moveDuration,
                                    bubbleTile.CachedTransform.DOMove(tilePosition, moveDuration).SetEase(Ease.Linear));
                    // A-3. 이동을 시작할 때 FadeIn 연출
                    if (bubbleTile.SpriteRenderer.color.a == 0f)
                    {
                        sequence.Join(bubbleTile.SpriteRenderer.DOFade(1f, fadeDuration));
                    }
                }
                totalSequence.Join(sequence);
            }
            totalSequence.SetEase(_sequenceEase);
            totalSequence.OnComplete(OnBubbleSequenceComplete);
        }

        private void OnBubbleSequenceComplete()
        {
            foreach (Vector2Int bubbleTileIndex in _sameBubbleTileIndexList)
            {
                _bubbleView.RemoveBubbleTile(bubbleTileIndex);
            }
            _sameBubbleTileIndexList.Clear();

            onBubbleSequeceComplete?.Invoke();
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

        internal void AddBubbleTile(Vector2Int tileIndex, BubbleTile bubbleTile)
        {
            _bubbleModel.AddBubbleTile(tileIndex, bubbleTile.BubbleColor);
            _bubbleView.AddBubbleTile(tileIndex, bubbleTile);

            _sameBubbleTileIndexList.Clear();
            _visitedIndexSet.Clear();
            _sameBubbleTileQueue.Clear();

            AddSameColorBubbleTileQueue(tileIndex);

            Color bubbleColor = _bubbleModel.GetBubbleColor(tileIndex);
            while (_sameBubbleTileQueue.Count > 0)
            {
                Vector2Int current = _sameBubbleTileQueue.Dequeue();
                _sameBubbleTileIndexList.Add(current);

                foreach (Vector2Int neighborIndex in requestGettingNeighborIndexList(current, _visitedIndexSet))
                {
                    if (_bubbleModel.GetBubbleColor(neighborIndex) == bubbleColor)
                    {
                        _sameBubbleTileQueue.Enqueue(neighborIndex);
                        _visitedIndexSet.Add(neighborIndex);
                    }
                }
            }

            // 같은 색상의 버블이 3개 이상인 경우 제거
            if (_sameBubbleTileIndexList.Count >= 3)
            {
                float moveDuration = _bubbleView.MoveDuration;
                float fadeDuration = moveDuration * .5f;
                Sequence sequence = DOTween.Sequence();
                foreach (Vector2Int bubbleTileIndex in _sameBubbleTileIndexList)
                {
                    _bubbleModel.RemoveBubbleTile(bubbleTileIndex);
                    BubbleTile removingTile = _bubbleView.GetBubbleTile(bubbleTileIndex);
                    if (removingTile != null)
                    {
                        sequence.Join(removingTile.SpriteRenderer.DOFade(0f, fadeDuration));
                    }
                }
                sequence.OnComplete(OnBubbleSequenceComplete);
            }
            else
            {
                OnBubbleSequenceComplete();
            }
        }

        private void AddSameColorBubbleTileQueue(Vector2Int tileIndex)
        {
            _sameBubbleTileQueue.Enqueue(tileIndex);
            _visitedIndexSet.Add(tileIndex);
        }
    }
}
