echo "Injecting secrets..."
echo "Updating Google plist"
echo $GOOGLE_SERVICES_PLIST | base64 --decode > "$APPCENTER_SOURCE_DIRECTORY/Questonaut.iOS/GoogleService-Info.plist"
echo "Finished injecting secrets."