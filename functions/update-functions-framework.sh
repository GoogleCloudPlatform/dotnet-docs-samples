#!/bin/sh

if [[ $# -ne 1 ]]
then
  script=$(basename -- "$0")
  echo "Usage: $script version"
  echo "e.g. $script 1.2.0"
  exit 1
fi

for proj in $(find . -name '*proj')
do
  sed -i -e "s/Include=\"Google.Cloud.Functions.Framework\" Version=\".*\"/Include=\"Google.Cloud.Functions.Framework\" Version=\"$1\"/g" $proj
  sed -i -e "s/Include=\"Google.Cloud.Functions.Invoker\" Version=\".*\"/Include=\"Google.Cloud.Functions.Invoker\" Version=\"$1\"/g" $proj
  sed -i -e "s/Include=\"Google.Cloud.Functions.Invoker.Testing\" Version=\".*\"/Include=\"Google.Cloud.Functions.Invoker.Testing\" Version=\"$1\"/g" $proj
done
