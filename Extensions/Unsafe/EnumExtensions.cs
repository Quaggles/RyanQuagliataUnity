using System;

namespace RyanQuagliataUnity.Extensions.Unsafe {
	public static class EnumExtensions {
		/// <summary>
		/// Garbage allocation free version of Enum.HasFlag using unsafe
        /// https://forum.unity.com/threads/c-hasaflag-method-extension-how-to-not-create-garbage-allocation.616924/#post-4420699
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static bool HasFlagUnsafe<T>(this T lhs, T rhs) where T :
#if CSHARP_7_3_OR_NEWER
			unmanaged, Enum
#else
            struct
#endif
		{
			unsafe {
#if CSHARP_7_3_OR_NEWER
				switch (sizeof(T)) {
					case 1:
						return (*(byte*) (&lhs) & *(byte*) (&rhs)) > 0;
					case 2:
						return (*(ushort*) (&lhs) & *(ushort*) (&rhs)) > 0;
					case 4:
						return (*(uint*) (&lhs) & *(uint*) (&rhs)) > 0;
					case 8:
						return (*(ulong*) (&lhs) & *(ulong*) (&rhs)) > 0;
					default:
						throw new Exception("Size does not match a known Enum backing type.");
				}

#else
                switch (UnsafeUtility.SizeOf<TEnum>())
                {
                    case 1:
                        {
                            byte valLhs = 0;
                            UnsafeUtility.CopyStructureToPtr(ref lhs, &valLhs);
                            byte valRhs = 0;
                            UnsafeUtility.CopyStructureToPtr(ref rhs, &valRhs);
                            return (valLhs & valRhs) > 0;
                        }
                    case 2:
                        {
                            ushort valLhs = 0;
                            UnsafeUtility.CopyStructureToPtr(ref lhs, &valLhs);
                            ushort valRhs = 0;
                            UnsafeUtility.CopyStructureToPtr(ref rhs, &valRhs);
                            return (valLhs & valRhs) > 0;
                        }
                    case 4:
                        {
                            uint valLhs = 0;
                            UnsafeUtility.CopyStructureToPtr(ref lhs, &valLhs);
                            uint valRhs = 0;
                            UnsafeUtility.CopyStructureToPtr(ref rhs, &valRhs);
                            return (valLhs & valRhs) > 0;
                        }
                    case 8:
                        {
                            ulong valLhs = 0;
                            UnsafeUtility.CopyStructureToPtr(ref lhs, &valLhs);
                            ulong valRhs = 0;
                            UnsafeUtility.CopyStructureToPtr(ref rhs, &valRhs);
                            return (valLhs & valRhs) > 0;
                        }
                    default:
                        throw new Exception("Size does not match a known Enum backing type.");
                }
#endif
			}
		}
	}
}