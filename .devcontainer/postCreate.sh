#!/bin/bash

# Install dotnet tools
export PATH=$PATH:/root/.dotnet/tools

dotnet tool install -g dotnet-ef
sudo dotnet workload update
sudo dotnet workload install aspire

# Trust dev certicficate
dotnet dev-certs https --trust; dotnet dev-certs https -ep "${HOME}/.aspnet/https/aspnetapp.pfx" -p "@SecurePwdGoesHere#"

# Aspire specific
dotnet new install Aspire.ProjectTemplates --force

# Restore packages
dotnet restore
