echo "Injecting firestore secrets..."
echo "Updating Google JSON"
echo $GOOGLE_SERVICES_JSON | base64 --decode > "$APPCENTER_SOURCE_DIRECTORY/Questonaut.Android/google-services.json"
echo "Finished injecting secrets."