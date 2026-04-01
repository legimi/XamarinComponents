
#addin nuget:?package=Cake.XCode
#addin nuget:?package=Cake.Xamarin.Build
#addin nuget:?package=Cake.Xamarin
#addin nuget:?package=Cake.FileHelpers
#addin nuget:?package=Cake.Yaml&loadDependencies=true
#addin nuget:?package=Cake.Json&loadDependencies=true

public enum TargetOS {
	Windows,
	Mac,
	Android,
	iOS,
	tvOS,
}

void BuildXCodeFatLibrary(FilePath xcodeProject, string target, string libraryTitle = null, FilePath fatLibrary = null, DirectoryPath workingDirectory = null, string targetFolderName = null, Dictionary<string, string> buildSettings = null)
{
	BuildXCodeFatLibrary_iOS(xcodeProject, target, libraryTitle, fatLibrary, workingDirectory, targetFolderName, buildSettings);
}

void BuildXCodeFatLibrary_iOS(FilePath xcodeProject, string target, string libraryTitle = null, FilePath fatLibrary = null, DirectoryPath workingDirectory = null, string targetFolderName = null, Dictionary<string, string> buildSettings = null)
{
	if (!IsRunningOnUnix())
	{
		Warning("{0} is not available on the current platform.", "xcodebuild");
		return;
	}

	libraryTitle = libraryTitle ?? target;
	fatLibrary = fatLibrary ?? string.Format("lib{0}.a", libraryTitle);
	workingDirectory = workingDirectory ?? Directory("./externals/");

	var output = string.Format("lib{0}.a", libraryTitle);
	var x86_64 = string.Format("lib{0}-x86_64.a", libraryTitle);
	var arm64 = string.Format("lib{0}-arm64.a", libraryTitle);

	var buildArch = new Action<string, string, FilePath>((sdk, arch, dest) => {
		if (!FileExists(dest))
		{
			XCodeBuild(new XCodeBuildSettings
			{
				Project = workingDirectory.CombineWithFilePath(xcodeProject).ToString(),
				Target = target,
				Sdk = sdk,
				Arch = arch,
				Configuration = "Release",
				BuildSettings = buildSettings
			});
			var tmpOutputPath = workingDirectory.Combine("build").Combine("Release-" + sdk);
			if (!string.IsNullOrEmpty (targetFolderName))
				tmpOutputPath = tmpOutputPath.Combine (targetFolderName);
			var outputPath = tmpOutputPath.CombineWithFilePath(output);

			CopyFile(outputPath, dest);
		}
	});

	buildArch("iphonesimulator", "x86_64", workingDirectory.CombineWithFilePath(x86_64));
	buildArch("iphoneos", "arm64", workingDirectory.CombineWithFilePath(arm64));

	RunLipoCreate(workingDirectory, fatLibrary, x86_64, arm64);
}

void BuildXCodeFatLibrary_tvOS(FilePath xcodeProject, string target, string libraryTitle = null, FilePath fatLibrary = null, DirectoryPath workingDirectory = null, string targetFolderName = null, Dictionary<string, string> buildSettings = null)
{
	if (!IsRunningOnUnix())
	{
		Warning("{0} is not available on the current platform.", "xcodebuild");
		return;
	}

	libraryTitle = libraryTitle ?? target;
	fatLibrary = fatLibrary ?? string.Format("lib{0}.a", libraryTitle);
	workingDirectory = workingDirectory ?? Directory("./externals/");

	var output = string.Format("lib{0}.a", libraryTitle);
	var x86_64 = string.Format("lib{0}-x86_64.a", libraryTitle);
	var arm64 = string.Format("lib{0}-arm64.a", libraryTitle);

	var buildArch = new Action<string, string, FilePath>((sdk, arch, dest) => {
		if (!FileExists(dest))
		{
			XCodeBuild(new XCodeBuildSettings
			{
				Project = workingDirectory.CombineWithFilePath(xcodeProject).ToString(),
				Target = target,
				Sdk = sdk,
				Arch = arch,
				Configuration = "Release",
				BuildSettings = buildSettings
			});
			var tmpOutputPath = workingDirectory.Combine("build").Combine("Release-" + sdk);
			if (!string.IsNullOrEmpty (targetFolderName))
				tmpOutputPath = tmpOutputPath.Combine (targetFolderName);
			var outputPath = tmpOutputPath.CombineWithFilePath(output);

			CopyFile(outputPath, dest);
		}
	});

	buildArch("appletvsimulator", "x86_64", workingDirectory.CombineWithFilePath(x86_64));
	buildArch("appletvos", "arm64", workingDirectory.CombineWithFilePath(arm64));

	RunLipoCreate(workingDirectory, fatLibrary, x86_64, arm64);
}

void BuildXCodeFatLibrary_macOS(FilePath xcodeProject, string target, string libraryTitle = null, FilePath fatLibrary = null, DirectoryPath workingDirectory = null, string targetFolderName = null, Dictionary<string, string> buildSettings = null)
{
	if (!IsRunningOnUnix())
	{
		Warning("{0} is not available on the current platform.", "xcodebuild");
		return;
	}

	libraryTitle = libraryTitle ?? target;
	fatLibrary = fatLibrary ?? string.Format("lib{0}.a", libraryTitle);
	workingDirectory = workingDirectory ?? Directory("./externals/");

	var output = string.Format("lib{0}.a", libraryTitle);
	var x86_64 = string.Format("lib{0}-x86_64.a", libraryTitle);

	var buildArch = new Action<string, string, FilePath>((sdk, arch, dest) => {
		if (!FileExists(dest))
		{
			XCodeBuild(new XCodeBuildSettings
			{
				Project = workingDirectory.CombineWithFilePath(xcodeProject).ToString(),
				Target = target,
				Sdk = sdk,
				Arch = arch,
				Configuration = "Release",
				BuildSettings = buildSettings
			});
			var tmpOutputPath = workingDirectory.Combine("build").Combine("Release");
			if (!string.IsNullOrEmpty (targetFolderName))
				tmpOutputPath = tmpOutputPath.Combine (targetFolderName);
			var outputPath = tmpOutputPath.CombineWithFilePath(output);

			CopyFile(outputPath, dest);
		}
	});

	buildArch("macosx", "x86_64", workingDirectory.CombineWithFilePath(x86_64));

	RunLipoCreate(workingDirectory, fatLibrary, x86_64);
}

void BuildXCode (FilePath project, string target, string libraryTitle, DirectoryPath workingDirectory, TargetOS os, Dictionary<string, string> buildSettings = null)
{
	if (!IsRunningOnUnix ()) {
		Warning("{0} is not available on the current platform.", "xcodebuild");
		return;
	}
	
	var fatLibrary = string.Format("lib{0}.a", libraryTitle);

	var output = string.Format ("lib{0}.a", libraryTitle);
	var x86_64 = string.Format ("lib{0}-x86_64.a", libraryTitle);
	var arm64 = string.Format ("lib{0}-arm64.a", libraryTitle);
	
	var buildArch = new Action<string, string, FilePath> ((sdk, arch, dest) => {
		if (!FileExists (dest)) {
			XCodeBuild (new XCodeBuildSettings {
				Project = workingDirectory.CombineWithFilePath (project).ToString (),
				Target = target,
				Sdk = sdk,
				Arch = arch,
				Configuration = "Release",
				BuildSettings = buildSettings
			});
			var outputPath = workingDirectory.Combine ("build").Combine (os == TargetOS.Mac ? "Release" : ("Release-" + sdk)).Combine (target).CombineWithFilePath (output);
			CopyFile (outputPath, dest);
		}
	});
	
	if (os == TargetOS.Mac) {
		// not supported anymore
		buildArch ("macosx", "x86_64", workingDirectory.CombineWithFilePath (x86_64));

		if (!FileExists (workingDirectory.CombineWithFilePath (fatLibrary))) {
			RunLipoCreate (workingDirectory, fatLibrary, x86_64);
		}
	} else if (os == TargetOS.iOS) {
		buildArch ("iphonesimulator", "x86_64", workingDirectory.CombineWithFilePath (x86_64));

		buildArch ("iphoneos", "arm64", workingDirectory.CombineWithFilePath (arm64));

		if (!FileExists (workingDirectory.CombineWithFilePath (fatLibrary))) {
			RunLipoCreate (workingDirectory, fatLibrary, x86_64, arm64);
		}
	} else if (os == TargetOS.tvOS) {
		buildArch ("appletvsimulator", "x86_64", workingDirectory.CombineWithFilePath (x86_64));

		buildArch ("appletvos", "arm64", workingDirectory.CombineWithFilePath (arm64));

		if (!FileExists (workingDirectory.CombineWithFilePath (fatLibrary))) {
			RunLipoCreate (workingDirectory, fatLibrary, x86_64, arm64);
		}
	}
}

void BuildDynamicXCode (FilePath project, string target, string libraryTitle, DirectoryPath workingDirectory, TargetOS os, Dictionary<string, string> buildSettings = null)
{
	if (!IsRunningOnUnix ()) {
		Warning("{0} is not available on the current platform.", "xcodebuild");
		return;
	}
	
	var fatLibrary = (DirectoryPath)string.Format("{0}.framework", libraryTitle);
	var fatLibraryPath = workingDirectory.Combine (fatLibrary);

	var output = (DirectoryPath)string.Format ("{0}.framework", libraryTitle);
	var x86_64 = (DirectoryPath)string.Format ("{0}-x86_64.framework", libraryTitle);
	var arm64 = (DirectoryPath)string.Format ("{0}-arm64.framework", libraryTitle);

	var buildArch = new Action<string, string, DirectoryPath> ((sdk, arch, dest) => {
		if (!DirectoryExists (dest)) {
			XCodeBuild (new XCodeBuildSettings {
				Project = workingDirectory.CombineWithFilePath (project).ToString (),
				Target = target,
				Sdk = sdk,
				Arch = arch,
				Configuration = "Release",
				BuildSettings = buildSettings
			});
			var outputPath = workingDirectory.Combine ("build").Combine (os == TargetOS.Mac ? "Release" : ("Release-" + sdk)).Combine (target).Combine (output);
			CopyDirectory (outputPath, dest);
		}
	});

	if (os == TargetOS.Mac) {
		buildArch ("macosx", "x86_64", workingDirectory.Combine (x86_64));

		if (!DirectoryExists (fatLibraryPath)) {
			CopyDirectory (workingDirectory.Combine (x86_64), fatLibraryPath);
			RunLipoCreate (workingDirectory, fatLibrary.CombineWithFilePath (libraryTitle),
				x86_64.CombineWithFilePath (libraryTitle));
		}
	} else if (os == TargetOS.iOS) {
		buildArch ("iphonesimulator", "x86_64", workingDirectory.Combine (x86_64));

		buildArch ("iphoneos", "arm64", workingDirectory.Combine (arm64));

		if (!DirectoryExists (fatLibraryPath)) {
			CopyDirectory (workingDirectory.Combine (arm64), fatLibraryPath);
			RunLipoCreate (workingDirectory, fatLibrary.CombineWithFilePath (libraryTitle), 
				x86_64.CombineWithFilePath (libraryTitle),
				arm64.CombineWithFilePath (libraryTitle));
		}
	} else if (os == TargetOS.tvOS) {
		buildArch ("appletvsimulator", "x86_64", workingDirectory.Combine (x86_64));

		buildArch ("appletvos", "arm64", workingDirectory.Combine (arm64));

		if (!DirectoryExists (fatLibraryPath)) {
			CopyDirectory (workingDirectory.Combine (arm64), fatLibraryPath);
			RunLipoCreate (workingDirectory, fatLibrary.CombineWithFilePath (libraryTitle), 
				x86_64.CombineWithFilePath (libraryTitle),
				arm64.CombineWithFilePath (libraryTitle));
	}
}
}

void BuildDynamicXCode_XCFramework (FilePath project, string target, string libraryTitle, DirectoryPath workingDirectory, TargetOS os, Dictionary<string, string> buildSettings = null)
{
	if (!IsRunningOnUnix ()) {
		Warning("{0} is not available on the current platform.", "xcodebuild");
		return;
	}

	var xcframework = (DirectoryPath)string.Format("{0}.xcframework", libraryTitle);
	var xcframeworkPath = workingDirectory.Combine (xcframework);

	var output = (DirectoryPath)string.Format ("{0}.framework", libraryTitle);
	var simulator = (DirectoryPath)string.Format ("{0}-simulator.framework", libraryTitle);
	var sim_x86_64 = (DirectoryPath)string.Format ("{0}-sim-x86_64.framework", libraryTitle);
	var sim_arm64 = (DirectoryPath)string.Format ("{0}-sim-arm64.framework", libraryTitle);
	var device = (DirectoryPath)string.Format ("{0}-device.framework", libraryTitle);

	var buildArch = new Action<string, string, DirectoryPath> ((sdk, arch, dest) => {
		if (!DirectoryExists (dest)) {
			XCodeBuild (new XCodeBuildSettings {
				Project = workingDirectory.CombineWithFilePath (project).ToString (),
				Target = target,
				Sdk = sdk,
				Arch = arch,
				Configuration = "Release",
				BuildSettings = buildSettings
			});
			var outputPath = workingDirectory.Combine ("build").Combine (os == TargetOS.Mac ? "Release" : ("Release-" + sdk)).Combine (target).Combine (output);
			CopyDirectory (outputPath, dest);
		}
	});

	if (os == TargetOS.iOS) {
		// Build for device (arm64)
		buildArch ("iphoneos", "arm64", workingDirectory.Combine (device));

		// Build for simulator (x86_64 and arm64)
		buildArch ("iphonesimulator", "x86_64", workingDirectory.Combine (sim_x86_64));
		buildArch ("iphonesimulator", "arm64", workingDirectory.Combine (sim_arm64));

		// Create fat simulator framework (x86_64 + arm64)
		if (!DirectoryExists (workingDirectory.Combine (simulator))) {
			CopyDirectory (workingDirectory.Combine (sim_arm64), workingDirectory.Combine (simulator));
			RunLipoCreate (workingDirectory, simulator.CombineWithFilePath (libraryTitle),
				sim_x86_64.CombineWithFilePath (libraryTitle),
				sim_arm64.CombineWithFilePath (libraryTitle));
		}

		// Create XCFramework from device + simulator frameworks
		// xcodebuild -create-xcframework requires framework dir name to match the binary name inside,
		// so we copy to temp dirs with the original framework name.
		if (!DirectoryExists (xcframeworkPath)) {
			var tmpDevice = workingDirectory.Combine ("_tmp_device").Combine (output);
			var tmpSimulator = workingDirectory.Combine ("_tmp_simulator").Combine (output);
			EnsureDirectoryExists (workingDirectory.Combine ("_tmp_device"));
			EnsureDirectoryExists (workingDirectory.Combine ("_tmp_simulator"));
			CopyDirectory (workingDirectory.Combine (device), tmpDevice);
			CopyDirectory (workingDirectory.Combine (simulator), tmpSimulator);

			StartProcess ("xcodebuild", new ProcessSettings {
				Arguments = string.Format(
					"-create-xcframework -framework \"{0}\" -framework \"{1}\" -output \"{2}\"",
					tmpDevice,
					tmpSimulator,
					xcframeworkPath)
			});

			DeleteDirectory (workingDirectory.Combine ("_tmp_device"), new DeleteDirectorySettings { Recursive = true });
			DeleteDirectory (workingDirectory.Combine ("_tmp_simulator"), new DeleteDirectorySettings { Recursive = true });
		}
	} else if (os == TargetOS.tvOS) {
		var tv_sim_x86_64 = (DirectoryPath)string.Format ("{0}-tvsim-x86_64.framework", libraryTitle);
		var tv_sim_arm64 = (DirectoryPath)string.Format ("{0}-tvsim-arm64.framework", libraryTitle);
		var tv_simulator = (DirectoryPath)string.Format ("{0}-tvsimulator.framework", libraryTitle);
		var tv_device = (DirectoryPath)string.Format ("{0}-tvdevice.framework", libraryTitle);

		buildArch ("appletvos", "arm64", workingDirectory.Combine (tv_device));
		buildArch ("appletvsimulator", "x86_64", workingDirectory.Combine (tv_sim_x86_64));
		buildArch ("appletvsimulator", "arm64", workingDirectory.Combine (tv_sim_arm64));

		if (!DirectoryExists (workingDirectory.Combine (tv_simulator))) {
			CopyDirectory (workingDirectory.Combine (tv_sim_arm64), workingDirectory.Combine (tv_simulator));
			RunLipoCreate (workingDirectory, tv_simulator.CombineWithFilePath (libraryTitle),
				tv_sim_x86_64.CombineWithFilePath (libraryTitle),
				tv_sim_arm64.CombineWithFilePath (libraryTitle));
		}

		if (!DirectoryExists (xcframeworkPath)) {
			var tmpDevice = workingDirectory.Combine ("_tmp_device").Combine (output);
			var tmpSimulator = workingDirectory.Combine ("_tmp_simulator").Combine (output);
			EnsureDirectoryExists (workingDirectory.Combine ("_tmp_device"));
			EnsureDirectoryExists (workingDirectory.Combine ("_tmp_simulator"));
			CopyDirectory (workingDirectory.Combine (tv_device), tmpDevice);
			CopyDirectory (workingDirectory.Combine (tv_simulator), tmpSimulator);

			StartProcess ("xcodebuild", new ProcessSettings {
				Arguments = string.Format(
					"-create-xcframework -framework \"{0}\" -framework \"{1}\" -output \"{2}\"",
					tmpDevice,
					tmpSimulator,
					xcframeworkPath)
			});

			DeleteDirectory (workingDirectory.Combine ("_tmp_device"), new DeleteDirectorySettings { Recursive = true });
			DeleteDirectory (workingDirectory.Combine ("_tmp_simulator"), new DeleteDirectorySettings { Recursive = true });
		}
	}
}

void DownloadMonoSources (string tag, DirectoryPath dest, params string[] urls)
{
	var rootUrl = $"https://github.com/mono/mono/raw/{tag}";

	EnsureDirectoryExists (dest);
	foreach (var originalUrl in urls) {
		// make sure the urls are rooted
		var url = originalUrl;
		if (!url.StartsWith ("http:") && !url.StartsWith ("https:")) {
			url = $"{rootUrl}/{url}";
		}
		// get the path parts
		var file = url.Substring (url.LastIndexOf ("/") + 1);
		var dir = url.Substring (0, url.LastIndexOf ("/"));
		var destFile = dest.CombineWithFilePath (file);
		// download the file
		if (!FileExists (destFile)) {
			Information ($"Downloading '{url}' to '{destFile}'...");
			DownloadFile (url, destFile);
		}
		// if this is a .sources file, download all the listed files too
		if (file.EndsWith (".sources")) {
			var listedFiles = FileReadLines (destFile)
				.Where (f => !f.StartsWith (".."))
				.Select (f => $"{dir}/{f}")
				.ToArray ();
			DownloadMonoSources (tag, dest, listedFiles);
		}
	}
}
