# Yi Camera FTP Scanner and Video Clips/Images Downloader with embedded FTP Server for other Cameras

- **Automatic** network scanning for YI Cameras
- **Embedded** FTP server for other cameras

The application supports two modes - arguments explicitly specified in command prompt and configuration based.
It can also run as Windows service.

[![GitHub release](https://img.shields.io/github/release/AndMu/YiScanner.svg)](https://github.com/AndMu/YiScanner/releases)

## Configuration via service.json

```
{
  "Scan": 30,
  "Timeout": 1200,
  "Archive": 2,
  "Output": {
    "Compress": false,
    "Images": false,
    "Out": "D:/Monitor"
  },    
  "AutoDiscovery": {
    "On": true,
    "NetworkMask": "192.168.0.0/255.255.255.0"
  },

  "YiFtp": {
    "Path": "/tmp/sd/record/",
    "Password": "",
    "Login": "root",
    "FileMask": "*.mp4"
  }
} 
```

- **Scan** - frequency of FTP scan (in seconds)
- **Timeout** - FTP scan Timeout(hard) (in seconds)
- **Archive** - delete previously downloaded old files. Number specifies how many days you want to keep history.

## Output

- **Compress** - do you want to compress files
- **Out** - location of downloaded files
- **Images** - Download as a snapshot or video
- **Archive** - delete previously downloaded old files. Number specifies how many days you want to keep history.

## Yi FTP Details

- **Path** - Where images are stored
- **Login** - Login
- **FileMask** - Files to download
- **Password** - Password

## Embedded FTP

- **Path** - Local sub-folder where images will be stored
- **Port** - Server port


## Actions on image

If you want some action to be executed on each retrieved image:
```
"Action":{
    "Type": "Execute",
    "Cmd": "%1",
    "Payload": null
    }
```

## Manual YI Camera setup

```
{
  "Scan": 30,
  "Timeout": 1200,
  "Archive": 2,
  "Output": {
    "Compress": false,
    "Images": false,
    "Out": "D:/Monitor"
  },  
  "YiFtp": {
    "Path": "/tmp/sd/record/",
    "Password": "",
    "Login": "root",
    "FileMask": "*.mp4"
  },
  "Known":{
    "Cameras": "1080i",
    "Hosts": "192.168.0.22"
    }
} 
```

If you don't want automatic camera discovery, you can predefine list of cameras.

- **Cameras** - list of cameras
- **Hosts** - list of hosts. 

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

