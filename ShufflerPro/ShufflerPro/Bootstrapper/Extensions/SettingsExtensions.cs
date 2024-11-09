using Ninject;
using ShufflerPro.Client;
using ShufflerPro.Client.Interfaces;

namespace ShufflerPro.Bootstrapper.Extensions;

public static class SettingsExtensions
{
    public static SettingsContainer SettingsContainer { get; } = new();

    public static void BindSettings(this IKernel container)
    {
        container.Bind<SettingsContainer>().ToConstant(SettingsContainer).InSingletonScope();
        container.Bind<ISettings>().ToMethod(_ => SettingsContainer.Settings);
    }
}