using Microsoft.Extensions.Localization;
using System.Diagnostics.CodeAnalysis;

namespace GeneralUnifiedTestSystemYard.Core;

public class MessageService
{
    private readonly IStringLocalizer _localizer = null!;

    public MessageService(IStringLocalizerFactory factory) =>
        _localizer = factory.Create(typeof(MessageService));

    [return: NotNullIfNotNull("_localizer")]
    public string? GetFormattedMessage(DateTime dateTime, double dinnerPrice)
    {
        LocalizedString localizedString = _localizer["DinnerPriceFormat", dateTime, dinnerPrice];
        return localizedString;
    }
}
