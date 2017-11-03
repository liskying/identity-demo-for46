// Copyright (c) Microsoft Corporation, Inc. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNet.Identity
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    /// <summary>
    /// 
    /// </summary>
    //internal static class TaskExtensions
    public static class TaskExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static CultureAwaiter<T> WithCurrentCulture<T>(this Task<T> task)
        {
            return new CultureAwaiter<T>(task);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static CultureAwaiter WithCurrentCulture(this Task task)
        {
            return new CultureAwaiter(task);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public struct CultureAwaiter<T> : ICriticalNotifyCompletion
        {
            private readonly Task<T> _task;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="task"></param>
            public CultureAwaiter(Task<T> task)
            {
                _task = task;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public CultureAwaiter<T> GetAwaiter()
            {
                return this;
            }
            /// <summary>
            /// 
            /// </summary>
            public bool IsCompleted
            {
                get { return _task.IsCompleted; }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public T GetResult()
            {
                return _task.GetAwaiter().GetResult();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="continuation"></param>
            public void OnCompleted(Action continuation)
            {
                // The compiler will never call this method
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="continuation"></param>
            public void UnsafeOnCompleted(Action continuation)
            {
                var currentCulture = Thread.CurrentThread.CurrentCulture;
                var currentUiCulture = Thread.CurrentThread.CurrentUICulture;
                _task.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(() =>
                {
                    var originalCulture = Thread.CurrentThread.CurrentCulture;
                    var originalUiCulture = Thread.CurrentThread.CurrentUICulture;
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    Thread.CurrentThread.CurrentUICulture = currentUiCulture;
                    try
                    {
                        continuation();
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = originalCulture;
                        Thread.CurrentThread.CurrentUICulture = originalUiCulture;
                    }
                });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public struct CultureAwaiter : ICriticalNotifyCompletion
        {
            private readonly Task _task;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="task"></param>
            public CultureAwaiter(Task task)
            {
                _task = task;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public CultureAwaiter GetAwaiter()
            {
                return this;
            }
            /// <summary>
            /// 
            /// </summary>
            public bool IsCompleted
            {
                get { return _task.IsCompleted; }
            }
            /// <summary>
            /// 
            /// </summary>
            public void GetResult()
            {
                _task.GetAwaiter().GetResult();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="continuation"></param>
            public void OnCompleted(Action continuation)
            {
                // The compiler will never call this method
                throw new NotImplementedException();
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="continuation"></param>
            public void UnsafeOnCompleted(Action continuation)
            {
                var currentCulture = Thread.CurrentThread.CurrentCulture;
                var currentUiCulture = Thread.CurrentThread.CurrentUICulture;
                _task.ConfigureAwait(false).GetAwaiter().UnsafeOnCompleted(() =>
                {
                    var originalCulture = Thread.CurrentThread.CurrentCulture;
                    var originalUiCulture = Thread.CurrentThread.CurrentUICulture;
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    Thread.CurrentThread.CurrentUICulture = currentUiCulture;
                    try
                    {
                        continuation();
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = originalCulture;
                        Thread.CurrentThread.CurrentUICulture = originalUiCulture;
                    }
                });
            }
        }
    }
}