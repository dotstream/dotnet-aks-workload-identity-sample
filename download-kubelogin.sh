#!/bin/sh

mkdir /kubelogin
newplatformversion="`echo $1 | sed 's/\//-/g'`"
foldername="`echo $1 | sed 's/\//_/g'`"

echo "$newplatformversion used"
wget "https://github.com/Azure/kubelogin/releases/download/$2/kubelogin-$newplatformversion.zip" --output-document="/kubelogin/kubelogin-$newplatformversion.zip"
unzip /kubelogin/kubelogin-$newplatformversion.zip -d /kubelogin
cp /kubelogin/bin/$foldername/kubelogin /kubelogin/