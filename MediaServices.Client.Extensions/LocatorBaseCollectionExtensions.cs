// <copyright file="LocatorBaseCollectionExtensions.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
// <license>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </license>

namespace Microsoft.WindowsAzure.MediaServices.Client
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains extension methods and helpers for the <see cref="LocatorBaseCollection"/> class.
    /// </summary>
    public static class LocatorBaseCollectionExtensions
    {
        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task&lt;ILocator&gt;"/> instance for new <see cref="ILocator"/>.
        /// </summary>
        /// <param name="locators">The <see cref="LocatorBaseCollection"/> instance.</param>
        /// <param name="locatorType">The <see cref="LocatorType"/>.</param>
        /// <param name="asset">The <see cref="IAsset"/> instance for the new <see cref="ILocator"/>.</param>
        /// <param name="permissions">The <see cref="AccessPermissions"/> of the <see cref="IAccessPolicy"/> associated with the new <see cref="ILocator"/>.</param>
        /// <param name="duration">The duration of the <see cref="IAccessPolicy"/> associated with the new <see cref="ILocator"/>.</param>
        /// <param name="startTime">The start time of the new <see cref="ILocator"/>.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task&lt;ILocator&gt;"/> instance for new <see cref="ILocator"/>.</returns>
        public static async Task<ILocator> CreateAsync(this LocatorBaseCollection locators, LocatorType locatorType, IAsset asset, AccessPermissions permissions, TimeSpan duration, DateTime? startTime)
        {
            if (locators == null)
            {
                throw new ArgumentNullException("locators", "The locators collection cannot be null.");
            }

            if (asset == null)
            {
                throw new ArgumentNullException("asset", "The asset cannot be null.");
            }

            MediaContextBase context = locators.MediaContext;

            var policy = await context.AccessPolicies.CreateAsync(asset.Name, duration, permissions);

            return await locators.CreateLocatorAsync(locatorType, asset, policy, startTime);
        }

        /// <summary>
        /// Returns a <see cref="System.Threading.Tasks.Task&lt;ILocator&gt;"/> instance for new <see cref="ILocator"/>.
        /// </summary>
        /// <param name="locators">The <see cref="LocatorBaseCollection"/> instance.</param>
        /// <param name="locatorType">The <see cref="LocatorType"/>.</param>
        /// <param name="asset">The <see cref="IAsset"/> instance for the new <see cref="ILocator"/>.</param>
        /// <param name="permissions">The <see cref="AccessPermissions"/> of the <see cref="IAccessPolicy"/> associated with the new <see cref="ILocator"/>.</param>
        /// <param name="duration">The duration of the <see cref="IAccessPolicy"/> associated with the new <see cref="ILocator"/>.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task&lt;ILocator&gt;"/> instance for new <see cref="ILocator"/>.</returns>
        public static Task<ILocator> CreateAsync(this LocatorBaseCollection locators, LocatorType locatorType, IAsset asset, AccessPermissions permissions, TimeSpan duration)
        {
            return locators.CreateAsync(locatorType, asset, permissions, duration, null);
        }

        /// <summary>
        /// Returns a new <see cref="ILocator"/> instance.
        /// </summary>
        /// <param name="locators">The <see cref="LocatorBaseCollection"/> instance.</param>
        /// <param name="locatorType">The <see cref="LocatorType"/>.</param>
        /// <param name="asset">The <see cref="IAsset"/> instance for the new <see cref="ILocator"/>.</param>
        /// <param name="permissions">The <see cref="AccessPermissions"/> of the <see cref="IAccessPolicy"/> associated with the new <see cref="ILocator"/>.</param>
        /// <param name="duration">The duration of the <see cref="IAccessPolicy"/> associated with the new <see cref="ILocator"/>.</param>
        /// <param name="startTime">The start time of the new <see cref="ILocator"/>.</param>
        /// <returns>A a new <see cref="ILocator"/> instance.</returns>
        public static ILocator Create(this LocatorBaseCollection locators, LocatorType locatorType, IAsset asset, AccessPermissions permissions, TimeSpan duration, DateTime? startTime)
        {
            using (Task<ILocator> task = locators.CreateAsync(locatorType, asset, permissions, duration, startTime))
            {
                return task.Result;
            }
        }

        /// <summary>
        /// Returns a new <see cref="ILocator"/> instance.
        /// </summary>
        /// <param name="locators">The <see cref="LocatorBaseCollection"/> instance.</param>
        /// <param name="locatorType">The <see cref="LocatorType"/>.</param>
        /// <param name="asset">The <see cref="IAsset"/> instance for the new <see cref="ILocator"/>.</param>
        /// <param name="permissions">The <see cref="AccessPermissions"/> of the <see cref="IAccessPolicy"/> associated with the new <see cref="ILocator"/>.</param>
        /// <param name="duration">The duration of the <see cref="IAccessPolicy"/> associated with the new <see cref="ILocator"/>.</param>
        /// <returns>A a new <see cref="ILocator"/> instance.</returns>
        public static ILocator Create(this LocatorBaseCollection locators, LocatorType locatorType, IAsset asset, AccessPermissions permissions, TimeSpan duration)
        {
            return locators.Create(locatorType, asset, permissions, duration, null);
        }
    }
}
