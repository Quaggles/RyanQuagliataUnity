using System;
using RyanQuagliata.NewObjectPool;
using UnityEngine;

namespace RyanQuagliata.Scripts.NewObjectPool {
	[Serializable] public class GameObjectPool : UnityObjectPool<GameObject> { }
	[Serializable] public class RectTransformPool : ComponentObjectPool<RectTransform> { }
}