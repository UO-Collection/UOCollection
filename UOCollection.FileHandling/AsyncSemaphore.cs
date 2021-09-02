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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UOCollection.FileHandling
{
    public class AsyncSemaphore
    {
        private static readonly Task _completed = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> _waiters = new Queue<TaskCompletionSource<bool>>();
        private int _currentCount;

        public AsyncSemaphore(int initialCount)
        {
            if (initialCount < 0)
            {
                throw new ArgumentOutOfRangeException("initialCount");
            }

            _currentCount = initialCount;
        }

        public Task WaitAsync()
        {
            lock (_waiters)
            {
                if (_currentCount > 0)
                {
                    --_currentCount;
                    return _completed;
                }

                var waiter = new TaskCompletionSource<bool>();

                _waiters.Enqueue(waiter);

                return waiter.Task;
            }
        }

        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;

            lock (_waiters)
            {
                if (_waiters.Count > 0)
                {
                    toRelease = _waiters.Dequeue();
                }
                else
                {
                    ++_currentCount;
                }
            }

            toRelease?.SetResult(true);
        }
    }
}
