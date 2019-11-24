
# AWS CodeDeploy .NET Core Global Tool

Deploy .NET Core applications using CodeDeploy and S3 Revision Locations

## Installation
1. Install aws-cli and [configure credentials](https://docs.aws.amazon.com/cli/latest/userguide/cli-chap-configure.html). Alternatively setup AWS Credentials from [Environment Variables](https://docs.aws.amazon.com/sdk-for-javascript/v2/developer-guide/loading-node-credentials-environment.html)
2. Install the extension using the dotnet tool install command 
```
dotnet tool install -g Tool.AWS.CodeDeploy
```

## Features
Create CodeDeploy Deployments:
```
dotnet codedeploy --application <value> --deploymentGroup -- <value> --s3RevisionLocation s3://<bucket>/<key>
```

## Licence
[Apache 2.0](./LICENSE)