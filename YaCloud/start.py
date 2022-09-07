#!/usr/bin/env python3

import argparse
import os
from base64 import b64decode

import requests
from python_on_whales import docker


def get_service_token():
    resp = requests.get(
        url='http://169.254.169.254/computeMetadata/v1/instance/service-accounts/default/token',
        headers={'Metadata-Flavor': 'Google'}
    ).json()
    return resp['access_token']


class Secret:
    def __init__(self, key, value):
        self.key = key
        self.value = value

    def __repr__(self):
        return str(self)

    def __str__(self):
        return self.key + ": " + self.value


def get_secrets(token, secret, version=None):
    resp = requests.get(
        url='https://payload.lockbox.api.cloud.yandex.net/lockbox/v1/secrets/%s/payload' % secret,
        headers={'Authorization': 'Bearer %s' % token}
    ).json()

    def get_value(e):
        return e['textValue'] if e.get('textValue') is not None else b64decode(e['binaryValue']).decode("utf-8")

    secrets = [Secret(e['key'], get_value(e)) for e in resp['entries']]
    return secrets


def start_container(name, secret, port, asp_env=None):
    token = get_service_token()
    secrets = get_secrets(token, secret)
    docker.build("./Source", file='./Docker/build.dockerfile', tags=name)
    try:
        docker.stop(name)
        docker.remove(name)
        print('stopped and removed previous container')
    except:
        pass
    envs = {s.key: s.value for s in secrets}
    envs['ASPNETCORE_ENVIRONMENT'] = asp_env if asp_env is not None else 'Production'
    docker.run(name, envs=envs, publish=[(port, 5069)], tty=True, detach=True, name=name, restart='always',
               volumes=[(os.environ.get('HOME') + '/Logs/%s' % name, '/app/Serilogs')])


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument("-n", dest="name")
    parser.add_argument("-e", dest="asp_env")
    parser.add_argument("-s", dest="secret")
    parser.add_argument("-p", dest="port")
    args = parser.parse_args()
    start_container(args.name, args.secret, args.port, args.asp_env)
