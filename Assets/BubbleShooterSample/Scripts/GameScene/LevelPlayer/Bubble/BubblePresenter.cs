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
                BubbleTilePathNode headPathNodel = tileModel.HeadPathNode;
                BubbleTile bubbleTile = _bubbleView.CreateBubble(headPathNodel.tilePosition);
                foreach (BubbleTilePathNode node in tileModel.MovementPathNodeList)
                {
                    bubbleTile.Move(node.tilePosition, node.turn);
                }
            }
        }
    }
}
