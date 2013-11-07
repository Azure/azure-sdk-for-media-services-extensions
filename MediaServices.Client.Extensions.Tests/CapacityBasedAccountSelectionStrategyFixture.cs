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

        private CapacityBasedAccountSelectionStrategy GetCapacityBasedAccountSelectionStrategy(List<IStorageAccount> storageAccounts)
        {
            StorageAccountBaseCollectionMock collection = null;
            if (storageAccounts != null)
            {
                collection = new StorageAccountBaseCollectionMock(storageAccounts.AsQueryable<IStorageAccount>());
            }

            MediaContextBaseMock mediaContextBaseMock = new MediaContextBaseMock(collection);

            CapacityBasedAccountSelectionStrategy strategy = new CapacityBasedAccountSelectionStrategy(mediaContextBaseMock);

            return strategy;            
        }

        [TestMethod]
        public void CapacityBasedShouldThrowIfAccountNamesAreNull()
        {
            string[] storageAccountNames = null;
            List<IStorageAccount> storageAccountList = null;

            CapacityBasedAccountSelectionStrategy strategy = GetCapacityBasedAccountSelectionStrategy(storageAccountList);

            try
            {
                strategy.SelectAccountForAssets(storageAccountNames);
                Assert.Fail("Should have throw an ArgumentNullException exception.");
            }
            catch (ArgumentNullException ae)
            {
                Assert.AreEqual("storageAccountNames", ae.ParamName);
            }
        }

        [TestMethod]
        public void CapacityBasedShouldThrowIfAccountNamesCannotBeFound()
        {
            List<IStorageAccount> storageAccountList = GetStorageAccountList(_fourStorageAccountNameArray, _evenBytesUsedValues);
            CapacityBasedAccountSelectionStrategy strategy = GetCapacityBasedAccountSelectionStrategy(storageAccountList);

            try
            {
                strategy.SelectAccountForAssets(_fiveStorageAccountNameArray);
                Assert.Fail("Should have throw an ArgumentNullException exception.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Unable to find a storage account with name \"account5\"", ae.Message);
            }
        }

        [TestMethod]
        public void CapacityBasedShouldOmitAccountsWithNoDataByDefault()
        {
            List<IStorageAccount> storageAccountList = GetStorageAccountList(_fourStorageAccountNameArray, _oneZeroBytesUsedValues);
            CapacityBasedAccountSelectionStrategy strategy = GetCapacityBasedAccountSelectionStrategy(storageAccountList);

            strategy.MaximumStorageAccountCapacity = oneGB;

            string accountNameToUse = strategy.SelectAccountForAssets(_fourStorageAccountNameArray);

            Assert.IsNotNull(accountNameToUse);
            Assert.AreEqual(0, _oneZeroBytesUsedValues[1].Value);
            Assert.AreEqual(_fourStorageAccountNameArray[1], accountNameToUse);
        }

        [TestMethod]
        public void CapacityBasedShouldIncludeAccountsWithNoDataWhenEnabled()
        {
            List<IStorageAccount> storageAccountList = GetStorageAccountList(_fourStorageAccountNameArray, _oneNullBytesUsedValues);
            CapacityBasedAccountSelectionStrategy strategy = GetCapacityBasedAccountSelectionStrategy(storageAccountList);

            strategy.MaximumStorageAccountCapacity = oneGB;
            strategy.IncludeAccountsWithNoCapacityData = true;
            strategy.Random = new RandomNumberGeneratorMock(_valuesForRandomNumberGeneratorToReturnEven);

            for (int i = 0; i < storageAccountList.Count; i++)
            {
                string accountNameToUse = strategy.SelectAccountForAssets(_fourStorageAccountNameArray);
                Assert.AreEqual(storageAccountList[i].Name, accountNameToUse);
            }
        }

        [TestMethod]
        public void CapacityBasedShouldPickAccountsBasedOnWeightedDistribution()
        {
            // Try even weighting
            List<IStorageAccount> storageAccountList = GetStorageAccountList(_fourStorageAccountNameArray, _evenBytesUsedValues);
            CapacityBasedAccountSelectionStrategy strategy = GetCapacityBasedAccountSelectionStrategy(storageAccountList);

            strategy.Random = new RandomNumberGeneratorMock(_valuesForRandomNumberGeneratorToReturnEven);

            for (int i = 0; i < storageAccountList.Count; i++)
            {
                string accountNameToUse = strategy.SelectAccountForAssets(_fourStorageAccountNameArray);
                Assert.AreEqual(storageAccountList[i].Name, accountNameToUse);
            }

            // Try skewed weighting
            // Note that the first account and the last account in the list are almost full.  With the "random" numbers we picked
            // we will always pick the two middle accounts.
            storageAccountList = GetStorageAccountList(_fourStorageAccountNameArray, _skewedBytesUsedValues);
            strategy = GetCapacityBasedAccountSelectionStrategy(storageAccountList);

            strategy.MaximumStorageAccountCapacity = oneGB;
            strategy.Random = new RandomNumberGeneratorMock(_valuesForRandomNumberGeneratorToReturnEven);

            string[] expectedAccountNames = new string[] { _fiveStorageAccountNameArray[1], _fiveStorageAccountNameArray[1], _fiveStorageAccountNameArray[2], _fiveStorageAccountNameArray[2] };
            for (int i = 0; i < storageAccountList.Count; i++)
            {
                string accountNameToUse = strategy.SelectAccountForAssets(_fourStorageAccountNameArray);
                Assert.AreEqual(expectedAccountNames[i], accountNameToUse);
            }

            // Try skewed weighting again but change the "random" numbers we generate to be very small and very large so that
            // we pick the first and last account even though they are almost full.
            storageAccountList = GetStorageAccountList(_fourStorageAccountNameArray, _skewedBytesUsedValues);
            strategy = GetCapacityBasedAccountSelectionStrategy(storageAccountList);

            strategy.MaximumStorageAccountCapacity = oneGB;
            strategy.Random = new RandomNumberGeneratorMock(_valuesForRandomNumberGeneratorToReturnTopAndBottom);

            expectedAccountNames = new string[] { _fiveStorageAccountNameArray[0], _fiveStorageAccountNameArray[0], _fiveStorageAccountNameArray[3], _fiveStorageAccountNameArray[3] };
            for (int i = 0; i < storageAccountList.Count; i++)
            {
                string accountNameToUse = strategy.SelectAccountForAssets(_fourStorageAccountNameArray);
                Assert.AreEqual(expectedAccountNames[i], accountNameToUse);
            }
        }

        [TestMethod]
        public void CapacityBasedShouldThrowWhenNoAccountCanBeSelected()
        {
            List<IStorageAccount> storageAccountList = GetStorageAccountList(_fourStorageAccountNameArray, _evenBytesUsedValues);
            CapacityBasedAccountSelectionStrategy strategy = GetCapacityBasedAccountSelectionStrategy(storageAccountList);

            strategy.MaximumStorageAccountCapacity = oneGB;

            try
            {
                string accountNameToUse = strategy.SelectAccountForAssets(_fourStorageAccountNameArray);
                Assert.Fail("Should have throw an ArgumentException exception.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Unable to find any storage accounts with available capacity!", ae.Message);
            }
        }
    }
}
