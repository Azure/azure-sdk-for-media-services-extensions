//-----------------------------------------------------------------------
// <copyright file="CapacityBasedAccountSelectionStrategyFixture.cs" company="Microsoft">Copyright 2012 Microsoft Corporation</copyright>
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.MediaServices.Client;
using MediaServices.Client.Extensions.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaServices.Client.Extensions.Tests
{
    [TestClass]
    public class CapacityBasedAccountSelectionStrategyFixture
    {
        private const long oneGB = 1024 * 1024 * 1024;
        private const long oneHundredMB = 100 * 1024 * 1024;
        private const long twoHundredMB = 200 * 1024 * 1024;
        private const long eightHundredMB = 800 * 1024 * 1024;
        private const long nineHundredMB = 900 * 1024 * 1024;

        private string[] _fourStorageAccountNameArray = new string[] { "account1", "account2", "account3", "account4" };
        private string[] _fiveStorageAccountNameArray = new string[] { "account1", "account2", "account3", "account4", "account5" };

        private long?[] _evenBytesUsedValues = new long?[] { oneGB, oneGB, oneGB, oneGB };
        private long?[] _skewedBytesUsedValues = new long?[] { nineHundredMB, oneHundredMB, twoHundredMB, eightHundredMB };
        private long?[] _oneZeroBytesUsedValues = new long?[] { oneGB, 0, oneGB, oneGB };
        private long?[] _oneNullBytesUsedValues = new long?[] { 0, null, 0, 0 };

        private double[] _valuesForRandomNumberGeneratorToReturnEven = new double[] { 0.2, 0.4, 0.6, 0.8 };
        private double[] _valuesForRandomNumberGeneratorToReturnTopAndBottom = new double[] { 0.0, 0.05, 0.99, 1.0 };

        private long CalculateExpectedAvailableCapacity(long? originalBytesUsed, long maximumStorageAccountCapacity, bool considerFullCapacityIfNoDataAvailable)
        {
            long bytesUsed = 0;

            if (originalBytesUsed.HasValue)
            {
                bytesUsed = originalBytesUsed.Value;
            }
            else
            {
                if (considerFullCapacityIfNoDataAvailable)
                {
                    bytesUsed = 0;
                }
                else
                {
                    bytesUsed = maximumStorageAccountCapacity;
                }
            }

            long bytesAvailable = maximumStorageAccountCapacity - bytesUsed;

            return Math.Max(bytesAvailable, 0);
        }

        private List<IStorageAccount> GetStorageAccountList(string[] accountNames, long?[] bytesUsedValues)
        {
            List<IStorageAccount> storageAccountList = new List<IStorageAccount>();

            for (int i = 0; i < accountNames.Length; i++)
            {            
                IStorageAccount account = GetStorageAccount(accountNames[i], bytesUsedValues[i]);
                storageAccountList.Add(account);
            }

            return storageAccountList;
        }

        private IStorageAccount GetStorageAccount(string name, long? bytesUsed)
        {
            IStorageAccount returnValue = new StorageAccountMock();
            returnValue.Name = name;
            returnValue.BytesUsed = bytesUsed;

            return returnValue;
        }

        private MediaContextBase GetMediaContextBase(List<IStorageAccount> storageAccounts)
        {
            StorageAccountBaseCollectionMock collection = null;
            if (storageAccounts != null)
            {
                collection = new StorageAccountBaseCollectionMock(storageAccounts.AsQueryable<IStorageAccount>());
            }

            return new MediaContextBaseMock(collection);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CapacityBasedShouldThrowIfNoAccounts()
        {
            MediaContextBase context = GetMediaContextBase(null);
            CapacityBasedAccountSelectionStrategy strategy = new CapacityBasedAccountSelectionStrategy(context);

            try
            {
                strategy.SelectAccountForAsset();
            }
            catch (InvalidOperationException e)
            {
                Assert.AreEqual("No storage accounts configured to select from.", e.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CapacityBasedShouldThrowIfAccountNamesCannotBeFound()
        {
            List<IStorageAccount> storageAccountList = GetStorageAccountList(_fourStorageAccountNameArray, _evenBytesUsedValues);
            
            MediaContextBase context = GetMediaContextBase(storageAccountList);

            CapacityBasedAccountSelectionStrategy strategy = new CapacityBasedAccountSelectionStrategy(context);

            try
            {
                foreach (string accountName in _fiveStorageAccountNameArray)
                {
                    strategy.AddStorageAccountByName(accountName);
                }
            }
            catch (ArgumentException ae)
            {
                Assert.IsTrue(ae.Message.Contains("Unable to find a storage account with name \"account5\""));
                Assert.AreEqual("storageAccountName", ae.ParamName);
                throw;
            }
        }

        [TestMethod]
        public void CapacityBasedShouldOmitAccountsWithNoDataByDefault()
        {
            List<IStorageAccount> storageAccountList = GetStorageAccountList(_fourStorageAccountNameArray, _oneZeroBytesUsedValues);

            MediaContextBase context = GetMediaContextBase(storageAccountList);
            CapacityBasedAccountSelectionStrategy strategy = CapacityBasedAccountSelectionStrategy.FromAccounts(context, maximumStorageAccountCapacity:oneGB);

            string accountNameToUse = strategy.SelectAccountForAsset();

            Assert.IsNotNull(accountNameToUse);
            Assert.AreEqual(0, _oneZeroBytesUsedValues[1].Value);
            Assert.AreEqual(_fourStorageAccountNameArray[1], accountNameToUse);

            VerifyStrategyEntriesMatchExpectations(storageAccountList, strategy, oneGB, false);
        }

        [TestMethod]
        public void CapacityBasedShouldIncludeAccountsWithNoDataWhenEnabled()
        {
            List<IStorageAccount> storageAccountList = GetStorageAccountList(_fourStorageAccountNameArray, _oneNullBytesUsedValues);

            MediaContextBase context = GetMediaContextBase(storageAccountList);

            CapacityBasedAccountSelectionStrategy strategy = CapacityBasedAccountSelectionStrategy.FromAccounts(context, includeAccountsWithNoCapacityData:true, maximumStorageAccountCapacity: oneGB);

            strategy.Random = new RandomNumberGeneratorMock(_valuesForRandomNumberGeneratorToReturnEven);

            for (int i = 0; i < storageAccountList.Count; i++)
            {
                string accountNameToUse = strategy.SelectAccountForAsset();
                Assert.AreEqual(storageAccountList[i].Name, accountNameToUse);
            }

            VerifyStrategyEntriesMatchExpectations(storageAccountList, strategy, oneGB, true);
        }

        [TestMethod]
        public void CapacityBasedShouldPickAccountsBasedOnWeightedDistribution()
        {
            // Try even weighting
            List<IStorageAccount> storageAccountList = GetStorageAccountList(_fourStorageAccountNameArray, _evenBytesUsedValues);

            MediaContextBase context = GetMediaContextBase(storageAccountList);
            CapacityBasedAccountSelectionStrategy strategy = CapacityBasedAccountSelectionStrategy.FromAccounts(context);

            strategy.Random = new RandomNumberGeneratorMock(_valuesForRandomNumberGeneratorToReturnEven);

            for (int i = 0; i < storageAccountList.Count; i++)
            {
                string accountNameToUse = strategy.SelectAccountForAsset();
                Assert.AreEqual(storageAccountList[i].Name, accountNameToUse);
            }

            VerifyStrategyEntriesMatchExpectations(storageAccountList, strategy, CapacityBasedAccountSelectionStrategy.oneHundredEightyTB, false);

            // Try skewed weighting
            // Note that the first account and the last account in the list are almost full.  With the "random" numbers we picked
            // we will always pick the two middle accounts.
            storageAccountList = GetStorageAccountList(_fourStorageAccountNameArray, _skewedBytesUsedValues);
            context = GetMediaContextBase(storageAccountList);
            strategy = CapacityBasedAccountSelectionStrategy.FromAccounts(context, maximumStorageAccountCapacity:oneGB);

            strategy.Random = new RandomNumberGeneratorMock(_valuesForRandomNumberGeneratorToReturnEven);

            string[] expectedAccountNames = new string[] { _fiveStorageAccountNameArray[1], _fiveStorageAccountNameArray[1], _fiveStorageAccountNameArray[2], _fiveStorageAccountNameArray[2] };
            for (int i = 0; i < storageAccountList.Count; i++)
            {
                string accountNameToUse = strategy.SelectAccountForAsset();
                Assert.AreEqual(expectedAccountNames[i], accountNameToUse);
            }

            VerifyStrategyEntriesMatchExpectations(storageAccountList, strategy, oneGB, false);

            // Try skewed weighting again but change the "random" numbers we generate to be very small and very large so that
            // we pick the first and last account even though they are almost full.
            storageAccountList = GetStorageAccountList(_fourStorageAccountNameArray, _skewedBytesUsedValues);
            context = GetMediaContextBase(storageAccountList);
            strategy = CapacityBasedAccountSelectionStrategy.FromAccounts(context, maximumStorageAccountCapacity: oneGB);

            strategy.Random = new RandomNumberGeneratorMock(_valuesForRandomNumberGeneratorToReturnTopAndBottom);

            expectedAccountNames = new string[] { _fiveStorageAccountNameArray[0], _fiveStorageAccountNameArray[0], _fiveStorageAccountNameArray[3], _fiveStorageAccountNameArray[3] };
            for (int i = 0; i < storageAccountList.Count; i++)
            {
                string accountNameToUse = strategy.SelectAccountForAsset();
                Assert.AreEqual(expectedAccountNames[i], accountNameToUse);
            }

            VerifyStrategyEntriesMatchExpectations(storageAccountList, strategy, oneGB, false);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CapacityBasedShouldThrowWhenNoAccountCanBeSelected()
        {
            List<IStorageAccount> storageAccountList = GetStorageAccountList(_fourStorageAccountNameArray, _evenBytesUsedValues);

            MediaContextBase context = GetMediaContextBase(storageAccountList);
            CapacityBasedAccountSelectionStrategy strategy = CapacityBasedAccountSelectionStrategy.FromAccounts(context, maximumStorageAccountCapacity:oneGB);

            VerifyStrategyEntriesMatchExpectations(storageAccountList, strategy, oneGB, false);

            try
            {
                string accountNameToUse = strategy.SelectAccountForAsset();
            }
            catch (InvalidOperationException e)
            {
                Assert.AreEqual("Unable to find any storage accounts with available capacity!", e.Message);
                throw;
            }
        }

        [TestMethod]
        public void CapacityBasedShouldFromAccountsShouldFilterBasedOnInputArray()
        {
            List<IStorageAccount> storageAccountList = GetStorageAccountList(_fourStorageAccountNameArray, _evenBytesUsedValues);

            // Copy 3 of the 4 names to the filter array
            string[] filterArray = new string[_fourStorageAccountNameArray.Length - 1];
            Array.Copy(_fourStorageAccountNameArray, filterArray, filterArray.Length);

            // save the name of the skipped entry
            string nameSkipped = _fourStorageAccountNameArray[_fourStorageAccountNameArray.Length - 1];

            // Create the CapacityBasedAccountSelectionStrategy
            MediaContextBase context = GetMediaContextBase(storageAccountList);
            CapacityBasedAccountSelectionStrategy strategy = CapacityBasedAccountSelectionStrategy.FromAccounts(context, maximumStorageAccountCapacity: oneGB, storageAccountNames:filterArray);

            // Now ensure that the internal list only has the expected number of entries.
            IList<CapacityBasedAccountSelectionStrategyListEntry> accountListFromStrategy = strategy.GetStorageAccounts();
            Assert.AreEqual(filterArray.Length, accountListFromStrategy.Count);

            foreach (CapacityBasedAccountSelectionStrategyListEntry entry in accountListFromStrategy)
            {
                Assert.AreNotEqual(nameSkipped, entry.StorageAccount.Name);
            }

            // Add the name previously skipped
            strategy.AddStorageAccountByName(nameSkipped, false, oneGB);

            VerifyStrategyEntriesMatchExpectations(storageAccountList, strategy, oneGB, false);

            // Now verify that if I have names in the filter array that don't exist in the account no exception occurs
            strategy = CapacityBasedAccountSelectionStrategy.FromAccounts(context, maximumStorageAccountCapacity: oneGB, storageAccountNames: _fiveStorageAccountNameArray);
            VerifyStrategyEntriesMatchExpectations(storageAccountList, strategy, oneGB, false);
        }

        private void VerifyStrategyEntriesMatchExpectations(List<IStorageAccount> expectedList, CapacityBasedAccountSelectionStrategy strategy, long maximumStorageAccountCapacity, bool considerFullCapacityIfNoDataAvailable)
        {
            IList<CapacityBasedAccountSelectionStrategyListEntry> accountListFromStrategy = strategy.GetStorageAccounts();
            Assert.AreEqual(expectedList.Count, accountListFromStrategy.Count);

            foreach (CapacityBasedAccountSelectionStrategyListEntry entry in accountListFromStrategy)
            {
                IStorageAccount accountFromExpectedList = expectedList.Where(st => st.Name == entry.StorageAccount.Name).SingleOrDefault();

                long expectedAvailableCapacity = CalculateExpectedAvailableCapacity(accountFromExpectedList.BytesUsed, maximumStorageAccountCapacity, considerFullCapacityIfNoDataAvailable);

                Assert.AreEqual(accountFromExpectedList.Name, entry.StorageAccount.Name);
                Assert.AreEqual(accountFromExpectedList.BytesUsed, entry.StorageAccount.BytesUsed);
                Assert.AreEqual(expectedAvailableCapacity, entry.AvailableCapacity);
            }
        }
    }
}
