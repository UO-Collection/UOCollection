//===================================================================
//===================================================================
// Copyright (C) 2021  3HMonkey @UO-Collection
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// NOTICE: Some script parts come from other collections. If a creator
// is known, it is listed here: Wyatt of RUOSI, andreakarasho 
//===================================================================
//===================================================================

using System;
using System.Threading;
using System.Threading.Tasks;

namespace UOCollection.FileHandling
{
    public class AsyncLock
    {
        private readonly Task<Releaser> _releaser;
        private readonly AsyncSemaphore _semaphore;

        public AsyncLock()
        {
            _semaphore = new AsyncSemaphore(1);
            _releaser = Task.FromResult(new Releaser(this));
        }

        public Task<Releaser> LockAsync()
        {
            var wait = _semaphore.WaitAsync();

            return wait.IsCompleted ?
                _releaser :
                wait.ContinueWith(
                    (_, state) => new Releaser((AsyncLock)state),
                    this,
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
        }

        public struct Releaser : IDisposable
        {
            private readonly AsyncLock _lock;

            internal Releaser(AsyncLock @lock)
            {
                _lock = @lock;
            }

            public void Dispose()
            {
                _lock?._semaphore.Release();
            }
        }
    }
}
