WAWSDeploy
==========

Simple command line tool to deploy a folder or a zip file to an Azure Website using WebDeploy. See this [post](http://blog.davidebbo.com/2014/03/WAWSDeploy.html)
for details.

**Note**: the `.PublishSettings` file discussed here is the one you can download for a specific  Azure Web App via the portal.

Sample use:

    WAWSDeploy c:\somefolder mysite.PublishSettings
    WAWSDeploy c:\somefile.zip mysite.PublishSettings

With optional password argument

    WAWSDeploy c:\somefolder mysite.PublishSettings /p mypubsettingspassword
    WAWSDeploy c:\somefolder mysite.PublishSettings /password mypubsettingspassword

Allowing untrusted cert

    WAWSDeploy c:\somefolder mysite.PublishSettings /au

Delete files on the remote host

    WAWSDeploy c:\somefolder mysite.PublishSettings /d

Use checksums instead of timestamps

    WAWSDeploy c:\somefolder mysite.PublishSettings /c

Verbose Logging

    WAWSDeploy c:\somefolder mysite.PublishSettings /v

[What If deployment](http://www.asp.net/web-forms/tutorials/deployment/advanced-enterprise-web-deployment/performing-a-what-if-deployment)

    WAWSDeploy c:\somefolder mysite.PublishSettings /w

Target Path - The virtual directory to deploy to
	
    WAWSDeploy C:\somefolder mysite.PublisSettings /t someVirtualDirectoryName

Target Path - Using a physical target folder

    WAWSDeploy C:\somefolder mysite.PublisSettings /t d:\home\site\blah


    

## To build this project

    npm install -g grunt-cli
    npm install
    grunt

## History

### 2/4/2017: v1.8

Add /AppOffline switch

### 5/25/2016: v1.7

Add /TargetPath switch

### 1/9/2015: v1.6

Add /TargetPath switch

### 11/12/2014: v1.5

Add -t switch to allow targeted deployment to a specific virtual directory.

### 5/1/2014: v1.4

Add -WhatIf and -deleteexistingfiles switches

### 3/26/2014: v1.3

Add support for untrusted certs.

### 3/6/2014: v1.2

Support optionally passing password

### 3/5/2014: v1.1

Support publishing from zip files

### 3/5/2014: v1.0

Original version
