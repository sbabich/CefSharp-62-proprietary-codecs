﻿// Copyright © 2010-2017 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CefSharp
{
    public static class AsyncExtensions
    {
        /// <summary>
        /// Deletes all cookies that matches all the provided parameters asynchronously.
        /// If both <paramref name="url"/> and <paramref name="name"/> are empty, all cookies will be deleted.
        /// </summary>
        /// <param name="url">The cookie URL. If an empty string is provided, any URL will be matched.</param>
        /// <param name="name">The name of the cookie. If an empty string is provided, any URL will be matched.</param>
        /// <return>Returns -1 if a non-empty invalid URL is specified, or if cookies cannot be accessed;
        /// otherwise, a task that represents the delete operation. The value of the TResult will be the number of cookies that were deleted or -1 if unknown.</return>
        public static Task<int> DeleteCookiesAsync(this ICookieManager cookieManager, string url = null, string name = null)
        {
            if(cookieManager == null)
            {
                throw new NullReferenceException("cookieManager");
            }

            if(cookieManager.IsDisposed)
            {
                throw new ObjectDisposedException("cookieManager");
            }

            var callback = new TaskDeleteCookiesCallback();
            if(cookieManager.DeleteCookies(url, name, callback))
            {
                return callback.Task;
            }

            //There was a problem deleting cookies

            Task<int> task = new Task<int>(() =>
            {
                return TaskDeleteCookiesCallback.InvalidNoOfCookiesDeleted;
            });
            task.Start();

            return task;// Task.<TaskDeleteCookiesCallback>();
        }

        /// <summary>
        /// Sets a cookie given a valid URL and explicit user-provided cookie attributes.
        /// This function expects each attribute to be well-formed. It will check for disallowed
        /// characters (e.g. the ';' character is disallowed within the cookie value attribute) and will return false without setting
        /// </summary>
        /// <param name="cookieManager">cookie manager</param>
        /// <param name="url">The cookie URL. If an empty string is provided, any URL will be matched.</param>
        /// <param name="cookie">the cookie to be set</param>
        /// <return>returns false if the cookie cannot be set (e.g. if illegal charecters such as ';' are used);
        /// otherwise task that represents the set operation. The value of the TResult parameter contains a bool to indicate success.</return>
        public static Task<bool> SetCookieAsync(this ICookieManager cookieManager, string url, Cookie cookie)
        {
            if (cookieManager == null)
            {
                throw new NullReferenceException("cookieManager");
            }

            if (cookieManager.IsDisposed)
            {
                throw new ObjectDisposedException("cookieManager");
            }

            var callback = new TaskSetCookieCallback();
            if (cookieManager.SetCookie(url, cookie, callback))
            {
                return callback.Task;	
            }

            Task<bool> task = new Task<bool>(() =>
            {
                return false;
            });
            task.Start();

            //There was a problem setting cookies
            return task;
        }

        /// <summary>
        /// Visits all cookies. The returned cookies are sorted by longest path, then by earliest creation date.
        /// </summary>
        /// <return>A task that represents the VisitAllCookies operation. The value of the TResult parameter contains a List of cookies
        /// or null if cookies cannot be accessed.</return>
        public static Task<List<Cookie>> VisitAllCookiesAsync(this ICookieManager cookieManager)
        {
            var cookieVisitor = new TaskCookieVisitor();

            if(cookieManager.VisitAllCookies(cookieVisitor))
            {
                return cookieVisitor.Task;
            }

            Task<List<Cookie>> task = new Task<List<Cookie>>(() =>
            {
                return new List<Cookie>(null);
            });
            task.Start();

            return task;
        }

        /// <summary>
        /// Visits a subset of the cookies. The results are filtered by the given url scheme, host, domain and path. 
        /// If <paramref name="includeHttpOnly"/> is true, HTTP-only cookies will also be included in the results. The returned cookies 
        /// are sorted by longest path, then by earliest creation date.
        /// </summary>
        /// <param name="url">The URL to use for filtering a subset of the cookies available.</param>
        /// <param name="includeHttpOnly">A flag that determines whether HTTP-only cookies will be shown in results.</param>
        /// <return>A task that represents the VisitUrlCookies operation. The value of the TResult parameter contains a List of cookies.
        /// or null if cookies cannot be accessed.</return>
        public static Task<List<Cookie>> VisitUrlCookiesAsync(this ICookieManager cookieManager, string url, bool includeHttpOnly)
        {
            var cookieVisitor = new TaskCookieVisitor();

            if(cookieManager.VisitUrlCookies(url, includeHttpOnly, cookieVisitor))
            {
                return cookieVisitor.Task;
            }

            Task<List<Cookie>> task = new Task<List<Cookie>>(() =>
            {
                return null;
            });
            task.Start();

            return task;
        }

        /// <summary>
        /// Flush the backing store (if any) to disk.
        /// </summary>
        /// <param name="cookieManager">cookieManager instance</param>
        /// <returns>A task that represents the FlushStore operation. Result indicates if the flush completed successfully.
        /// Will return false if the cookikes cannot be accessed.</returns>
        public static Task<bool> FlushStoreAsync(this ICookieManager cookieManager)
        {
            var handler = new TaskCompletionCallback();

            if (cookieManager.FlushStore(handler))
            {
                return handler.Task;
            }

            Task<bool> task = new Task<bool>(() =>
            {
                return false;
            });
            task.Start();

            //returns null if cookies cannot be accessed.
            return task;
        }
    }
}
