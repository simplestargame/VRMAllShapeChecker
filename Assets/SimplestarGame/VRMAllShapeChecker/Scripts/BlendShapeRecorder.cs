using System.Collections;
using System.IO;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEditor.Recorder.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace SimplestarGame
{
	/// <summary>
	/// 対象メッシュに埋まっている BlendShape を録画する
	/// </summary>
	public class BlendShapeRecorder : MonoBehaviour
	{
		/// <summary>
		/// 録画期間のタイムラインを作成して、トラック再生という録画を実行 RecResult フォルダに連番で軽量 mp4 が作られる
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <param name="duration">録画時間 秒</param>
		/// <param name="resolution">解像度 width px x height px</param>
		/// <returns>ディレクター</returns>
		PlayableDirector RecordOneTimeLine(string fileName, float duration, Vector2Int resolution)
		{
			var timeline = ScriptableObject.CreateInstance<TimelineAsset>();
			var track = timeline.CreateTrack<RecorderTrack>(null, "AAA");

			var clip = track.CreateClip<RecorderClip>();

			clip.start = 0;
			clip.duration = duration;

			var recorderSettings = ScriptableObject.CreateInstance<MovieRecorderSettings>();

			var expectedOutputFile = Application.dataPath + "/../RecResult/VRMShapeCheckResult_001.mp4";

			recorderSettings.OutputFile = Application.dataPath + $"/../RecResult/VRMShapeCheckResult_{DefaultWildcard.Date}_{fileName}";

			recorderSettings.ImageInputSettings = new GameViewInputSettings
			{
				OutputWidth = resolution.x,
				OutputHeight = resolution.y
			};

			recorderSettings.OutputFormat = MovieRecorderSettings.VideoRecorderOutputFormat.MP4;
			recorderSettings.VideoBitRateMode = VideoBitrateMode.Low;

			var recorderClip = (RecorderClip)clip.asset;
			recorderClip.settings = recorderSettings;

			var director = new GameObject("director").AddComponent<PlayableDirector>();
			director.playableAsset = timeline;

			timeline.durationMode = TimelineAsset.DurationMode.FixedLength;
			timeline.fixedDuration = duration;

			if (File.Exists(expectedOutputFile))
				File.Delete(expectedOutputFile);

			director.Play();
			return director;
		}

		IEnumerator CoBlendAnimation(PlayableDirector director, GameObject gameObject, Button button, SkinnedMeshRenderer[] skinnedMeshRenderers)
		{
			foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
			{
				var skinnedMesh = skinnedMeshRenderer.sharedMesh;
				var blendShapeCount = skinnedMesh.blendShapeCount;
				for (int blendShapeIndex = 0; blendShapeIndex < blendShapeCount; blendShapeIndex++)
				{
					if (0 < blendShapeIndex)
					{
						skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex - 1, 0);
					}
					skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, 100);
					yield return new WaitForSeconds(1.0f);
				}
				if (0 < blendShapeCount)
				{
					skinnedMeshRenderer.SetBlendShapeWeight(blendShapeCount - 1, 0);
				}
			}
			director.Stop();
			gameObject.SetActive(false);
			button.gameObject.SetActive(true);
		}

		internal void RecordAllBlendShape(Button button, GameObject gameObject, string fileName)
		{
			button.gameObject.SetActive(false);
			int totalBlendShapeCount = 0;
			var skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
			{
				var skinnedMesh = skinnedMeshRenderer.sharedMesh;
				var blendShapeCount = skinnedMesh.blendShapeCount;
				totalBlendShapeCount += blendShapeCount;
			}
			var director = this.RecordOneTimeLine(fileName, totalBlendShapeCount * 5, new Vector2Int(640, 480));

			StartCoroutine(this.CoBlendAnimation(director, gameObject, button, skinnedMeshRenderers));
		}
	}
}