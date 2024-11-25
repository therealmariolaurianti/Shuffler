using Ninject;
using ShufflerPro.Client;

namespace ShufflerPro.Bootstrapper.Extensions;

public static class AccessTokensExtensions
{
    public static AccessKeysContainer AccessKeysContainer { get; } = new();

    public static void BindAccessTokens(this IKernel container)
    {
        container.Bind<AccessKeysContainer>().ToConstant(AccessKeysContainer).InSingletonScope();
    }
}