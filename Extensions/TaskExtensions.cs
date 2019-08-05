using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace RyanQuagliataUnity.Extensions {
	public static class TaskExtensions {
		/// <summary>
		/// Adds a continuation to Debug.LogError all exceptions to the Unity Console for a task
		/// </summary>
		/// <param name="that"></param>
		/// <param name="context"></param>
		/// <returns>Same task as the parameter for method chaining</returns>
		public static Task DoLogExceptions([NotNull] this Task that, UnityEngine.Object context = null) {
			if (that == null) throw new ArgumentNullException(nameof(that));
			that.ContinueWith(t => {
				if (!(t.Exception is AggregateException)) return;
				Debug.LogError($"{t.Exception.InnerExceptions.Count} Error/s occured in Task ID# {that.Id}", context);

				// Log each exception to console
				for (var i = 0; i < t.Exception.InnerExceptions.Count; i++) {
					var e = t.Exception.InnerExceptions[i];
					Debug.LogError($"\t{i + 1}: {e.ToString()}", context);
				}
			}, TaskContinuationOptions.OnlyOnFaulted);
			return that;
		}

		/// <summary>
		/// Adds a continuation to Debug.LogError all exceptions to the Unity Console for a task
		/// </summary>
		/// <param name="that"></param>
		/// <param name="context"></param>
		/// <returns>Same task as the parameter for method chaining</returns>
		public static Task<T> DoLogExceptions<T>([NotNull] this Task<T> that, UnityEngine.Object context = null) {
			if (that == null) throw new ArgumentNullException(nameof(that));
			that.ContinueWith(t => {
				if (!(t.Exception is AggregateException)) return;
				Debug.LogError($"{t.Exception.InnerExceptions.Count} Error/s occured in Task ID# {that.Id}", context);

				// Log each exception to console
				for (var i = 0; i < t.Exception.InnerExceptions.Count; i++) {
					var e = t.Exception.InnerExceptions[i];
					Debug.LogError($"\t{i + 1}: {e.ToString()}", context);
				}
			}, TaskContinuationOptions.OnlyOnFaulted);
			return that;
		}

		/// <summary>
		/// Adds a continuation to run an action on all exceptions generated within this task
		/// </summary>
		/// <param name="that"></param>
		/// <param name="action">Action to perform on every exception</param>
		/// <returns>Same task as the parameter for method chaining</returns>
		public static Task ForEachException([NotNull] this Task that, Action<Exception> action) {
			if (that == null) throw new ArgumentNullException(nameof(that));
			that.ContinueWith(t => {
				if (!(t.Exception is AggregateException)) return;

				// Invoke action for every exception
				t.Exception.InnerExceptions.ForEach(action.Invoke);
			}, TaskContinuationOptions.OnlyOnFaulted);
			return that;
		}

		/// <summary>
		/// Returns the result of the first task that completed successfully, if none complete successfully an exception is thrown
		/// </summary>
		/// <param name="that"></param>
		/// <typeparam name="TResult"></typeparam>
		/// <returns></returns>
		public static Task<TResult> WhenAnySuccessful<TResult>([NotNull] this IEnumerable<Task<TResult>> that) {
			if (that == null) throw new ArgumentNullException(nameof(that));
			var tcs = new TaskCompletionSource<TResult>();
			int remainingTasks = that.Count();
			if (remainingTasks == 0) tcs.SetException(new ArgumentOutOfRangeException("Task enumerable is empty"));
			foreach (var task in that) {
				task.ContinueWith(t => {
					if (task.Status == TaskStatus.RanToCompletion)
						tcs.TrySetResult(t.Result);
					else if (Interlocked.Decrement(ref remainingTasks) == 0)
						tcs.SetException(new AggregateException(
							that.SelectMany(t2 => t2.Exception?.InnerExceptions ?? Enumerable.Empty<Exception>())));
				});
			}

			return tcs.Task;
		}

		/// <summary>
		/// Returns the result of the first task that completed successfully, if none complete successfully the default value is returned
		/// </summary>
		/// <param name="that"></param>
		/// <param name="defaultValue"></param>
		/// <typeparam name="TResult"></typeparam>
		/// <returns></returns>
		public static Task<TResult> WhenAnySuccessful<TResult>([NotNull] this IEnumerable<Task<TResult>> that, TResult defaultValue) {
			if (that == null) throw new ArgumentNullException(nameof(that));
			var tcs = new TaskCompletionSource<TResult>();
			int remainingTasks = that.Count();
			foreach (var task in that) {
				task.ContinueWith(t => {
					if (task.Status == TaskStatus.RanToCompletion)
						tcs.TrySetResult(t.Result);
					else if (Interlocked.Decrement(ref remainingTasks) == 0) tcs.TrySetResult(defaultValue);
				});
			}

			return tcs.Task;
		}
		
		/// <summary>
		/// Returns the first task that completes and satisfies the predicate
		/// </summary>
		/// <param name="that"></param>
		/// <param name="predicate"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentException"></exception>
		public static Task<Task<T>> WhenFirst<T>(this IEnumerable<Task<T>> that, Func<Task<T>, bool> predicate) {
			if (that == null) throw new ArgumentNullException(nameof(that));
			if (predicate == null) throw new ArgumentNullException(nameof(predicate));

			var tasksArray = that as IReadOnlyList<Task<T>> ?? that.ToArray();
			if (tasksArray.Count == 0) throw new ArgumentException("Empty task list", nameof(that));
			if (tasksArray.Any(t => t == null))
				throw new ArgumentException("Tasks contains a null reference", nameof(that));

			var tcs = new TaskCompletionSource<Task<T>>();
			var count = tasksArray.Count;
			
			// If the task OnRanToCompletion check if the result satisfies the predicate, if so set it as the result
			void Continuation(Task<T> t) {
				if (predicate(t)) tcs.TrySetResult(t);
				if (Interlocked.Decrement(ref count) == 0) tcs.TrySetResult(null);
			}
			
			// If the task did not run to completion decrement the counter
			void Error(Task<T> t) {
				if (Interlocked.Decrement(ref count) == 0) tcs.TrySetResult(null);
				// t.ForEachException(Debug.LogError);
			}

			foreach (var task in tasksArray) {
				task.ContinueWith(Continuation, TaskContinuationOptions.OnlyOnRanToCompletion);
				task.ContinueWith(Error, TaskContinuationOptions.NotOnRanToCompletion);
			}

			return tcs.Task;
		}

		public static async Task<IEnumerable<TResult>> WhenAllSwallowExceptions<TResult>(
			this IEnumerable<Task<TResult>> that) {
			var tasklist = that.ToList();
			var results = new List<TResult>();
			while (tasklist.Any()) {
				var completedTask = await Task.WhenAny(tasklist);
				try {
					results.Add(await completedTask);
				} catch {
					// Swallow
				}

				tasklist.Remove(completedTask);
			}

			return results;
		}
	}
}