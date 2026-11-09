using Microsoft.eShopWeb.ApplicationCore.Interfaces;
namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class UriComposer : IUriComposer
{
    private readonly CatalogSettings _catalogSettings;

    public UriComposer(CatalogSettings catalogSettings)
        => _catalogSettings = catalogSettings;

    public string ComposePicUri(string uriTemplate)
    {
        var imageBaseUrl = _catalogSettings.ImageBaseUrl;

        if (!string.IsNullOrEmpty(imageBaseUrl))
        {
            return uriTemplate.Replace(
                "http://catalogbaseurltobereplaced",
                imageBaseUrl.TrimEnd('/'));
        }

        return uriTemplate.Replace(
            "http://catalogbaseurltobereplaced",
            _catalogSettings.CatalogBaseUrl ?? string.Empty);
    }
}
