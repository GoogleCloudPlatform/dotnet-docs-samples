$configListText = gcloud spanner instance-configs list
$configList = $configListText -replace "\s+", "," | ConvertFrom-Csv
$usCentral = $configList | Where-Object { $_.NAME -like "*us-central*"} | Select -ExpandProperty Name | Select -First 1
gcloud spanner instances create my-instance --config=$usCentral --description="IntegrationTest" --nodes=1
gcloud spanner databases create my-database --instance=my-instance