using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DunnJenn
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var serviceProvider = BuildServiceProvider();

            using var game = serviceProvider.GetService<MainGame>();
            game!.Run();
        }

        private static IServiceProvider BuildServiceProvider()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Console()
                .WriteTo.File("client.log")
                .CreateLogger();

            var chunkCreatorOptions = new ChunkCreatorOptions();
            configuration.GetSection(nameof(ChunkCreator)).Bind(chunkCreatorOptions);
            var chunkProviderOptions = new ChunkProviderOptions();
            configuration.GetSection(nameof(ChunkProvider)).Bind(chunkProviderOptions);
            
            var services = new ServiceCollection();
            services.AddSingleton(Log.Logger);
            services.AddSingleton(chunkCreatorOptions);
            services.AddSingleton(chunkProviderOptions);
            services.AddSingleton<IChunkCreator, ChunkCreator>();
            services.AddSingleton<IChunkLoader, ChunkLoader>();
            services.AddSingleton<IChunkSaver, ChunkSaver>();
            services.AddSingleton<IChunkProvider, ChunkProvider>();
            services.AddSingleton<INoiseGenerator, FastNoiseNoiseGenerator>();
            services.AddSingleton<IMapProvider, MapProvider>();
            services.AddSingleton<MainGame>();
            
            return services.BuildServiceProvider();
        }
    }
}