Windows Azure Media Services .NET SDK Extensions
================================================

## What is it?
A NuGet package that contains a set of extension methods and helpers for the Windows Azure Media Services SDK for .NET.

## Usage
Install the [WindowsAzure.MediaServices.Extensions](https://www.nuget.org/packages/WindowsAzure.MediaServices.Extensions) Nuget package by running `Install-Package WindowsAzure.MediaServices.Extensions` in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console/).

After installing the package, a **MediaServicesExtensions** folder will be added to your project's root directory containing the following files:
- AssetBaseCollectionExtensions.cs: Contains extension methods and helpers for the [AssetBaseCollection](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.assetbasecollection.aspx) class.
- IAssetExtensions.cs: Contains extension methods and helpers for the [IAsset](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.iasset.aspx) interface.
- JobBaseCollectionExtensions.cs: Contains extension methods and helpers for the [JobBaseCollection](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.jobbasecollection.aspx) class.
- IJobExtensions.cs: Contains extension methods and helpers for the [IJob](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.ijob.aspx) interface.
- LocatorBaseCollectionExtensions.cs: Contains extension methods and helpers for the [LocatorBaseCollection](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.locatorbasecollection.aspx) class.
- ILocatorExtensions.cs: Contains extension methods for to the [ILocator](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.ilocator.aspx) interface.
- MediaProcessorBaseCollectionExtensions.cs: Contains extension methods and helpers for the [MediaProcessorBaseCollection](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.mediaprocessorbasecollection.aspx) class.
- MediaEncoderTaskPresetStrings.cs: Contains constants with the names of the available [Task Preset Strings for the Windows Azure Media Encoder](http://msdn.microsoft.com/en-us/library/windowsazure/jj129582.aspx).
- MediaProcessorNames.cs: Contains constants with the names of the available [Media Processors](http://msdn.microsoft.com/en-us/library/windowsazure/jj129580.aspx).
- MediaServicesExceptionParser.cs: Contains helper methods to parse Windows Azure Media Services error messages in XML format.

## Extension Methods and Helpers available

### Create an Asset from a single local file
Create a new asset by uploading a local file using a single extension method for the [AssetBaseCollection](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.assetbasecollection.aspx) class. There are additional overloads with different parameters and _async_ support.
```csharp
CloudMediaContext context = new CloudMediaContext("%accountName%", "%accountKey%");

// The local path to the file to upload to the new asset.
string filePath = @"C:\AssetFile.wmv";

// The options for creating the new asset.
AssetCreationOptions assetCreationOptions = AssetCreationOptions.None;

// Create a new asset and upload a local file using a single extension method.
IAsset asset = context.Assets.CreateFromFile(filePath, assetCreationOptions);
```

### Create an Asset from a local folder
Create a new asset by uploading all the files in a local folder using a single extension method for the [AssetBaseCollection](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.assetbasecollection.aspx) class. There are additional overloads with different parameters and _async_ support.
```csharp
CloudMediaContext context = new CloudMediaContext("%accountName%", "%accountKey%");

// The path to an existing local folder with the files to upload to the new asset.
string folderPath = @"C:\AssetFilesFolder";

// The options for creating the new asset.
AssetCreationOptions assetCreationOptions = AssetCreationOptions.None;

// Create a new asset and upload all the files in a local folder using a single extension method.
IAsset asset = context.Assets.CreateFromFolder(folderPath, assetCreationOptions);
```

### Generate Asset Files from Blob storage
Generate the asset files of an existing asset using a single extension method for the [IAsset](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.iasset.aspx) interface. You can use this method after uploading content directly to the asset container in Azure Blob storage. This method leverages the [CreateFileInfos REST API Function](http://msdn.microsoft.com/library/windowsazure/jj683097.aspx#createfileinfos). There is an additional overload with _async_ support.
```csharp
CloudMediaContext context = new CloudMediaContext("%accountName%", "%accountKey%");

// Create an empty asset.
IAsset asset = context.Assets.Create("MyAssetName", AssetCreationOptions.None);

// Upload content to the previous asset directly to its Blob storage container.
// You can use a SAS locator with Write permissions to do this.
// ...

// Generate all the asset files in the asset from its Blob storage container using a single extension method.
asset.GenerateFromStorage();
```

### Download Asset Files to a local folder
Download all the asset files in an asset using a single extension method for the [IAsset](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.iasset.aspx) interface. There are additional overloads with different parameters and _async_ support.
```csharp
CloudMediaContext context = new CloudMediaContext("%accountName%", "%accountKey%");

// The asset with the asset files to download. Get a reference to it from the context.
IAsset asset = null;

// The path to an existing local folder where to download all the asset files in the asset.
string folderPath = @"C:\AssetFilesFolder";

// Download all the asset files to a local folder using a single extension method.
asset.DownloadToFolder(folderPath);
```

### Get manifest Asset File
Get a reference to the asset file that represents the ISM manifest using a single extension method for the [IAsset](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.iasset.aspx) interface.
```csharp
// The asset with multi-bitrate content. Get a reference to it from the context.
IAsset asset = null;

// Get the asset file representing the ISM manifest.
IAssetFile manifestAssetFile = asset.GetManifestAssetFile();
```

### Create a Locator
Create a locator and its associated access policy using a single extension method for the [LocatorBaseCollection](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.locatorbasecollection.aspx) class. There are additional overloads with different parameters and _async_ support.
```csharp
CloudMediaContext context = new CloudMediaContext("%accountName%", "%accountKey%");

// The asset used to create the locator. Get a reference to it from the context.
IAsset asset = null;

// The locator type.
LocatorType locatorType = LocatorType.OnDemandOrigin;

// The permissions for the locator's access policy.
AccessPermissions accessPolicyPermissions = AccessPermissions.Read | AccessPermissions.List;

// The duration for the locator's access policy.
TimeSpan accessPolicyDuration = TimeSpan.FromDays(30);

// Create a locator and its associated access policy using a single extension method.
ILocator locator = context.Locators.Create(locatorType, asset, accessPolicyPermissions, accessPolicyDuration);
```

### Get Smooth Streaming URL from an Asset
Get the Smooth Streaming URL of a multi-bitrate Smooth Streaming or MP4 asset using a single extension method for the [IAsset](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.iasset.aspx) interface. This methods requires the asset to contain an ISM manifest asset file and that you previously created an Origin locator for the asset; otherwise it returns _null_.
```csharp
// The asset with multi-bitrate Smooth Streaming or MP4 content. Get a reference to it from the context.
IAsset asset = null;

// Make sure to create an Origin locator for the asset.

// Get the Smooth Streaming URL of the asset for adaptive streaming.
Uri smoothStreamingUri = asset.GetSmoothStreamingUri();
```

### Get Smooth Streaming URL from a Locator
Get the Smooth Streaming URL of a multi-bitrate Smooth Streaming or MP4 asset using a single extension method for the [ILocator](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.ilocator.aspx) interface. This methods requires the locator to be of type [LocatorType.OnDemandOrigin](http://msdn.microsoft.com/en-US/library/microsoft.windowsazure.mediaservices.client.locatortype.aspx), and its asset to contain an ISM manifest asset file.
```csharp
// The Origin locator for the multi-bitrate Smooth Streaming or MP4 asset. Get a reference to it from the context.
ILocator originLocator = null;


// Get the Smooth Streaming URL of the locator's asset for adaptive streaming.
Uri smoothStreamingUri = originLocator.GetSmoothStreamingUri();
```

### Get HLS URL from an Asset
Get the HLS URL of a multi-bitrate Smooth Streaming or MP4 asset using a single extension method for the [IAsset](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.iasset.aspx) interface. This methods requires the asset to contain an ISM manifest asset file and that you previously created an Origin locator for the asset; otherwise it returns _null_.
```csharp
// The asset with multi-bitrate Smooth Streaming or MP4 content. Get a reference to it from the context.
IAsset asset = null;

// Make sure to create an Origin locator for the asset.

// Get the HLS URL of the asset for adaptive streaming.
Uri hlsUri = asset.GetHlsUri();
```

### Get HLS URL from a Locator
Get the HLS URL of a multi-bitrate Smooth Streaming or MP4 asset using a single extension method for the [ILocator](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.ilocator.aspx) interface. This methods requires the locator to be of type [LocatorType.OnDemandOrigin](http://msdn.microsoft.com/en-US/library/microsoft.windowsazure.mediaservices.client.locatortype.aspx), and its asset to contain an ISM manifest asset file.
```csharp
// The Origin locator for the multi-bitrate Smooth Streaming or MP4 asset. Get a reference to it from the context.
ILocator originLocator = null;

// Make sure to create an Origin locator for the asset.

// Get the HLS URL of the locator's asset for adaptive streaming.
Uri hlsUri = originLocator.GetHlsUri();
```

### Get MPEG-DASH URL from an Asset
Get the MPEG-DASH URL of a multi-bitrate Smooth Streaming or MP4 asset using a single extension method for the [IAsset](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.iasset.aspx) interface. This methods requires the asset to contain an ISM manifest asset file and that you previously created an Origin locator for the asset; otherwise it returns _null_.
```csharp
// The asset with multi-bitrate Smooth Streaming or MP4 content. Get a reference to it from the context.
IAsset asset = null;

// Make sure to create an Origin locator for the asset.

// Get the MPEG-DASH URL of the locator's asset for adaptive streaming.
Uri mpegDashUri = asset.GetMpegDashUri();
```

### Get MPEG-DASH URL from a Locator
Get the MPEG-DASH URL of a multi-bitrate Smooth Streaming or MP4 asset using a single extension method for the [ILocator](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.ilocator.aspx) interface. This methods requires the locator to be of type [LocatorType.OnDemandOrigin](http://msdn.microsoft.com/en-US/library/microsoft.windowsazure.mediaservices.client.locatortype.aspx), and its asset to contain an ISM manifest asset file.
```csharp
// The Origin locator for the multi-bitrate Smooth Streaming or MP4 asset. Get a reference to it from the context.
ILocator originLocator = null;

// Make sure to create an Origin locator for the asset.

// Get the MPEG-DASH URL of the asset for adaptive streaming.
Uri mpegDashUri = originLocator.GetMpegDashUri();
```

### Get SAS URL from an Asset File
Get the SAS URL of an asset file for progressive download using a single extension method for the [IAssetFile](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.iassetfile.aspx) interface. This methods requires the parent asset to contain a SAS locator for the asset; otherwise it returns _null_. There is an additional overload that receives an [ILocator](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.ilocator.aspx) as a parameter.
```csharp
// The asset with multi-bitrate MP4 content. Get a reference to it from the context.
IAsset asset = null;

// Make sure to create a SAS locator for the asset.
IAssetFile assetFile = asset.AssetFiles.ToList().Where(af => af.Name.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)).First();

// Get the SAS URL of the asset file for progressive download.
Uri sasUri = assetFile.GetSasUri();
```

### Get latest Media Processor by name
Get the latest version of a media processor filtering by its name using a single extension method for the [MediaProcessorBaseCollection](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.mediaprocessorbasecollection.aspx) class.
```csharp
CloudMediaContext context = new CloudMediaContext("%accountName%", "%accountKey%");

// The media processor name.
string mediaProcessorName = MediaProcessorNames.WindowsAzureMediaEncoder;

// Get the latest version of a media processor by its name using a single extension method.
IMediaProcessor processor = context.MediaProcessors.GetLatestMediaProcessorByName(mediaProcessorName);
```

### Create a Job with a single Task
Create a job with a single task ready to be submitted using a single extension method for the [JobBaseCollection](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.jobbasecollection.aspx) class. There is an additional overload with different parameters. 
```csharp
CloudMediaContext context = new CloudMediaContext("%accountName%", "%accountKey%");

// The media processor name used in the job's task.
string mediaProcessorName = MediaProcessorNames.WindowsAzureMediaEncoder;

// The task configuration.
string taskConfiguration = MediaEncoderTaskPresetStrings.H264AdaptiveBitrateMP4Set720p;

// The input asset for the task. Get a reference to it from the context.
IAsset inputAsset = null;

// The name for the output asset of the task.
string outputAssetName = "OutputAssetName";

// The options for creating the output asset of the task.
AssetCreationOptions outputAssetOptions = AssetCreationOptions.None;

// Create a job ready to be submitted with a single task with one input/output asset using a single extension method.
IJob job = context.Jobs.CreateWithSingleTask(mediaProcessorName, taskConfiguration, inputAsset, outputAssetName, outputAssetOptions);

// Submit the job and wait until it is completed to get the output asset.
// ...
```

### Start Job execution progress task to notify when its state or overall progress change
Start a [Task](http://msdn.microsoft.com/library/system.threading.tasks.task.aspx) to monitor a job progress using a single extension method for the [IJob](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.ijob.aspx) interface. The difference with the [IJob.GetExecutionProgressTask](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.ijob.getexecutionprogresstask.aspx) method is that this extension invokes a callback when the job state or overall progress change. There is an additional overload with different parameters.
```csharp
CloudMediaContext context = new CloudMediaContext("%accountName%", "%accountKey%");

// The input asset for the task. Get a reference to it from the context.
IAsset inputAsset = null;

// Prepare a job ready to be submitted with a single task with one input/output asset using a single extension method.
IJob job = context.Jobs.CreateWithSingleTask(MediaProcessorNames.WindowsAzureMediaEncoder, MediaEncoderTaskPresetStrings.H264AdaptiveBitrateMP4Set720p, inputAsset, "OutputAssetName", AssetCreationOptions.None);

// Submit the job.
job.Submit();

// Start a task to monitor the job progress by invoking a callback when its state or overall progress change in a single extension method.
job = await job.StartExecutionProgressTask(
    j =>
    {
        Console.WriteLine("Current job state: {0}", j.State);
        Console.WriteLine("Current job progress: {0}", j.GetOverallProgress());
    },
    CancellationToken.None);
```

### Get Job overall progress
Get the overall progress of a job by calculating the average progress of all its tasks using a single extension method for the [IJob](http://msdn.microsoft.com/library/microsoft.windowsazure.mediaservices.client.ijob.aspx) interface.
```csharp
CloudMediaContext context = new CloudMediaContext("%accountName%", "%accountKey%");

// The input asset for the task. Get a reference to it from the context.
IAsset inputAsset = null;

// Prepare a job ready to be submitted with a single task with one input/output asset using a single extension method.
IJob job = context.Jobs.CreateWithSingleTask(MediaProcessorNames.WindowsAzureMediaEncoder, MediaEncoderTaskPresetStrings.H264AdaptiveBitrateMP4Set720p, inputAsset, "OutputAssetName", AssetCreationOptions.None);

// Submit the job.
job.Submit();

// ...

// Refresh the job instance.
job = context.Jobs.Where(j => j.Id == job.Id).First();

// Get the overall progress of the job by calculating the average progress of all its tasks using a single extension method.
double jobOverallProgress = job.GetOverallProgress();
```

### Parse Media Services error messages in XML format
Parse exceptions with Windows Azure Media Services error messages in XML format.
```csharp
CloudMediaContext context = new CloudMediaContext("%accountName%", "%accountKey%");

// Create an empty asset.
IAsset asset = context.Assets.Create("MyAssetName", AssetCreationOptions.None);

try
{
    // Generate an error trying to delete the asset twice.
    asset.Delete();
    asset.Delete();
}
catch (Exception exception)
{
    // Parse the exception to get the error message from the Media Services XML response.
    Exception parsedException = MediaServicesExceptionParser.Parse(exception);
    string mediaServicesErrorMessage = parsedException.Message;
}
```
