// <copyright file="TaskCollectionMock.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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

namespace MediaServices.Client.Extensions.Tests.Mocks
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.WindowsAzure.MediaServices.Client;

    public class TaskCollectionMock : TaskCollection
    {
        private readonly IList<ITask> tasks;

        public TaskCollectionMock()
        {
            var tasksField = typeof(TaskCollection).GetField("_tasks", BindingFlags.NonPublic | BindingFlags.Instance);

            this.tasks = (IList<ITask>)tasksField.GetValue(this);
        }

        public void Add(ITask task)
        {
            this.tasks.Add(task);
        }
    }
}
