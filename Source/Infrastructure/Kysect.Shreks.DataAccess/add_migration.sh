#!/bin/bash

dotnet ef migrations add $1 --context ShreksDatabaseContext --output-dir Migrations -s ../../Presentation/Kysect.Shreks.WebApi