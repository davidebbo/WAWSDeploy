WAWSDeploy
==========

Simple command line tool to deploy a folder or a zip file to an Azure Website using WebDeploy. See this [post](http://blog.davidebbo.com/2014/03/WAWSDeploy.html)
for details.

Sample use:

    WAWSDeploy c:\somefolder mysite.PublishSettings
    WAWSDeploy c:\somefile.zip mysite.PublishSettings

With optional password argument

    WAWSDeploy c:\somefolder mysite.PublishSettings /p mypubsettingspassword
    WAWSDeploy c:\somefolder mysite.PublishSettings /password mypubsettingspassword

Allowing untrusted cert

    WAWSDeploy c:\somefolder mysite.PublishSettings /au


Do not delete files on the remote server

	WAWSDeploy c:\somefolder mysite.PublishSettings /d


View all remote server output on the console

	WAWSDeploy c:\somefolder mysite.PublishSettings /v

	


## To build this project

    npm install -g grunt-cli
    npm install
    grunt

## History

### 3/26/2013: v1.3

Add support for untrusted certs.

### 3/6/2013: v1.2

Support optionally passing password

### 3/5/2013: v1.1

Support publishing from zip files

### 3/5/2013: v1.0

Original version
