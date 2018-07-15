//-----------------------------------------------------------------------
// <copyright file="NamedLocker.cs">
//     Copyright (c) 2018 Adam Craven. All rights reserved.
// </copyright>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-----------------------------------------------------------------------

namespace ChannelAdam.Xml.Internal
{
    using System;
    using System.Collections.Concurrent;

    internal class NamedLocker : ChannelAdam.Disposing.Abstractions.Disposable
    {
        #region Private Fields

        private readonly ConcurrentDictionary<string, object> _lockDict = new ConcurrentDictionary<string, object>();

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// For use with a lock(){} block.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetLock(string name)
        {
            return _lockDict.GetOrAdd(name, new object());
        }

        /// <summary>
        /// Removes a lock object that is no longer needed.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public void RemoveLock(string name)
        {
            _lockDict.TryRemove(name, out object o);
        }

        /// <summary>
        /// Performs the given function body inside a lock block.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="name"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public TResult RunWithLock<TResult>(string name, Func<TResult> body)
        {
            lock (_lockDict.GetOrAdd(name, new object()))
            {
                return body();
            }
        }

        /// <summary>
        /// Performs the given action body inside a lock block.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="name"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public void RunWithLock(string name, Action body)
        {
            lock (_lockDict.GetOrAdd(name, new object()))
            {
                body();
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void DisposeManagedResources()
        {
            _lockDict.Clear();

            base.DisposeManagedResources();
        }

        #endregion Protected Methods
    }
}