# DNSChanger

A DNS changer desktop application for Windows.

## Preview

![Screenshot of the application](https://raw.githubusercontent.com/luisbraganca/dns-changer/master/Screenshots/preview.png)

## Technical details

This application grabs all the Network Interfaces on the running device and changes all their DNS server addresses according to the one listed on this [link](https://gist.githubusercontent.com/luisbraganca/1c756ab03c94ce49f60be89092f28c0b/raw/2f7bbf59ed80cd499c5873deedea655a206c09e9/opendns.txt) with a single button click.
The advantage of having the DNS server address on a website rather than being hardcoded on the application is that you can publish or share it with anyone, and you won't need to give them more releases if you ever need to change the DNS server, just simply modify the raw text website content. It's also possible to reset all the network interface's DNS server address to the default value.

### Functionalities

* Change DNS server addresses of all the network interfaces of the device
* Reset the DNS server addresses of all the network interfaces on the device
* Optionally start Chrome or Firefox (incognito mode) after the DNS is changed

## Getting started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

It's highly recommended that you edit this project using Visual Studio from Microsoft that can be downloaded [here](https://www.visualstudio.com/vs/community/) since it was developed using that same tool (Visual Studio Community 2017).

### Setting up Visual Studio

While setting up your Visual Studio installation, mark at least the following components to be installed:

- [x] Universal Windows Platform Development
- [x] .NET desktop development

After you finish installing Visual Studio (should take a while), run it and open [DNSChanger.sln](https://github.com/luisbraganca/dns-changer/blob/master/DNSChanger/DNSChanger.sln) or simply double click the file.

Please read the notes before compiling the application.

### Resources

* Icon

The icon used was made by:
© 2007 Nuno Pinheiro & David Vignoni & David Miller & Johann Ollivier Lapeyre & Kenneth Wimer & Riccardo Iaconelli / KDE / LGPL 3

* DNS Server

The DNS server used is from:

© [OpenDNS](https://www.opendns.com/), 2017

## Notes

As you can see on [app.manifest](https://github.com/luisbraganca/dns-changer/blob/master/DNSChanger/app.manifest), this application requires admin privileges
```
<requestedExecutionLevel level="requireAdministrator" uiAccess="false" />
```
So you'll need admin rights on your development environment as well in order to run the application, simply run Visual Studio as an Administrator.

## Authors

* **Luís Bragança Silva** - *Initial work*
