//-----------------------------------------------------------------------
// <copyright file="CapacityBasedAccountSelectionStrategy.cs" company="Microsoft">Copyright 2012 Microsoft Corporation</copyright>
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.WindowsAzure.MediaServices.Client
{
    /// <summary>
    /// This IAccountSelectionStrategy implementation uses the number of used bytes in a storage account to help determine which storage account
    /// should be used to create the next asset.  It relies on the capacity metrics of the blob storage service to get the number of bytes used
    /// via the Wiindows Azure Media Services REST API.  If storage account metrics are not enabled for the storage account, then null is returned
    /// for the number of bytes used (these accounts are not used unless IncludeAccountsWithNoCapacityData is set to true).  Even knowing the 
    /// capacity of each storage account, the IAccountSelectionStrategy implementation still tries to spread out the new assets between the given 
    /// storage accounts.  This is especially important since the capacity is only calculated once per day by the storage service.  Thus, the
    /// IAccountSelectionStrategy implementation randomly selects from the available storage accounts but the probability of a storage account 
    /// being selected is based on the accounts remaining capacity.  For example, if there are two storage accounts to choose from where one account 
    /// is 50% full and the other account is 30% full then the implementation will pick the half full storage account 5 times out of 
    /// 12 ((100 – 50)/(200 – 50 – 30)) and the other storage account 7 times out of 12 ((100 – 30)/(200 – 50 – 30)).  If the algorithm always 
    /// selected the storage account with the most available space, it would be used exclusively for 24 hours until the storage account metrics 
    /// were recalculated.  This weighted probability approach should spread the assets across the given storage accounts over time.  In order 
    /// to have a margin of safety, storage accounts that are too close to the limit will be given a zero probability of being selected.  
    /// This is controlled by the MaximumStorageAccountCapacity value.  Storage accounts that are at this capacity or greater will be considered
    /// full and will not be selected for new assets.
    /// </summary>
    public class CapacityBasedAccountSelectionStrategy : BaseEntity<IAccountSelectionStrategy>, IAccountSelectionStrategy
    {
        public const long twoHundredTB = 219902325555200; // 200 * 1024^4 or 200TB
        public const long oneHundredEightyTB = 197912092999680; // 180 * 1024^4 or 180TB
        private Random _rand = new Random();
        private long _totalCapacityOfAllStorageAccounts;
        private List<CapacityBasedAccountSelectionStrategyListEntry> _weightedList;

        /// <summary>
        /// Constructor for the CapacityBasedAccountSelectionStrategy class
        /// </summary>
        /// <param name="mediaContextBase">MediaContextBase instance to query IStorageAccount entities</param>
        public CapacityBasedAccountSelectionStrategy(MediaContextBase mediaContextBase)
        {
            if (mediaContextBase == null)
            {
                throw new ArgumentNullException("mediaContextBase");
            }

            SetMediaContext(mediaContextBase);
            _weightedList = new List<CapacityBasedAccountSelectionStrategyListEntry>();
        }

        /// <summary>
        /// Creates a new instance of the CapacityBasedAccountSelectionStrategy class and then enumerates all of the storage accounts in the 
        /// given MediaContextBase.StorageAccounts collection and adds each account to the classes internal list.  The includeAccountsWithNoCapacityData
        /// flag is used if no capacity data is available.  If includeAccountsWithNoCapacityData is true and no data is available, then the storage 
        /// account is considered to have its full capacity (maximumStorageAccountCapacity) available.  If includeAccountsWithNoCapacityData 
        /// is false and no data is available, then the storage account is considered to have no capacity (0) available.  If data is available,
        /// then the IStorageAccount.BytesUsed value is subtracted from maximumStorageAccountCapacity to get the available capacity.  The 
        /// storageAccountNames list is used to filter the results of the MediaContextBase.StorageAccounts collection.  Account names that
        /// are not on the list but in the enumeration will be skipped.  Names that are on the list but not in the enumeration will not be
        /// added.  Pass null (the default) to include all of the enumerated accounts.
        /// </summary>
        /// <param name="mediaContextBase">MediaContextBase used to enumerate the IStorageAccount instances to add</param>
        /// <param name="includeAccountsWithNoCapacityData">Boolean value telling the method whether accounts should be used if no capacity data is available.</param>
        /// <param name="maximumStorageAccountCapacity">Maximum storage account capacity for the given storage accounts.</param>
        /// <param name="storageAccountNames">List of storage account names to include.</param>
        /// <returns></returns>
        public static CapacityBasedAccountSelectionStrategy FromAccounts(MediaContextBase mediaContextBase, bool includeAccountsWithNoCapacityData = false, long maximumStorageAccountCapacity = oneHundredEightyTB, string[] storageAccountNames = null)
        {
            if (null == mediaContextBase)
            {
                throw new ArgumentNullException("mediaContextBase");
            }

            CapacityBasedAccountSelectionStrategy strategy = new CapacityBasedAccountSelectionStrategy(mediaContextBase);                      

            foreach (IStorageAccount storageAccount in GetAllStorageAccounts(mediaContextBase))
            {
                if ((storageAccountNames == null) || (storageAccountNames.Contains(storageAccount.Name)))
                {
                    strategy.AddStorageAccount(storageAccount, includeAccountsWithNoCapacityData, maximumStorageAccountCapacity);
                }
            }

            return strategy;
        }

        /// <summary>
        /// Adds a CapacityBaseAccountSelectionStrategyListEntry to the classes internal list.  The considerFullCapacityIfNoDataAvailable flag is
        /// used if no capacity data is available.  If considerFullCapacityIfNoDataAvailable is true and no data is available, then the storage 
        /// account is considered to have its full capacity (maximumStorageAccountCapacity) available.  If considerFullCapacityIfNoDataAvailable 
        /// is false and no data is available, then the storage account is considered to have no capacity (0) available.  If data is available,
        /// then the IStorageAccount.BytesUsed value is subtracted from maximumStorageAccountCapacity to get the available capacity.
        /// </summary>
        /// <param name="storageAccount">IStorageAccount instance to add</param>
        /// <param name="considerAccountWithNoDataToBeEmpty">Boolean value telling the method whether accounts should be considered to be empty (and can be used for new assets) if no capacity data is available.</param>
        /// <param name="maximumStorageAccountCapacity">Maximum storage account capacity for the given storage account.</param>
        public void AddStorageAccount(IStorageAccount storageAccount, bool considerAccountWithNoDataToBeEmpty = false, long maximumStorageAccountCapacity = oneHundredEightyTB)
        {
            // make sure we don't already have this storage account in the list
            if (_weightedList.Where(item => item.StorageAccount.Name == storageAccount.Name).Count() > 0)
            {
                throw new ArgumentException("storageAccount", "The storage account already exists in the storage account list.");
            }

            CapacityBasedAccountSelectionStrategyListEntry entry = GetListEntry(storageAccount, considerAccountWithNoDataToBeEmpty, maximumStorageAccountCapacity);

            // As we build the list, add up the AvailableCapacity values so we know the end range of each entry
            entry.EndOfRange = _totalCapacityOfAllStorageAccounts + entry.AvailableCapacity;

            _weightedList.Add(entry);

            _totalCapacityOfAllStorageAccounts = entry.EndOfRange;
        }

        /// <summary>
        /// Adds a CapacityBaseAccountSelectionStrategyListEntry to the classes internal list.  The given storage account name is used to look 
        /// up the IStorageAccount entity.  The considerFullCapacityIfNoDataAvailable flag is used if no capacity data is available.  If 
        /// considerFullCapacityIfNoDataAvailable is true and no data is available, then the storage account is considered to have its full 
        /// capacity (maximumStorageAccountCapacity) available.  If considerFullCapacityIfNoDataAvailable is false and no data is available, 
        /// then the storage account is considered to have no capacity (0) available.  If data is available, then the IStorageAccount.BytesUsed 
        /// value is subtracted from maximumStorageAccountCapacity to get the available capacity.
        /// </summary>
        /// <param name="storageAccountName">Name of the storage account to add</param>
        /// <param name="considerFullCapacityIfNoDataAvailable">Boolean value telling the method whether accounts should be considered to have full capacity if no capacity data is available.</param>
        /// <param name="maximumStorageAccountCapacity">Maximum storage account capacity for the given storage account.</param>
        public void AddStorageAccountByName(string storageAccountName, bool considerFullCapacityIfNoDataAvailable = false, long maximumStorageAccountCapacity = oneHundredEightyTB)
        {
            if (String.IsNullOrWhiteSpace(storageAccountName))
            {
                throw new ArgumentException("storageAccountName", "storageAccountName must not be null or empty");
            }

            IStorageAccount storageAccount = GetStorageAccount(storageAccountName);

            if (storageAccount == null)
            {
                string message = string.Format("Unable to find a storage account with name \"{0}\"", storageAccountName);
                throw new ArgumentException(message, "storageAccountName");
            }
            else
            {
                AddStorageAccount(storageAccount, considerFullCapacityIfNoDataAvailable, maximumStorageAccountCapacity);
            }
        }

        /// <summary>
        /// Implementation of the IAccountSelectionStrategy method SelectAccountForAssets
        /// </summary>
        /// <returns>The name of the storage account to use for creating a new asset.</returns>
        public string SelectAccountForAsset()
        {
            if (_weightedList.Count == 0)
            {
                throw new InvalidOperationException("No storage accounts configured to select from.");
            }

            return RandomlySelectFromWeightedList(_weightedList);
        }

        /// <summary>
        /// Get the list of CapacityBaseAccountSelectionStrategyListEntry that contain the storage accounts to select from along
        /// with their calculated available capacity.
        /// </summary>
        /// <returns>IList of CapacityBaseAccountSelectionStrategyListEntrys</returns>
        public IList<CapacityBasedAccountSelectionStrategyListEntry> GetStorageAccounts()
        {
            return _weightedList.AsReadOnly();
        }

        /// <summary>
        /// Property to get or set the <see cref="System.Random"/> instance used by the class.
        /// </summary>
        public Random Random
        {
            get { return _rand; }
            set { _rand = value; }
        }

        private static CapacityBasedAccountSelectionStrategyListEntry GetListEntry(IStorageAccount storageAccount, bool considerAccountWithNoDataToBeEmpty, long maximumStorageAccountCapacity)
        {
            long bytesUsed = 0;
            if (storageAccount.BytesUsed.HasValue)
            {
                bytesUsed = storageAccount.BytesUsed.Value;
            }
            else
            {
                //
                //  If considerAccountWithNoDataToBeEmpty is true, then we want to consider the
                //  storage account to have a BytesUsed of zero (the account is empty).  If the
                //  considerAccountWithNoDataToBeEmpty is false, then we want to consider the
                //  storage account to have a BytesUsed of maximumStorageAccountCapacity (the
                //  account is full and it won't be used for new assets).
                //
                if (considerAccountWithNoDataToBeEmpty)
                {
                    bytesUsed = 0;
                }
                else
                {
                    bytesUsed = maximumStorageAccountCapacity;
                }
            }

            long bytesAvailable = maximumStorageAccountCapacity - bytesUsed;

            // If the storage capacity is more than the max, then we give it a weight of 0 (it won't be selected)
            bytesAvailable = Math.Max(bytesAvailable, 0);

            return new CapacityBasedAccountSelectionStrategyListEntry(storageAccount, bytesAvailable);
        }

        private long RandomLongInclusive(long min, long max)
        {
            if (min >= max)
            {
                throw new ArgumentOutOfRangeException("min", "min must be less than max");
            }

            if (min <= 0)
            {
                throw new ArgumentOutOfRangeException("min", "min must be greater than zero");
            }

            double randomDouble = _rand.NextDouble();

            double truncatedValue = Math.Truncate(randomDouble * (max - min));

            return (min + Convert.ToInt64(truncatedValue));
        }

        private string RandomlySelectFromWeightedList(List<CapacityBasedAccountSelectionStrategyListEntry> weightedItems)
        {
            string selectedStorageAccount = null;

            if (_totalCapacityOfAllStorageAccounts == 0)
            {
                throw new InvalidOperationException("Unable to find any storage accounts with available capacity!");
            }

            // next randomly select a value from 1 to the totalOfAllWeights
            long randomLong = RandomLongInclusive(1, _totalCapacityOfAllStorageAccounts);

            // now figure out which storage account the randomly selected value lives in
            for (int i = 0; i < weightedItems.Count; i++)
            {
                if (randomLong <= weightedItems[i].EndOfRange)
                {
                    selectedStorageAccount = weightedItems[i].StorageAccount.Name;
                    break;
                }
            }

            return selectedStorageAccount;
        }

        private IStorageAccount GetStorageAccount(string storageAccountName)
        {
            StorageAccountBaseCollection storageAccounts = GetMediaContext().StorageAccounts;

            return storageAccounts.Where(c => c.Name == storageAccountName).FirstOrDefault();
        }

        private static IEnumerable<IStorageAccount> GetAllStorageAccounts(MediaContextBase mediaContextBase)
        {
            StorageAccountBaseCollection storageAccounts = mediaContextBase.StorageAccounts;

            return storageAccounts.AsEnumerable<IStorageAccount>();
        }
    }

}
