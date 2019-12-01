
# AWS CodeDeploy Global Tool

![Build Status](https://travis-ci.org/mziyabo/codedeploy-globaltool.svg?branch=master) ![NuGet](https://img.shields.io/nuget/v/Tool.AWS.CodeDeploy.svg)

Deploy .NET Core applications using CodeDeploy

# Installation
1. Install aws-cli and [configure credentials](https://docs.aws.amazon.com/cli/latest/userguide/cli-chap-configure.html). Alternatively setup AWS Credentials from [Environment Variables](https://docs.aws.amazon.com/sdk-for-javascript/v2/developer-guide/loading-node-credentials-environment.html)
2. Install the extension using the dotnet tool install command 
```
dotnet tool install -g Tool.AWS.CodeDeploy
```

# Available Commands

**Create Deployment**

Zips an application path (Default './') and creates S3 revision and then creates and CodeDeploy deployment:
```
dotnet codedeploy --application <value> --deploymentGroup <value> --s3-location s3://<bucket>/<key>
```

**Get Deployment Status**

Retrieves the deployment status
```
dotnet codedeploy status --deployment-id <value>
```

# Limitations
- Currently only deploys using S3 Revision locations- GitHub not available

# Licence
[Apache-2.0](./LICENSE)