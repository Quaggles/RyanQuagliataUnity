// using System;
// using UniRx.Async;
// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using UnityEngine.ResourceManagement.AsyncOperations;
// using UnityEngine.ResourceManagement.ResourceProviders;
// using UnityEngine.SceneManagement;
//
// namespace RyanQuagliataUnity.Extensions.Unity.Addressables {
// 	public static class AssetReferenceExtensions {
// 		public static async UniTask<T> LoadAssetUniTaskAsync<T>(this AssetReferenceT<T> that,
// 			IProgress<float> progress = null) =>
// 			await that.LoadAssetAsync<T>().HandleAsyncOperationAsync(progress);
//
// 		public static async UniTask<GameObject> InstantiateUniTaskAsync(this AssetReference that,
// 			IProgress<float> progress = null) =>
// 			await that.InstantiateAsync().HandleAsyncOperationAsync(progress);
//
// 		public static async UniTask<GameObject>
// 			InstantiateUniTaskAsync<T>(this T that, IProgress<float> progress = null)
// 			where T : AssetReferenceT<GameObject> =>
// 			await that.InstantiateAsync().HandleAsyncOperationAsync(progress);
//
// 		public static async UniTask<SceneInstance> LoadSceneUniTaskAsync(this AssetReference that, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100, IProgress<float> progress = null) =>
// 			await UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(that.RuntimeKey, loadMode, activateOnLoad, priority) .HandleAsyncOperationAsync(progress);
//
// 		public static async UniTask<T> HandleAsyncOperationAsync<T>(this AsyncOperationHandle<T> that,
// 			IProgress<float> progress = null) {
// 			var tcs = new UniTaskCompletionSource<T>();
//
// 			void Completed(AsyncOperationHandle<T> handle) {
// 				switch (handle.Status) {
// 					case AsyncOperationStatus.None:
// 						tcs.TrySetException(handle.OperationException);
// 						break;
// 					case AsyncOperationStatus.Succeeded:
// 						tcs.TrySetResult(handle.Result);
// 						break;
// 					case AsyncOperationStatus.Failed:
// 						tcs.TrySetException(handle.OperationException);
// 						break;
// 					default:
// 						throw new ArgumentOutOfRangeException();
// 				}
// 			}
//
// 			that.Completed += Completed;
// 			try {
// 				if (progress != null) {
// 					while (!tcs.Task.IsCompleted) {
// 						progress.Report(that.PercentComplete);
// 						await UniTask.DelayFrame(1);
// 					}
// 				}
//
// 				return await tcs.Task;
// 			} finally {
// 				that.Completed -= Completed;
// 			}
// 		}
// 	}
// }
