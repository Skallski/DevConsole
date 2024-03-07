# DevConsole

<p align="center">
	<img alt="package.json version" src ="https://img.shields.io/github/package-json/v/Skallu0711/DevConsole" />
	<a href="https://github.com/Skallu0711/DevConsole/issues">
		<img alt="GitHub issues" src ="https://img.shields.io/github/issues/Skallu0711/DevConsole" />
	</a>
	<a href="https://github.com/Skallu0711/DevConsole/pulls">
		<img alt="GitHub pull requests" src ="https://img.shields.io/github/issues-pr/Skallu0711/DevConsole" />
	</a>
	<a href="https://github.com/Skallu0711/DevConsole/blob/master/LICENSE">
		<img alt="GitHub license" src ="https://img.shields.io/github/license/Skallu0711/DevConsole" />
	</a>
	<img alt="GitHub last commit" src ="https://img.shields.io/github/last-commit/Skallu0711/DevConsole" />
</p>

### Introduction
DevConsole is a lightweight multi-functional UI based console window tool. <br>
The main purpose behind it's creation was to provide easy to use tool that will speed up the debugging and testing inside built projects.

### Logger
Dev console's main feature is to display easy to read informations about logs (Info, Warning, Eror, Assert) and exceptions inside the build.
Log messages and their timestamps are printed on vertical scroll view. In order to view stack trace, simply click on the log.
You can also use filters to display only the logs of certain type.

### CommandPrompt
Dev console comes with built in command prompt tool. It allows to display and modify values of primitive variables. <br>
Simply add ```DevConsoleField``` attribute before declaring global variable in order to mark the field as usable by the DevConsole. <br>
Like in this example:
```cs
[SerializeField, DevConsoleField("testIntVariable")] private int _variableToModify = 5;
```
Then You can use following commands:
```
/get testIntVariable      - prints value of "_variableToModify"
/set testIntVariable 10   - modifies value of "_variableToModify"
/getAll                   - prints all variables with DevConsoleField attribute
```
The CommandPrompt functionality if easily scalable, so You can add more comands that will suit Your needs.

### Instalation
This is an embeded Unity package. In order to install it, use package manager tool (Window -> Package Manager). <br>
Then import package by git url or locally from disk.
