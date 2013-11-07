//-----------------------------------------------------------------------
// <copyright file="FakeRandomNumberGenerator.cs" company="Microsoft">Copyright 2012 Microsoft Corporation</copyright>
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

namespace MediaServices.Client.Extensions.Tests
{
    class RandomNumberGeneratorMock : Random
    {
        private int _currentIndex;
        private double[] _valuesToReturn;

        public RandomNumberGeneratorMock(double[] valuesToReturn)
        {
            if (valuesToReturn == null)
            {
                throw new ArgumentNullException();
            }

            _valuesToReturn = valuesToReturn;
            _currentIndex = 0;
        }

        protected override double Sample()
        {
            return _valuesToReturn[_currentIndex++];
        }

        public override int Next()
        {
            return (int)(Sample() * int.MaxValue);
        }

        public override int Next(int minvalue, int maxValue)
        {
            throw new NotImplementedException();
        }

        public override int Next(int maxValue)
        {
            throw new NotImplementedException();
        }
    }
}
