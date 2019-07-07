echo "Injecting firestore secrets..."
echo "Updating android secret"
echo $ANDROID_SECRET > "$APPCENTER_SOURCE_DIRECTORY/Questonaut.Android/appcenter-config.json"
echo "Finished injecting secrets."