using System;
using UnityEngine;
using UnityEngine.Events;

namespace RyanQuagliata.UnityEvents {
	[Serializable] public class BooleanEvent : UnityEvent<Boolean> { }

	[Serializable] public class ByteEvent : UnityEvent<Byte> { }

	[Serializable] public class SByteEvent : UnityEvent<SByte> { }

	[Serializable] public class UInt16Event : UnityEvent<UInt16> { }

	[Serializable] public class Int16Event : UnityEvent<Int16> { }

	[Serializable] public class UInt32Event : UnityEvent<UInt32> { }

	[Serializable] public class Int32Event : UnityEvent<Int32> { }

	[Serializable] public class UInt64Event : UnityEvent<UInt64> { }

	[Serializable] public class Int64Event : UnityEvent<Int64> { }

	[Serializable] public class SingleEvent : UnityEvent<Single> { }

	[Serializable] public class DoubleEvent : UnityEvent<Double> { }

	[Serializable] public class ColorEvent : UnityEvent<Color> { }

	[Serializable] public class Color32Event : UnityEvent<Color32> { }

	[Serializable] public class Vector2Event : UnityEvent<Vector2> { }

	[Serializable] public class Vector3Event : UnityEvent<Vector3> { }

	[Serializable] public class Vector4Event : UnityEvent<Vector4> { }

	[Serializable] public class QuaternionEvent : UnityEvent<Quaternion> { }

	[Serializable] public class CharEvent : UnityEvent<Char> { }

	[Serializable] public class StringEvent : UnityEvent<String> { }

	[Serializable] public class TransformEvent : UnityEvent<Transform> { }

	[Serializable] public class RectTransformEvent : UnityEvent<RectTransform> { }

	[Serializable] public class GameObjectEvent : UnityEvent<GameObject> { }
	
	[Serializable] public class SpriteEvent : UnityEvent<Sprite> { }
	
	[Serializable] public class MaterialEvent : UnityEvent<Material> { }
}