using System;
using RyanQuagliataUnity.Configurable;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions.DOTween {
	[CreateAssetMenu(fileName = nameof(TweenerSettingsV2), menuName = "Ryan Quagliata/Tweener Settings V2", order = 1000)]
	public class TweenerSettingsV2Asset : ConfigurableAsset<TweenerSettingsV2> { }
	
	public interface ITweenerConfig : IConfig<TweenerSettingsV2> { }
	
	[Serializable]
	public class TweenerSettingsV2AssetContainer : ConfigurableAssetContainer<TweenerSettingsV2Asset, TweenerSettingsV2>, ITweenerConfig { }
}