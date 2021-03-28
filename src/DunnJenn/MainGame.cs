using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Serilog;
using SpriteFontPlus;
using Num = System.Numerics;

namespace DunnJenn
{
    public class MainGame : Game
    {
        private readonly IMapProvider _mapProvider;
        private readonly IChunkProvider _chunkProvider;
        private readonly INoiseGenerator _noiseGenerator;
        private readonly ILogger _logger;
        private bool _isWindowFocused;
        private SpriteBatch _spriteBatch;
        private SpriteFont _spriteFont;
        private Camera _camera;

        private Texture2D _textureAtlas;
        private readonly World _world;
        private Guid _currentMapId;

        private KeyboardState _oldKeyboardState;
        private KeyboardState _newKeyboardState;
        private readonly IList<Chunk> _visibleChunks;
        private Hero _hero;
        private StringBuilder _heroText;
        private Matrix _heroTransform;

        private ImGuiRenderer _imGuiRenderer;
        private bool _isUiVisible = true;

        public MainGame(
            ILogger logger,
            IMapProvider mapProvider,
            IChunkProvider chunkProvider,
            INoiseGenerator noiseGenerator)
        {
            _logger = logger.ForContext<MainGame>();
            _mapProvider = mapProvider;
            _chunkProvider = chunkProvider;
            _noiseGenerator = noiseGenerator;

            FNALoggerEXT.LogError = message => _logger.Error("FNA: {@Message}", message);
            FNALoggerEXT.LogInfo = message => _logger.Information("FNA: {@Message}", message);
            FNALoggerEXT.LogWarn = message => _logger.Warning("FNA: {@Message}", message);

            var graphics = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef,
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
                PreferMultiSampling = true,
                SynchronizeWithVerticalRetrace = false
            };

            Window.Title = "DunnJenn";
            Activated += (_, _) => { _isWindowFocused = true; };
            Deactivated += (_, _) =>
            {
                _isWindowFocused = false;
                Window.Title = $"DunnJenn";
            };

            IsMouseVisible = true;
            IsFixedTimeStep = false;
            Window.AllowUserResizing = true;

            Content.RootDirectory = nameof(Content);

            _visibleChunks = new List<Chunk>(16);
            _world = new World(logger, _mapProvider);
            _currentMapId = Guid.Empty;
            _hero = new Hero();
            _hero.Position = new Vector2(4, 4);
            _heroText = new StringBuilder();
            _heroTransform = Matrix.Identity;
        }

        protected override void Draw(GameTime gameTime)
        {
            if (!_isWindowFocused)
            {
                return;
            }

            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap,
                DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, _camera.Transform);
            foreach (var chunk in _visibleChunks)
            {
                var chunkPositionX = chunk.Position.X * Chunk.ChunkSize;
                var chunkPositionY = chunk.Position.Y * Chunk.ChunkSize;
                for (var y = 0; y < Chunk.ChunkSize; y++)
                {
                    for (var x = 0; x < Chunk.ChunkSize; x++)
                    {
                        var tile = chunk.Tiles[y * Chunk.ChunkSize + x];
                        var tileScreenX = chunkPositionX * Tile.TileSize + (x - 1) * Tile.TileSize;
                        var tileScreenY = chunkPositionY * Tile.TileSize + (y - 1) * Tile.TileSize;

                        if (_camera.VisibleArea.Contains(tileScreenX, tileScreenY))
                        {
                            _spriteBatch.Draw(_textureAtlas, new Rectangle(tileScreenX, tileScreenY, Tile.TileSize, Tile.TileSize), GetRectangleForTileId(tile.Id), Color.White);
                        }
                        else
                        {
                            //_spriteBatch.Draw(_textureAtlas, new Rectangle(tileScreenX, tileScreenY, Tile.TileSize, Tile.TileSize), GetRectangleForTileId(tile.Id), Color.Red);
                        }
                    }
                }
            }
            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, _camera.Transform);
            _spriteBatch.Draw(_textureAtlas, _camera.Position * Tile.TileSize,
                GetRectangleForTileId(5), Color.White);
            _spriteBatch.End();

            /*
            _heroText.Clear();
            _heroText.AppendLine($"CamPos   {_camera.Position.ToString()}");
            _heroText.AppendLine($"CamPos2D {(_camera.Position * Tile.TileSize).ToString()}");
            var chunkPosition = new Point(
                (int)Math.Floor(_camera.Position.X / Chunk.ChunkSize),
                (int)Math.Floor(_camera.Position.Y / Chunk.ChunkSize));
            _heroText.AppendLine($"ChunkPos {chunkPosition.ToString()}");
            _heroText.AppendLine($"Vis {_camera.VisibleArea.ToString()}");

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            _spriteBatch.DrawString(_spriteFont, _heroText, new Vector2(8, 8), Color.LawnGreen);
            _spriteBatch.End();
*/

            if (_isUiVisible)
            {
                DrawUserInterface(gameTime);
            }

            base.Draw(gameTime);
        }

        private void DrawUserInterface(GameTime gameTime)
        {
            _imGuiRenderer.BeginLayout(gameTime);
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("Map"))
                    {
                        if (ImGui.MenuItem("Reset"))
                        {
                            _chunkProvider.Reset(ResetOptions.ResetCache | ResetOptions.ResetFileSystem);
                        }

                        ImGui.EndMenu();
                    }

                    ImGui.EndMenuBar();
                }
                ImGui.EndMainMenuBar();
            }

            if (ImGui.Begin("Noise Options"))
            {
                var frequency = _noiseGenerator.Frequency;
                var octaves = _noiseGenerator.Octaves;
                var lacunarity = _noiseGenerator.Lacunarity;
                var gain = _noiseGenerator.Gain;
                var cellularJitter = _noiseGenerator.CellularJitter;

                if (ImGui.Button("CDF-Euclidian"))
                {
                    _noiseGenerator.CellularDistanceFunction = FastNoiseLite.CellularDistanceFunction.Euclidean;
                    _chunkProvider.Reset(ResetOptions.ResetCache | ResetOptions.ResetFileSystem);
                }

                ImGui.SameLine();
                if (ImGui.Button("CDF-EuclidianSq"))
                {
                    _noiseGenerator.CellularDistanceFunction = FastNoiseLite.CellularDistanceFunction.EuclideanSq;
                    _chunkProvider.Reset(ResetOptions.ResetCache | ResetOptions.ResetFileSystem);
                }

                ImGui.SameLine();
                if (ImGui.Button("CDF-Hybrid"))
                {
                    _noiseGenerator.CellularDistanceFunction = FastNoiseLite.CellularDistanceFunction.Hybrid;
                    _chunkProvider.Reset(ResetOptions.ResetCache | ResetOptions.ResetFileSystem);
                }

                ImGui.SameLine();
                if (ImGui.Button("CDF-Manhattan"))
                {
                    _noiseGenerator.CellularDistanceFunction = FastNoiseLite.CellularDistanceFunction.Manhattan;
                    _chunkProvider.Reset(ResetOptions.ResetCache | ResetOptions.ResetFileSystem);
                }

                if (ImGui.Button("FT-None"))
                {
                    _noiseGenerator.FractalType = FastNoiseLite.FractalType.None;
                    _chunkProvider.Reset(ResetOptions.ResetCache | ResetOptions.ResetFileSystem);
                }

                ImGui.SameLine();
                if (ImGui.Button("FT-Ridged"))
                {
                    _noiseGenerator.FractalType = FastNoiseLite.FractalType.Ridged;
                    _chunkProvider.Reset(ResetOptions.ResetCache | ResetOptions.ResetFileSystem);
                }

                ImGui.SameLine();
                if (ImGui.Button("FT-FBm"))
                {
                    _noiseGenerator.FractalType = FastNoiseLite.FractalType.FBm;
                    _chunkProvider.Reset(ResetOptions.ResetCache | ResetOptions.ResetFileSystem);
                }

                ImGui.SameLine();
                if (ImGui.Button("FT-PingPong"))
                {
                    _noiseGenerator.FractalType = FastNoiseLite.FractalType.PingPong;
                    _chunkProvider.Reset(ResetOptions.ResetCache | ResetOptions.ResetFileSystem);
                }

                ImGui.SameLine();
                if (ImGui.Button("FT-DomainWarpIndependent"))
                {
                    _noiseGenerator.FractalType = FastNoiseLite.FractalType.DomainWarpIndependent;
                    _chunkProvider.Reset(ResetOptions.ResetCache | ResetOptions.ResetFileSystem);
                }

                ImGui.SameLine();
                if (ImGui.Button("FT-DomainWarpProgressive"))
                {
                    _noiseGenerator.FractalType = FastNoiseLite.FractalType.DomainWarpProgressive;
                    _chunkProvider.Reset(ResetOptions.ResetCache | ResetOptions.ResetFileSystem);
                }

                if (ImGui.Button("NT-Cellular"))
                {
                    _noiseGenerator.NoiseType = FastNoiseLite.NoiseType.Cellular;
                    _chunkProvider.Reset(ResetOptions.ResetCache | ResetOptions.ResetFileSystem);
                }

                ImGui.SameLine();
                if (ImGui.Button("NT-Perlin"))
                {
                    _noiseGenerator.NoiseType = FastNoiseLite.NoiseType.Perlin;
                    _chunkProvider.Reset(ResetOptions.ResetCache | ResetOptions.ResetFileSystem);
                }

                ImGui.SameLine();
                if (ImGui.Button("NT-Value"))
                {
                    _noiseGenerator.NoiseType = FastNoiseLite.NoiseType.Value;
                    _chunkProvider.Reset(ResetOptions.ResetCache | ResetOptions.ResetFileSystem);
                }

                ImGui.SameLine();
                if (ImGui.Button("NT-OpenSimplex"))
                {
                    _noiseGenerator.NoiseType = FastNoiseLite.NoiseType.OpenSimplex2;
                    _chunkProvider.Reset(ResetOptions.ResetCache | ResetOptions.ResetFileSystem);
                }

                ImGui.SameLine();
                if (ImGui.Button("NT-ValueCubic"))
                {
                    _noiseGenerator.NoiseType = FastNoiseLite.NoiseType.ValueCubic;
                    _chunkProvider.Reset(ResetOptions.ResetCache | ResetOptions.ResetFileSystem);
                }

                ImGui.SameLine();
                if (ImGui.Button("NT-OpenSimplex2S"))
                {
                    _noiseGenerator.NoiseType = FastNoiseLite.NoiseType.OpenSimplex2S;
                    _chunkProvider.Reset(ResetOptions.ResetCache | ResetOptions.ResetFileSystem);
                }

                if (ImGui.SliderFloat("Frequency", ref frequency, 0.001f, 1.0f))
                {
                    _noiseGenerator.Frequency = frequency;
                }

                if (ImGui.SliderInt("Octaves", ref octaves, 1, 10))
                {
                    _noiseGenerator.Octaves = octaves;
                }

                if (ImGui.SliderFloat("Lacunarity", ref lacunarity, 0.001f, 2.0f))
                {
                    _noiseGenerator.Lacunarity = lacunarity;
                }

                if (ImGui.SliderFloat("Gain", ref gain, 0.01f, 3.0f))
                {
                    _noiseGenerator.Gain = gain;
                }

                if (ImGui.SliderFloat("Cellular Jitter", ref cellularJitter, 0.01f, 2.0f))
                {
                    _noiseGenerator.CellularJitter = cellularJitter;
                }

                var camPos = new Num.Vector2(_camera.Position.X, _camera.Position.Y);
                if (ImGui.InputFloat2("Camera Position", ref camPos))
                {
                }

                var camChunk = new Num.Vector2((int) Math.Floor(_camera.Position.X / Chunk.ChunkSize),
                    (int) Math.Floor(_camera.Position.Y / Chunk.ChunkSize));
                if (ImGui.InputFloat2("Chunk Position", ref camChunk))
                {
                }

                ImGui.End();
            }

            _imGuiRenderer.EndLayout();
        }

        private Rectangle GetRectangleForTileId(int tileId)
        {
            var tilesPerRow = _textureAtlas.Width / 32;

            var tileY = tileId / tilesPerRow;
            var tileX = tileId % tilesPerRow;

            return new Rectangle(tileX * Tile.TileSize, tileY * Tile.TileSize, Tile.TileSize, Tile.TileSize);
        }

        protected override void Initialize()
        {
            base.Initialize();

            _camera = new Camera(GraphicsDevice.Viewport);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _imGuiRenderer = new ImGuiRenderer(this);
            _imGuiRenderer.RebuildFontAtlas();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _textureAtlas = Content.Load<Texture2D>("Textures/LandAtlas");

            using var fontStream = File.OpenRead("Content/Fonts/Ruda-Regular.ttf");
            var fontBakerResult = TtfFontBaker.Bake(fontStream, 28, 1024, 1024, new[]
            {
                CharacterRange.BasicLatin
            });

            _spriteFont = fontBakerResult.CreateSpriteFont(GraphicsDevice);
            _oldKeyboardState = Keyboard.GetState();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            _imGuiRenderer.Dispose();
            _spriteBatch.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _imGuiRenderer.UpdateInput();
            _camera.UpdateCamera(GraphicsDevice.Viewport);

            _newKeyboardState = Keyboard.GetState();
            if (_newKeyboardState.IsKeyDown(Keys.F1) && _oldKeyboardState.IsKeyUp(Keys.F1))
            {
                _isUiVisible = !_isUiVisible;
            }

            _oldKeyboardState = _newKeyboardState;

            var visibleScreenArea = _camera.VisibleArea;

            var visibleChunkAreaLeft = visibleScreenArea.Left / Chunk.ChunkSize / Tile.TileSize - 1;
            var visibleChunkAreaTop = visibleScreenArea.Top / Chunk.ChunkSize / Tile.TileSize - 1;
            var visibleChunkAreaRight = visibleChunkAreaLeft + visibleScreenArea.Width / Chunk.ChunkSize / Tile.TileSize + 1;
            var visibleChunkAreaBottom = visibleChunkAreaTop + visibleScreenArea.Height / Chunk.ChunkSize / Tile.TileSize + 1;

            _visibleChunks.Clear();
            for (var y = visibleChunkAreaTop; y <= visibleChunkAreaBottom; y++)
            {
                for (var x = visibleChunkAreaLeft; x <= visibleChunkAreaRight; x++)
                {
                    _visibleChunks.Add(_world.GetChunk(_currentMapId, new Point(x, y)));
                }
            }
        }
    }
}
