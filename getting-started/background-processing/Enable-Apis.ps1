$servicesToEnable = @"
translate.googleapis.com
pubsub.googleapis.com
firestore.googleapis.com
cloudrun.googleapis.com
"@
gcloud services enable $servicesToEnable
