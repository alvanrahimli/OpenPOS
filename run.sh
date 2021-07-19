PROJS=$(find . -maxdepth 4 -type f -name *.csproj)
PROJ_PIDS=()

for proj in $PROJS; do
    echo '   > Running $proj'
    dotnet run --project $proj > '/logs/test'  exit 1 
    PROJ_PIDS+=($!)
done

# if echo $* | grep -e "--stop" -q; then
#     for id in $PROJ_PIDS; do
#         echo 'Stopping project $id'
#         kill -9 $id
#     done
# fi

echo $PROJ_PIDS
