# start with a powershell image.
FROM microsoft/powershell

# install dotnet core sdk.
# steps from https://www.microsoft.com/net/core#linuxubuntu
RUN curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg
RUN mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg
RUN sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-ubuntu-xenial-prod xenial main" > /etc/apt/sources.list.d/dotnetdev.list'
RUN apt-get -y update
RUN apt-get -y install dotnet-sdk-2.0.0