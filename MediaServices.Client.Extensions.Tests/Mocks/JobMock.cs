// <copyright file="JobMock.cs" company="Microsoft">Copyright 2013 Microsoft Corporation</copyright>
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

using System.Diagnostics.CodeAnalysis;

namespace MediaServices.Client.Extensions.Tests.Mocks
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.MediaServices.Client;

    public class JobMock : IJob
    {
        #pragma warning disable 0067
        public event EventHandler<JobStateChangedEventArgs> StateChanged;
        #pragma warning restore 0067

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastModified { get; set; }

        public int Priority { get; set; }

        public TimeSpan RunningDuration { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public JobState State { get; set; }

        public string TemplateId { get; set; }

        public ReadOnlyCollection<IAsset> InputMediaAssets { get; set; }

        public ReadOnlyCollection<IAsset> OutputMediaAssets { get; set; }

        public JobNotificationSubscriptionCollection JobNotificationSubscriptions { get; set; }

        public TaskCollection Tasks { get; set; }

        public IJobTemplate SaveAsTemplate(string templateName)
        {
            throw new NotImplementedException();
        }

        public Task<IJobTemplate> SaveAsTemplateAsync(string templateName)
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public Task CancelAsync()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync()
        {
            throw new NotImplementedException();
        }

        public Task GetExecutionProgressTask(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Submit()
        {
            throw new NotImplementedException();
        }

        public Task<IJob> SubmitAsync()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public Task<IJob> UpdateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
