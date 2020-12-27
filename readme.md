![.NET](https://github.com/walljm/googlewifionhub/workflows/.NET/badge.svg)

# JMW.Google.OnHub

JMW.Google.OnHub is a Library and Console application for querying the Google OnHub WIFI router.

## Installation

You can download the prebuilt binaries from here: 

or

You can clone and build the dotnet project and build with dotnet.


```bash
$ dotnet restore
$ dotnet build
```

## Usage

To print out the data on the console, just pass in the router ip like so
```bash
onhub -t 192.168.84.1
```

To get the data back as JSON add the `-j` or `--json` flag
```bash
onhub -t 192.168.84.1 -j true
```

To get the data back a specific category of data use the `-c` or `--category` flag 
with one of the supported options:

* all (default)
* arp
* cam
* ifc

```bash
onhub -t 192.168.84.1 -c arp
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)