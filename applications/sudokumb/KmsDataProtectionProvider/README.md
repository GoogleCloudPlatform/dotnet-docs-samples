### <a name="kms"><img src="http://cloud.google.com/_static/images/cloud/products/logos/svg/kms.svg" width=64> Sudokumb secures forms and cookies with [Key Management Service](https://cloud.google.com/kms/).</a>

ASP.NET core uses a [`IDataProtectionProvider`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.dataprotection.idataprotectionprovider?view=aspnetcore-2.0)
for cryptographic operations to validate forms authentication and to protect view state.
By default, ASP.NET core uses an `IDataProtectionProvider` based on a [machine key](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/compatibility/replacing-machinekey?view=aspnetcore-2.1)  However, if the ASP.NET application is deployed to a server farm,
then each machine gets its own machine key.  If a form is provided by one webserver and
accepted by another, then the user will experience an error.

Google [Key Management Service](https://cloud.google.com/kms/) provides a more secure
way to protect data, because keys rotate automatically.

[KmsDataProtectionProvider.cs](./KmsDataProtectionProvider.cs)

TODO: Explain more and add a drawing.
