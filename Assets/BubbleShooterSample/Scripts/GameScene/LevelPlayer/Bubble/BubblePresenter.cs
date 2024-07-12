using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public event Action<Vector2Int> onBubbleTileRemoved
        {
            add { _bubbleModel.onBubbleTileRemoved += value; }
            remove { _bubbleModel.onBubbleTileRemoved -= value; }
        }
        public event Action onBubbleSequeceComplete;
        public event Func<Vector2Int, HashSet<Vector2Int>, IEnumerable<Vector2Int>> requestGettingNeighborIndexList;

        // 같은 색상의 버블을 제거하기 위한 용도
        private HashSet<Vector2Int> _visitedIndexSet;
        private Queue<Vector2Int> _sameBubbleTileQueue;
        private List<Vector2Int> _sameBubbleTileIndexList;

        // 뿌리로부터 연결된 버블을 찾기 위한 용도
        private HashSet<Vector2Int> _connectedIndexSet;
        private Queue<Vector2Int> _connectedTileQueue;
        private List<Vector2Int> _unconnectedTileIndexList;

        private void Awake()
        {
            _sameBubbleTileIndexList = new();
            _visitedIndexSet = new();
            _sameBubbleTileQueue = new();

            _connectedIndexSet = new();
            _connectedTileQueue = new();
            _unconnectedTileIndexList = new();
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
            onBubbleSequeceComplete?.Invoke();
        }

        private IEnumerator DropUnconnectedBubbles()
        {
            _connectedIndexSet.Clear();
            _connectedTileQueue.Clear();
            _unconnectedTileIndexList.Clear();

            // 루트 인덱스에서 연결된 버블을 찾기
            foreach (Vector2Int rootIndex in _bubbleModel.RootIndexSet)
            {
                AddConnectedTileQueue(rootIndex);
            }

            while (_connectedTileQueue.Count > 0)
            {
                Vector2Int current = _connectedTileQueue.Dequeue();

                foreach (Vector2Int neighborIndex in requestGettingNeighborIndexList(current, _connectedIndexSet))
                {
                    if (_bubbleModel.ContainsBubbleTile(neighborIndex))
                    {
                        AddConnectedTileQueue(neighborIndex);
                    }
                }
            }

            // 연결되지 않은 버블 제거
            if (_connectedIndexSet.Count != _bubbleModel.BubbleTileList.Count)
            {
                float fadeDuration = _bubbleView.MoveDuration * .5f;

                foreach (IBubbleTileModel tileModel in _bubbleModel.BubbleTileList)
                {
                    Vector2Int tileIndex = tileModel.TileIndex;
                    if (_connectedIndexSet.Contains(tileIndex))
                    {
                        continue;
                    }

                    _unconnectedTileIndexList.Add(tileIndex);
                }

                Sequence sequence = DOTween.Sequence();
                foreach (Vector2Int tileIndex in _unconnectedTileIndexList)
                {
                    _bubbleModel.RemoveBubbleTile(tileIndex);
                    BubbleTile removingTile = _bubbleView.GetBubbleTile(tileIndex);
                    if (removingTile != null)
                    {
                        removingTile.DOKill();
                        sequence.Join(removingTile.SpriteRenderer.DOFade(0f, fadeDuration));
                    }
                }
                yield return sequence.WaitForCompletion();

                foreach (Vector2Int bubbleTileIndex in _unconnectedTileIndexList)
                {
                    _bubbleView.RemoveBubbleTile(bubbleTileIndex);
                }
            }

            _unconnectedTileIndexList.Clear();
            yield break;
        }

        private void AddConnectedTileQueue(Vector2Int tileIndex)
        {
            _connectedIndexSet.Add(tileIndex);
            _connectedTileQueue.Enqueue(tileIndex);
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

            StartCoroutine(RemoveBubblesCoroutine(tileIndex));

        }

        private IEnumerator RemoveBubblesCoroutine(Vector2Int tileIndex)
        {
            yield return RemoveSameBubbles(tileIndex);

            yield return DropUnconnectedBubbles();

            OnBubbleSequenceComplete();
        }

        private IEnumerator RemoveSameBubbles(Vector2Int tileIndex)
        {
            _visitedIndexSet.Clear();
            _sameBubbleTileQueue.Clear();
            _sameBubbleTileIndexList.Clear();

            AddSameBubbleTileQueue(tileIndex);

            Color bubbleColor = _bubbleModel.GetBubbleTileColor(tileIndex);
            while (_sameBubbleTileQueue.Count > 0)
            {
                Vector2Int current = _sameBubbleTileQueue.Dequeue();
                _sameBubbleTileIndexList.Add(current);

                foreach (Vector2Int neighborIndex in requestGettingNeighborIndexList(current, _visitedIndexSet))
                {
                    if (_bubbleModel.GetBubbleTileColor(neighborIndex) == bubbleColor)
                    {
                        AddSameBubbleTileQueue(neighborIndex);
                    }
                }
            }

            // 같은 색상의 버블이 3개 이상인 경우 제거
            if (_sameBubbleTileIndexList.Count >= 3)
            {
                float fadeDuration = _bubbleView.MoveDuration * .5f;
                Sequence sequence = DOTween.Sequence();
                foreach (Vector2Int bubbleTileIndex in _sameBubbleTileIndexList)
                {
                    _bubbleModel.RemoveBubbleTile(bubbleTileIndex);
                    BubbleTile removingTile = _bubbleView.GetBubbleTile(bubbleTileIndex);
                    if (removingTile != null)
                    {
                        removingTile.DOKill();
                        sequence.Join(removingTile.SpriteRenderer.DOFade(0f, fadeDuration));
                    }
                }
                yield return sequence.WaitForCompletion();

                foreach (Vector2Int bubbleTileIndex in _sameBubbleTileIndexList)
                {
                    _bubbleView.RemoveBubbleTile(bubbleTileIndex);
                }
            }

            _sameBubbleTileIndexList.Clear();
        }

        private void AddSameBubbleTileQueue(Vector2Int tileIndex)
        {
            _sameBubbleTileQueue.Enqueue(tileIndex);
            _visitedIndexSet.Add(tileIndex);
        }
    }
}
