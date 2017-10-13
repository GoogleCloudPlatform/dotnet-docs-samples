FROM gcr.io/cloud-devrel-kokoro-resources/dotnet:latest

ARG KOKORO_GFILE_DIR

# Copy secrets from kokoro gfile.
COPY $KOKORO_GFILE_DIR /kokoro_gfile_dir
ENV KOKORO_GFILE_DIR /kokoro_gfile_dir

# Copy in this repo.
RUN git clone https://github.com/GoogleCloudPlatform/dotnet-docs-samples.git
RUN cd dotnet-docs-samples; git checkout -t origin/kokoro

WORKDIR /dotnet-docs-samples/.kokoro/
