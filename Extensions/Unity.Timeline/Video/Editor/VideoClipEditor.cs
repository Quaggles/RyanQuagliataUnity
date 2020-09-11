using UnityEditor.Timeline;
using UnityEngine.Timeline;

namespace RyanQuagliataUnity.Extensions.Unity.Timeline.Video.Editor {
	[CustomTimelineEditor(typeof(VideoScriptPlayableAsset))]
	public class VideoClipEditor : ClipEditor {
		public override void OnClipChanged(TimelineClip clip) {
			if (clip.asset is VideoScriptPlayableAsset clipAsset) {
				if (clipAsset.videoClip) {
					clip.displayName = clipAsset.videoClip.name;
				} else {
					clip.displayName = "Empty";
				}
			}
		}
	}
}