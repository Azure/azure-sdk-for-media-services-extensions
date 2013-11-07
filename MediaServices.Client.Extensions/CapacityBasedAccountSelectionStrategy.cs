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
        private const long twoHundredTB = 219902325555200; // 200 * 1024^4 or 200TB
        private const long oneHundredEightyTB = 197912092999680; // 180 * 1024^4 or 180TB
        private Random _rand = new Random();
        private long _maximumStorageAccountCapacity;

        public Random Random
        {
            get { return _rand; }
            set { _rand = value; }
        }

        public long MaximumStorageAccountCapacity
        {
            get { return _maximumStorageAccountCapacity; }
            set { SetMaximumStorageAccountCapacity(value); }
        }

        public bool IncludeAccountsWithNoCapacityData { get; set; }

        public CapacityBasedAccountSelectionStrategy(MediaContextBase mediaContextBase)
            : this(mediaContextBase, oneHundredEightyTB, false)
        {
        }

        public CapacityBasedAccountSelectionStrategy(MediaContextBase mediaContextBase, bool includeAccountsWithNoCapacityData)
            : this(mediaContextBase, oneHundredEightyTB, includeAccountsWithNoCapacityData)
        {
        }

        public CapacityBasedAccountSelectionStrategy(MediaContextBase mediaContextBase, long maximumStorageAccountCapacity, bool includeAccountsWithNoCapacityData)
        {
            if (mediaContextBase == null)
            {
                throw new ArgumentNullException("mediaContextBase");
            }

            SetMediaContext(mediaContextBase);
            SetMaximumStorageAccountCapacity(maximumStorageAccountCapacity);
            IncludeAccountsWithNoCapacityData = includeAccountsWithNoCapacityData;
        }

        private void SetMaximumStorageAccountCapacity(long maximumStorageAccountCapacity)
        {
            if ((maximumStorageAccountCapacity > twoHundredTB) || (maximumStorageAccountCapacity <= 0))
            {
                throw new ArgumentOutOfRangeException("maximumStorageAccountCapacity", "The maximum storage account capacity must be between 1 and 219902325555200");
            }

            _maximumStorageAccountCapacity = maximumStorageAccountCapacity;
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

        private string RandomlySelectFromWeightedList(List<Tuple<string, long>> weightedItems)
        {
            string selectedStorageAccount = null;

            // first add up all of the weights
            long totalOfAllWeights = 0;
            for (int i = 0; i < weightedItems.Count; i++)
            {
                totalOfAllWeights += weightedItems[i].Item2;
            }

            if (totalOfAllWeights == 0)
            {
                throw new ArgumentException("Unable to find any storage accounts with available capacity!");
            }

            // next randomly select a value from 1 to the totalOfAllWeights
            long randomLong = RandomLongInclusive(1, totalOfAllWeights);

            // now figure out which storage account the randomly selected value lives in
            long runningTotal = 0;
            for (int i = 0; i < weightedItems.Count; i++)
            {
                long currentWeight = weightedItems[i].Item2;
                if (randomLong <= (runningTotal + currentWeight))
                {
                    selectedStorageAccount = weightedItems[i].Item1;
                    break;
                }
                else
                {
                    runningTotal += currentWeight;
                }
            }

            return selectedStorageAccount;
        }

        protected IStorageAccount GetStorageAccount(string storageAccountName)
        {
            StorageAccountBaseCollection storageAccounts = GetMediaContext().StorageAccounts;

            return storageAccounts.Where(c => c.Name == storageAccountName).FirstOrDefault();
        }

        private List<Tuple<string, long>> GetWeightedList(string[] accountNames)
        {
            List<Tuple<string, long>> returnValue = new List<Tuple<string, long>>();

            for (int i = 0; i < accountNames.Length; i++)
            {
                IStorageAccount storageAccount = GetStorageAccount(accountNames[i]);
                if (storageAccount == null)
                {
                    string message = string.Format(CultureInfo.CurrentCulture, "Unable to find a storage account with name \"{0}\"", accountNames[i]);
                    throw new ArgumentException(message);
                }

                long bytesUsed = 0;
                if (storageAccount.BytesUsed.HasValue)
                {
                    bytesUsed = storageAccount.BytesUsed.Value;
                }
                else
                {
                    if (this.IncludeAccountsWithNoCapacityData)
                    {
                        bytesUsed = 0;
                    }
                    else
                    {
                        bytesUsed = MaximumStorageAccountCapacity;
                    }
                }

                long weight = MaximumStorageAccountCapacity - bytesUsed;

                // If the storage capacity is more than the max, then we give it a weight of 0 (it won't be selected)
                weight = Math.Max(weight, 0);

                Tuple<string, long> current = new Tuple<string, long>(accountNames[i], weight);
                returnValue.Add(current);
            }

            return returnValue;
        }

        public string SelectAccountForAssets(string[] storageAccountNames)
        {
            if (storageAccountNames == null)
            {
                throw new ArgumentNullException("storageAccountNames");
            }

            List<Tuple<string, long>> weightedList = GetWeightedList(storageAccountNames);

            return RandomlySelectFromWeightedList(weightedList);
        }
    }

}
