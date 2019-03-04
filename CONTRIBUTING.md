# How to become a contributor and submit your own code

## Contributor License Agreements

We'd love to accept your patches! Before we can take them, we
have to jump a couple of legal hurdles.

Please fill out either the individual or corporate Contributor License Agreement
(CLA).

  * If you are an individual writing original source code and you're sure you
    own the intellectual property, then you'll need to sign an [individual
    CLA](https://developers.google.com/open-source/cla/individual).
  * If you work for a company that wants to allow you to contribute your work,
    then you'll need to sign a [corporate
    CLA](https://developers.google.com/open-source/cla/corporate).

Follow either of the two links above to access the appropriate CLA and
instructions for how to sign and return it. Once we receive it, we'll be able to
accept your pull requests.

## Setting Up An Environment
For instructions regarding development environment setup, please visit [the documentation](https://cloud.google.com/dotnet/docs/setup).

## Contributing A Patch

1. Submit an issue describing your proposed change to the repo in question.
1. The repo owner will respond to your issue promptly.
1. If your proposed change is accepted, and you haven't already done so, sign a
   Contributor License Agreement (see details above).
1. Fork the desired repo, develop and test your code changes.
1. Ensure that your code adheres to the existing style in the sample to which
   you are contributing.
1. Ensure that your code has an appropriate set of unit tests which all pass.
1. Submit a pull request.

## Style

Samples in this repository follow the [framework coding
style](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md).
This is enforced using [CodeFormatter](https://github.com/dotnet/codeformatter).

### Starting a new sample
When starting a new sample, match the file and directory layout in the
[the translate sample](translate/api).  That sample is the canonical example
of how a new sample should be structured.