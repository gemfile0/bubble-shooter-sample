using System;
using UnityEngine;

namespace BubbleShooterSample
{
    public class GateKeeperPresenter : MonoBehaviour,
        ILootAnimationEndPoint
    {
        [SerializeField] private GateKeeperModel _gateKeeperModel;
        [SerializeField] private GateKeeperView _gateKeeperView;

        public event Func<Vector2Int, Vector3> requestGridTilePosition;

        private void Start()
        {
            Vector3 gridTilePosition = requestGridTilePosition(new Vector2Int(5, 1));
            _gateKeeperView.CachedTransform.position = gridTilePosition;
        }

        public void BeginLootAnimation()
        {

        }

        public void EndLootAnimation(long bonusCount)
        {

        }

        public Vector3 GetLootAnimtionEndPoint()
        {
            return _gateKeeperView.CachedTransform.position;
        }
    }
}
