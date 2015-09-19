# PlatformAdapter  
<img src="https://raw.githubusercontent.com/thomasgalliker/PlatformAdapter/master/PlatformAdapter.NuGet/PlatformAdapterIcon.png" alt="PlatformAdapter" align="right" width="200" height="200"> 

The main goal of PlatformAdapter project is to have a component which allows you to probe from a platform-independent interface to its platform-specific implementation. This functionality is often needed when cross-platform compatible 
code is written. Abstractions need to be resolved into implementations at runtime. 

Imagine following scenario: You have a library which targets multiple platforms (e.g. a photo camera library which you want to use on Windows Phone, Xamarin.Android, Xamarin.iOS, etc) and you want to expose an abstraction onf all camera 
related functionalities in an  interface called IPhotoCamera. The purpose of this interface is to abstract platform-dependent code. The concrete implementation(s) of PhotoCamera will be maintained in seperate, platform-specific assemblies. 
How could you - on each platform - find the implementation for this IPhotoCamera interface? This is exactly where PlatformAdapter comes to the rescue! 

### Download and Install PlatformAdapter 
This library is available on NuGet: https://www.nuget.org/packages/CrossPlatformAdapter/ 
Use the following command to install PlatformAdapter using NuGet package manager console: 

    PM> Install-Package CrossPlatformAdapter 

You can use this library in any .Net project which is compatible to PCL (e.g. Xamarin Android, iOS, Windows Phone, Windows Store, Universal Apps, etc.) 

### API Usage 
#### Register probing strategies
In order to find platform-specific types based on their platform-independent interface, PlatformAdapter makes use of different strategies. By default, following two strategies are configured:
- DefaultProbingStrategy: Probes in the path of the current assembly for platform-specific types.
- PlatformProbingStrategy: Adds ".Platform" to the current assembly name in order to probe for platform-specific types.

You are free to define your own probing strategies. The only thing you need to do is:
- Create a new class and implement IProbingStrategy
- Add your implementation of IProbingStrategy to the PlatformAdapter by calling AddProbingStrategy method

#### Resolve a plattform-specific type from an interface 
If you have a plattform-agnostic interface, e.g. IPhotoCamera, and you want to resolve the .Net System.Type of the implementation for the platform you're running on, issue the following command: 

``` 
Type photoCameraType = PlatformAdapter.Current.ResolveClassType<IPhotoCamera>(); 
``` 

#### Resolve a plattform-specific object from an interface 
If you have a plattform-agnostic interface, e.g. IPhotoCamera, and you want PlatformAdapter to return a concrete object of the platform-specific implementation, issue the following command: 

``` 
IPhotoCamera photoCamera = PlatformAdapter.Current.Resolve<IPhotoCamera>(); 
``` 
Recommendation of the author: It is highly recommended to delegate the taks of dependeny management to an IoC framework, e.g. Autofac, Unity, SimpleIoc, etc. PlatformAdapter does only provide very basic  


### License 
PlatformAdapter is Copyright &copy; 2015 [Thomas Galliker](https://ch.linkedin.com/in/thomasgalliker). Free for non-commercial use. For commercial use please contact the author. 
