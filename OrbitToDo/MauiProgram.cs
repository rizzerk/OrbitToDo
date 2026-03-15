using Microsoft.Extensions.Logging;

namespace OrbitToDo;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Orbitron-Regular.ttf", "Orbitron");
                fonts.AddFont("Orbitron-Bold.ttf", "OrbitronBold");
                fonts.AddFont("Exo2-Regular.ttf", "Exo2");
                fonts.AddFont("Exo2-SemiBold.ttf", "Exo2SemiBold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
