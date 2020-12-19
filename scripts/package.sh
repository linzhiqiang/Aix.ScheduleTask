set -ex

cd $(dirname $0)/../

artifactsFolder="./artifacts"

if [ -d $artifactsFolder ]; then
  rm -R $artifactsFolder
fi

mkdir -p $artifactsFolder

dotnet restore ./Aix.ScheduleTask.sln
dotnet build ./Aix.ScheduleTask.sln -c Release


dotnet pack ./src/Aix.ScheduleTask/Aix.ScheduleTask.csproj -c Release -o $artifactsFolder
