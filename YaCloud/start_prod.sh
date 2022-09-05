#!/bin/bash

IAM_TOKEN=`curl -H Metadata-Flavor:Google http://169.254.169.254/computeMetadata/v1/instance/service-accounts/default/token | jq -r .access_token`;
secrets=`curl -X GET -H "Authorization: Bearer ${IAM_TOKEN}" https://payload.lockbox.api.cloud.yandex.net/lockbox/v1/secrets/e6qm27rggpubvhiq0oih/payload`;
secrets_env=`echo "$secrets" | jq -r $'.entries[] | ("export " + .key + "=\'" + (.textValue // (.binaryValue | @base64d))) + "\'"'`;
eval $(echo "$secrets_env");

env_docker=`echo "$secrets" | jq -r '"-env " + .entries[].key + " "'`;
env > .env;
sudo DOCKER_BUILDKIT=1 docker build -f Docker/build.dockerfile -t shreks ./Source && \
sudo docker run --env-file .env -it shreks;
