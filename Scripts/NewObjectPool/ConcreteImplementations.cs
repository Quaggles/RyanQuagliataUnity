using System;
using UnityEngine;

namespace RyanQuagliataUnity.NewObjectPool {
	[Serializable] public class GameObjectPool : UnityObjectPool<GameObject> { }
	[Serializable] public class RectTransformPool : ComponentObjectPool<RectTransform> { }
}