# Fluent86
**Fluent86** is a configuration manager for the [86Box emulator](https://github.com/86Box/86Box). It's released under the MIT license, so it can be freely distributed with 86Box. See the `LICENSE` file for license information and `AUTHORS` for a complete list of contributors and authors.

It's written in C# with WinUI 3, and started as a fork of the official [86Box Manager project](https://github.com/86Box/86BoxManager).

## Features
* Modern interface
* Powerful, lightweight and completely optional
* Create multiple isolated virtual machines
* Give each virtual machine a unique name and an optional description
* Run multiple virtual machines at the same time
* Control virtual machines from the Manager (pause, reset, etc.)

## System requirements
System requirements are the same as for 86Box. Additionally, the following is required:  

* [86Box](https://github.com/86Box/86Box/releases) (latest version)
* [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

## Support
If you have any issues, questions, suggestions, etc., please post an issue on this GitHub project.

Fluent86 is by no means associated with the official 86Box nor the 86BoxManager projects and won't answer your support requests for Fluent86.

## How to use
⚠️ No released builds officially exists yet.

## How to build
1. Clone the repo
2. Open `Fluent86.sln` solution file in Visual Studio
3. Make your changes
4. Choose the `Release` configuration and `x86` platform/CPU
5. Build the solution
6. `Fluent86.exe` is now in `Bin\x86\Release\`
