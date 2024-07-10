using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample
{
    public class BubblePresenter : MonoBehaviour
    {
        [SerializeField] private BubbleModel _bubbleModel;
        [SerializeField] private BubbleView _bubbleView;
        [SerializeField] private GridView _gridView;

        internal void UpdateFlowTileListDict(IReadOnlyDictionary<Color, LinkedList<IFlowTileModel>> colorTileListDict)
        {
            _bubbleModel.FillBubbleTileList(colorTileListDict);

            foreach (IBubbleTileModel tileModel in _bubbleModel.BubbleTileList)
            {
                _bubbleView.CreateBubble(tileModel.HeadIndex);
                foreach (var (tileIndex, turn) in tileModel.MovementPath)
                {
                    _bubbleView.MoveBubble(tileIndex, turn);
                }
            }
        }
    }
}
