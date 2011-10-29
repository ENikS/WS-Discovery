using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        #region Task To Asynchronous Programming Model pattern

        /// <summary>
        /// Implements the APM pattern with Tasks.
        /// Usage:
        /// 
        ///     static Task FooAsync();
        /// 
        /// Is wrapped into an APM implementation:
        /// 
        /// static IAsyncResult BeginFoo(AsyncCallback callback, object state)
        /// {
        ///     return FooAsync().ToApm(callback, state);
        /// }
        /// 
        /// static int EndFoo(IAsyncResult asyncResult)
        /// {
        ///     if (!result.IsCompleted)
        ///         result.AsyncWaitHandle.WaitOne();
        /// }
        /// </summary>
        /// <param name="task"><see cref="Task"/></param>
        /// <param name="callback">Callback delegate.</param>
        /// <param name="state">AsyncState object</param>
        /// <returns></returns>
        public static Task ToApm(this Task task, AsyncCallback callback, object state)
        {
            if (task.AsyncState == state)
            {
                if (callback != null)
                {
                    task.ContinueWith(delegate { callback(task); },
                        CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
                }
                return task;
            }

            var tcs = new TaskCompletionSource<object>(state);
            task.ContinueWith(delegate
            {
                if (task.IsFaulted) tcs.TrySetException(task.Exception.InnerExceptions);
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else tcs.TrySetResult(null);

                if (callback != null) callback(tcs.Task);

            }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            return tcs.Task;
        }

        /// <summary>
        /// Implements the APM pattern with Tasks<TResult>.
        /// Usage:
        /// 
        ///     static Task<int> FooAsync();
        /// 
        /// Is wrapped into an APM implementation:
        /// 
        /// static IAsyncResult BeginFoo(AsyncCallback callback, object state)
        /// {
        ///     return FooAsync().ToApm(callback, state);
        /// }
        /// 
        /// static int EndFoo(IAsyncResult asyncResult)
        /// {
        ///     return ((Task<int>)asyncResult).Result; 
        /// }
        /// </summary>
        /// <typeparam name="TResult">Type of return value</typeparam>
        /// <param name="task"><see cref="Tasks<TResult>"/></param>
        /// <param name="callback">Callback delegate.</param>
        /// <param name="state">AsyncState object</param>
        /// <returns></returns>
        public static Task<TResult> ToApm<TResult>(this Task<TResult> task, AsyncCallback callback, object state)
        {
            if (task.AsyncState == state)
            {
                if (callback != null)
                {
                    task.ContinueWith(delegate { callback(task); },
                        CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
                }
                return task;
            }

            var tcs = new TaskCompletionSource<TResult>(state);
            task.ContinueWith(delegate
            {
                if (task.IsFaulted) tcs.TrySetException(task.Exception.InnerExceptions);
                else if (task.IsCanceled) tcs.TrySetCanceled();
                else tcs.TrySetResult(task.Result);

                if (callback != null) callback(tcs.Task);

            }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            return tcs.Task;
        }
        
        #endregion

    }

}
