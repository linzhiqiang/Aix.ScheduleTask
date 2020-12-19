set -ex

cd $(dirname $0)/../

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

mkdir -p $artifactsFolder


dotnet build ./src/Aix.ScheduleTask/Aix.ScheduleTask.csproj -c Release

dotnet pack ./src/Aix.ScheduleTask/Aix.ScheduleTask.csproj -c Release -o $artifactsFolder

dotnet nuget push ./$artifactsFolder/Aix.ScheduleTask.*.nupkg -k $PRIVATE_NUGET_KEY -s http://192.168.102.34:8081/repository/nuget-hosted
