CONFIG_JSON=$APPCENTER_SOURCE_DIRECTORY/<path>/config.json

if [ -e "$CONFIG_JSON" ]
then
    echo "Updating Config Json"
    echo "$CONFIG_J" > $CONFIG_JSON
    sed -i -e 's/\\"/'\"'/g' $CONFIG_JSON

    echo "File content:"
    cat $CONFIG_JSON
fi