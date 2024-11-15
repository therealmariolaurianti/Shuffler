using Ninject;
using ShufflerPro.Client.AudioEqualizer;

namespace ShufflerPro.Bootstrapper.Extensions;

public static class EqualizerBandExtensions
{
    public static EqualizerBandContainer EqualizerBandContainer { get; } = new();

    public static void BindEqualizer(this IKernel container)
    {
        container.Bind<EqualizerBandContainer>().ToConstant(EqualizerBandContainer).InSingletonScope();
        container.Bind<IEqualizerBandContainer>().ToMethod(_ => EqualizerBandContainer);
    }
}