using BubbleShooterSample.GameData;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public class BubblePresenter : MonoBehaviour, ILootAnimationTrigger
    {
        [SerializeField] private BubbleData _bubbleData;
        [SerializeField] private BubbleModel _bubbleModel;
        [SerializeField] private BubbleView _bubbleView;

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
        public event Action<LootAnimationType, LootAnimationInfo> requestLootAnimation;

        // 같은 색상의 버블을 제거하기 위한 용도
        private HashSet<Vector2Int> _visitedIndexSet;
        private Queue<Vector2Int> _sameBubbleTileQueue;
        private List<(Vector2Int, int)> _sameBubbleTileIndexList;

        private List<TaskCompletionSource<bool>> _tcsList;

        // 뿌리로부터 연결된 버블을 찾기 위한 용도
        private HashSet<Vector2Int> _connectedIndexSet;
        private Queue<Vector2Int> _connectedTileQueue;
        private List<(Vector2Int, int)> _unconnectedTileIndexList;

        private void Awake()
        {
            _sameBubbleTileIndexList = new();
            _visitedIndexSet = new();
            _sameBubbleTileQueue = new();

            _tcsList = new();

            _connectedIndexSet = new();
            _connectedTileQueue = new();
            _unconnectedTileIndexList = new();
        }

        internal void UpdateFlowTileListDict(IReadOnlyDictionary<Color, LinkedList<IFlowTileModel>> colorTileListDict)
        {
            _bubbleModel.FeedBubbleTileList(colorTileListDict, GetRandomBubbleTileColor);

            StartCoroutine(FeedBubblesCoroutine());
        }

        private IEnumerator FeedBubblesCoroutine()
        {
            yield return FeedBubbles();

            OnBubbleSequenceComplete();
        }

        private IEnumerator FeedBubbles()
        {
            // A-1. 버블 생성 연출
            Sequence totalSequence = DOTween.Sequence();
            foreach (IBubbleTileModel tileModel in _bubbleModel.BubbleTileDict.Values)
            {
                BubbleTilePathNode? firstPathNode = tileModel.FirstPathNode;
                if (firstPathNode == null)
                {
                    continue;
                }

                //Debug.Log($"=====");
                int tileID = tileModel.TileID;
                Color bubbleColor = tileModel.BubbleColor;
                int attackPoint = tileModel.AttackPoint;
                BubbleTile bubbleTile = _bubbleView.GetOrCreateBubbleTile(tileID, bubbleColor, attackPoint > 0, firstPathNode.Value.NextTilePosition);

                Sequence sequence = DOTween.Sequence();
                float moveDuration = _bubbleData.MoveBubbleDuration;
                float fadeDuration = _bubbleData.FadeBubbleDuration;
                //Debug.Log($"FeedBubbles 1 : {tileID}");
                foreach (BubbleTilePathNode node in tileModel.PathNodeQueue)
                {
                    int turn = node.Turn;
                    Vector2 prevTilePosition = node.PrevTilePosition;
                    Vector2 nextTilePosition = node.NextTilePosition;

                    // A-2. 생성된 지점에서는 연출 생략 (bubbleTile.SpriteRenderer.color.a 값이 0 인 상태로 존재)
                    if ((Vector2)bubbleTile.CachedTransform.position == nextTilePosition)
                    {
                        continue;
                    }

                    //Debug.Log($"FeedBubbles 2 : {turn}, {node.NextTileIndex}, {bubbleTile.CachedTransform.position} -> {node.NextTilePosition}");
                    sequence.InsertCallback(turn * moveDuration, () =>
                    {
                        bubbleTile.CachedTransform.position = prevTilePosition;
                    });
                    sequence.Insert(turn * moveDuration,
                                    bubbleTile.CachedTransform.DOMove(nextTilePosition, moveDuration).SetEase(Ease.Linear));

                    // A-3. 이동을 시작할 때 FadeIn 연출
                    if (bubbleTile.IsAlphaValue(0f))
                    {
                        foreach (SpriteRenderer spriteRenderer in bubbleTile.SpriteRendererList)
                        {
                            sequence.Join(spriteRenderer.DOFade(1f, fadeDuration));
                        }
                    }
                }
                tileModel.ConsumePathNodeList();
                totalSequence.Insert(0f, sequence);
            }
            totalSequence.SetEase(_bubbleData.FeedBubbleSequenceEase);
            yield return totalSequence.WaitForCompletion();
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
            if (_connectedIndexSet.Count != _bubbleModel.BubbleTileDict.Count)
            {
                foreach (IBubbleTileModel tileModel in _bubbleModel.BubbleTileDict.Values)
                {
                    Vector2Int tileIndex = tileModel.TileIndex;
                    int tileID = tileModel.TileID;
                    if (_connectedIndexSet.Contains(tileIndex))
                    {
                        continue;
                    }

                    _unconnectedTileIndexList.Add((tileIndex, tileID));
                }

                //Sequence sequence = DOTween.Sequence();
                float fadeDuration = _bubbleData.FadeBubbleDuration;
                Vector2 droppingForceRange = _bubbleData.DroppingForceRange;
                float droppingGravityScale = _bubbleData.DroppingGravityScale;
                foreach ((Vector2Int tileIndex, int tileID) in _unconnectedTileIndexList)
                {
                    _bubbleModel.RemoveBubbleTile(tileIndex);
                    BubbleTile removingTile = _bubbleView.GetBubbleTile(tileID);

                    float droppingForce = UnityEngine.Random.Range(droppingForceRange.x, droppingForceRange.y);
                    removingTile.Drop(droppingForce, droppingGravityScale);
                    //removingTile.DOKill();
                    foreach (SpriteRenderer spriteRenderer in removingTile.SpriteRendererList)
                    {
                        //sequence.Join(spriteRenderer.DOFade(0f, fadeDuration));
                    }
                }
                yield return new WaitForSeconds(3f);
                //yield return sequence.WaitForCompletion();

                //foreach ((Vector2Int tileIndex, int tileID) in _unconnectedTileIndexList)
                //{
                //    _bubbleView.RemoveBubbleTile(tileID);
                //}
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
            return _bubbleData.GetRandomBubbleTileColor();
        }

        // BubbleShooter 로부터 호출
        public BubbleTile CreateBubbleTile(Vector2 tilePosition)
        {
            Color bubbleColor = GetRandomBubbleTileColor();
            BubbleTile bubbleTile = _bubbleView.CreateBubbleTile(bubbleColor, hasAttackPoint: false, tilePosition);
            return bubbleTile;
        }

        internal void AddBubbleTile(Vector2Int tileIndex, BubbleTile bubbleTile)
        {
            _bubbleModel.AddBubbleTile(tileIndex, bubbleTile.BubbleColor);
            int tileID = _bubbleModel.GetBubbleTile(tileIndex).TileID;
            _bubbleView.AddBubbleTile(tileID, bubbleTile);

            StartCoroutine(RemoveBubblesCoroutine(tileIndex));
        }

        private IEnumerator RemoveBubblesCoroutine(Vector2Int tileIndex)
        {
            yield return RemoveSameBubbles(tileIndex);

            yield return DropUnconnectedBubbles();

            foreach (var tcs in _tcsList)
            {
                yield return new WaitUntil(() => tcs.Task.IsCompleted);
            }

            _bubbleModel.FeedBubbleTileList();
            yield return FeedBubbles();

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
                int tileID = _bubbleModel.GetBubbleTile(current).TileID;
                _sameBubbleTileIndexList.Add((current, tileID));

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
                _tcsList.Clear();

                float fadeDuration = _bubbleData.FadeBubbleDuration;
                Sequence sequence = DOTween.Sequence();
                foreach ((Vector2Int bubbleTileIndex, int tileID) in _sameBubbleTileIndexList)
                {
                    IBubbleTileModel removedTileModel = _bubbleModel.RemoveBubbleTile(bubbleTileIndex);
                    BubbleTile removingTile = _bubbleView.GetBubbleTile(tileID);
                    removingTile.DOKill();
                    foreach (SpriteRenderer spriteRenderer in removingTile.SpriteRendererList)
                    {
                        sequence.Join(spriteRenderer.DOFade(0f, fadeDuration));
                    }

                    int attackPoint = removedTileModel.AttackPoint;
                    if (attackPoint > 0)
                    {
                        var tcs = new TaskCompletionSource<bool>();
                        _tcsList.Add(tcs);

                        var lootAnimationInfo = new LootAnimationInfo(
                            attackPoint,
                            removingTile.CachedTransform.position,
                            removingTile.CachedTransform.rotation,
                            onComplete: () => tcs.SetResult(true)
                        );
                        requestLootAnimation(LootAnimationType.AttackPoint, lootAnimationInfo);
                    }
                }
                yield return sequence.WaitForCompletion();

                foreach ((Vector2Int bubbleTileIndex, int tileID) in _sameBubbleTileIndexList)
                {
                    _bubbleView.RemoveBubbleTile(tileID);
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
