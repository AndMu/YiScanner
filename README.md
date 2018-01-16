# Yi Camera FTP Scanner and Video Clips/Images Downloader

The application supports two modes - arguments explicitly specified in command prompt and configuration based.
It can also run as Windows service.

[![GitHub release](https://img.shields.io/github/release/AndMu/YiScanner.svg)](https://github.com/AndMu/YiScanner/releases)

## Configuration via service.json
In this mode, application will monitor designated cameras

```
"Scan": 30,
"Cameras": "1080i,720p",
"Hosts": "192.168.0.103,192.168.0.129",
"Compress": false,
"Archive": 2,
"Images": false,
"All": false,
"Out": "D:/Cloud/Camera/Monitor",
"Action": null,    
```

## Settings:
- **Cameras** - list of cameras
- **Hosts** - list of camera ips. 
- **Compress** - do you want to compress retrieved video/images
- **Out** - location of downloaded files
- **Scan** - frequency of FTP scan (in seconds)
- **Archive** - delete previously downloaded old files. Number specifies how many days you want to keep history
- **Images** - Do you want to retrieve video as images
- **Action** - Execute action on each retrieved image

## Actions on image

If you want some action to be executed on each retrieved image:
```
"Action":{
    "Type": "Execute",
    "Cmd": "%1",
    "Payload": null
    }
```

%1 will be replaced by file name. 


## Install as Windows service
```
Wikiled.YiScanner.exe install
```

## Running with arguments

### Monitoring and downloading latest clips

```
Wikiled.YiScanner.exe Monitor -Cameras=1080i -Hosts=192.168.0.202 [-Compress] -Out=c:\out -Scan=10 [-Archive=2]
```

### Download once files, which haven't been downloaded yet

```
Wikiled.YiScanner.exe Download -Cameras=1080i -Hosts=192.168.0.202 [-Compress] -Out=c:\out [-Archive=2]
```

Options:
- **Cameras** - list of cameras
- **Hosts** - list of hosts. 
- **Compress** - do you want to compress files
- **Out** - location of downloaded files
- **Scan** - frequency of FTP scan (in seconds)
- **Archive** - delete previously downloaded old files. Number specifies how many days you want to keep history.


# FTP configuration 
FTP configuration can be modified in file **appsettings.json**