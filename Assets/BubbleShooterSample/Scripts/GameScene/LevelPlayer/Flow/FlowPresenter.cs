using BubbleShooterSample.System;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooterSample.LevelPlayer
{
    public class FlowPresenter : MonoBehaviour,
        ILevelRestorable
    {
        [SerializeField] private FlowModel _flowModel;

        public event Action<IReadOnlyDictionary<Color, LinkedList<IFlowTileModel>>> onColorTileListDictUpdated
        {
            add { _flowModel.onColorTileListDictUpdated += value; }
            remove { _flowModel.onColorTileListDictUpdated -= value; }
        }

        public LevelDataId RestoreLevelID => LevelDataId.Flow;

        private void Awake()
        {
            _flowModel.Init();
        }

        public void RestoreLevelData(string dataStr)
        {
            _flowModel.RestoreLevelData(dataStr);


        }
    }
}
