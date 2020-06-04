using UnityEngine;
using UnityEngine.UI;

namespace SimplestarGame
{
    /// <summary>
    /// 読み込んだ VRM のすべての BlendShape を順に有効化して、それを終始録画して動画出力する
    /// </summary>
    public class BlendShapeChekcer : MonoBehaviour
    {
        [SerializeField, Tooltip("VRMを読み込む処理")] VRMLoader vrmLoader = null;
        [SerializeField, Tooltip("カメラ映像を動画記録する処理")] BlendShapeRecorder timelineRecorder = null;

        void Start()
        {
            this.vrmLoader.onLoadVRM += this.timelineRecorder.RecordAllBlendShape;
        }
    }
}