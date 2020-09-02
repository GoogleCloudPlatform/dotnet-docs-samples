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
  sed -i -e "s/Include=\"Google.Events\" Version=\".*\"/Include=\"Google.Events\" Version=\"$1\"/g" $proj
  sed -i -e "s/Include=\"Google.Events.Protobuf\" Version=\".*\"/Include=\"Google.Events.Protobuf\" Version=\"$1\"/g" $proj
  sed -i -e "s/Include=\"Google.Events.SystemTextJson\" Version=\".*\"/Include=\"Google.Events.SystemTextJson\" Version=\"$1\"/g" $proj
done
