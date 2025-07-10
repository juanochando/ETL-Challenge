#!/bin/bash
bash .devcontainer/sql/postCreateCommand.sh 'P@ssw0rd' './bin/Debug/' '.devcontainer/sql/'

# Install dotnet tools
export PATH=$PATH:/root/.dotnet/tools

dotnet tool install -g dotnet-ef
sudo dotnet workload update
sudo dotnet workload install aspire

# Trust dev certicficate and restore packages
cd ETLChallenge.UI
dotnet dev-certs https --trust; dotnet dev-certs https -ep "${HOME}/.aspnet/https/aspnetapp.pfx" -p "@SecurePwdGoesHere#"
dotnet restore
