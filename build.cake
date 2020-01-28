var target = Argument<string>("Target", "Default");
var configuration = Argument<string>("Configuration", "Release");
bool publishWithoutBuild = Argument<bool>("PublishWithoutBuild", false);
string nugetPrereleaseTextPart = Argument<string>("PrereleaseText", "alpha");

var artifactsDirectory = Directory("./artifacts");
var samplesDirectory = Directory("./samples");
string testResultDir = "./temp/";
var isRunningOnBuildServer = !BuildSystem.IsLocalBuild;

var msBuildSettings = new DotNetCoreMSBuildSettings();

if (HasArgument("PrereleaseNumber"))
{
    msBuildSettings.WithProperty("PrereleaseNumber", Argument<string>("PrereleaseNumber"));
    msBuildSettings.WithProperty("VersionSuffix", nugetPrereleaseTextPart + Argument<string>("PrereleaseNumber"));
}

if (HasArgument("VersionPrefix"))
{
    msBuildSettings.WithProperty("VersionPrefix", Argument<string>("VersionPrefix"));
}

Task("Clean-Artifacts")
    .Does(() =>
{
    CleanDirectory(artifactsDirectory);
});

Task("Build")
    .Does(() =>
{
    var dotNetCoreSettings = new DotNetCoreBuildSettings()
            {
                Configuration = configuration,
                MSBuildSettings = msBuildSettings
            };
    DotNetCoreBuild("Cmdty.TimeSeries.sln", dotNetCoreSettings);
});

Task("Test-C#")
    .IsDependentOn("Build")
    .Does(() =>
{
    Information("Cleaning test output directory");
    CleanDirectory(testResultDir);

    var projects = GetFiles("./tests/**/*.Test.csproj");
    
    foreach(var project in projects)
    {
        Information("Testing project " + project);
        DotNetCoreTest(
            project.ToString(),
            new DotNetCoreTestSettings()
            {
                ArgumentCustomization = args=>args.Append($"/p:CollectCoverage=true /p:CoverletOutputFormat=cobertura"),
                Logger = "trx",
                ResultsDirectory = testResultDir,
                Configuration = configuration,
                NoBuild = true
            });
    }
});

Task("Build-Samples")
	.Does(() =>
{
	var dotNetCoreSettings = new DotNetCoreBuildSettings()
        {
            Configuration = configuration,
        };
	DotNetCoreBuild("samples/Cmdty.TimeSeries.Samples.sln", dotNetCoreSettings);
});

Task("Verify-TryDotNetDocs")
    .IsDependentOn("Build-Samples")
	.Does(() =>
{
	StartProcessThrowOnError("dotnet", $"try verify {samplesDirectory}");
});

private void StartProcessThrowOnError(string applicationName, params string[] processArgs)
{
    var argsBuilder = new ProcessArgumentBuilder();
    foreach(string processArg in processArgs)
    {
        argsBuilder.Append(processArg);
    }
    int exitCode = StartProcess(applicationName, new ProcessSettings {Arguments = argsBuilder});
    if (exitCode != 0)
        throw new ApplicationException($"Starting {applicationName} in new process returned non-zero exit code of {exitCode}");
}

using System.Reflection;
private string GetAssemblyVersion(string configuration, string workingDirectory)
{
	// TODO find better way of doing this!
	string assemblyPath = System.IO.Path.Combine(workingDirectory, "src", "Cmdty.TimeSeries", "bin", configuration, "netstandard2.0", "Cmdty.TimeSeries.dll");
	Assembly assembly = Assembly.LoadFrom(assemblyPath);
	string productVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
	return productVersion;
}

Task("Pack-NuGet")
	.IsDependentOn("Build-Samples")
	.IsDependentOn("Test-C#")
	.IsDependentOn("Clean-Artifacts")
	.Does(() =>
{
	var dotNetPackSettings = new DotNetCorePackSettings()
                {
                    Configuration = configuration,
                    OutputDirectory = artifactsDirectory,
                    NoRestore = true,
                    MSBuildSettings = msBuildSettings
                };
	DotNetCorePack("src/Cmdty.TimeSeries/Cmdty.TimeSeries.csproj", dotNetPackSettings);
});

private string GetEnvironmentVariable(string envVariableName)
{
    string envVariableValue = EnvironmentVariable(envVariableName);
    if (string.IsNullOrEmpty(envVariableValue))
        throw new ApplicationException($"Environment variable '{envVariableName}' has not been set.");
    return envVariableValue;
}

var publishNuGetTask = Task("Publish-NuGet")
    .Does(() =>
{
    string nugetApiKey = GetEnvironmentVariable("NUGET_API_KEY");

    var nupkgPath = GetFiles(artifactsDirectory.ToString() + "/*.nupkg").Single();

    NuGetPush(nupkgPath, new NuGetPushSettings 
    {
        ApiKey = nugetApiKey,
        Source = "https://api.nuget.org/v3/index.json"
    });
});

if (!publishWithoutBuild)
{
    publishNuGetTask.IsDependentOn("Pack-NuGet");
}
else
{
    Information("Publishing without first building as PublishWithoutBuild variable set to true.");
}

Task("Default")
	.IsDependentOn("Verify-TryDotNetDocs")
    .IsDependentOn("Pack-NuGet");

RunTarget(target);