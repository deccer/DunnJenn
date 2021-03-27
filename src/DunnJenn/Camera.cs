using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DunnJenn
{
    public class Camera
    {
        public float Zoom { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle Bounds { get; protected set; }
        public Rectangle VisibleArea { get; protected set; }
        public Matrix Transform { get; protected set; }

        private float _currentMouseWheelValue;
        private float _previousMouseWheelValue;
        private float _zoom;
        private float _previousZoom;
        private KeyboardState _oldKeyboardState;
        private KeyboardState _newKeyboardState;

        public Camera(Viewport viewport)
        {
            Bounds = viewport.Bounds;
            Zoom = 1f;
            Position = Vector2.Zero;
        }

        private void UpdateVisibleArea()
        {
            var inverseViewMatrix = Matrix.Invert(Transform);

            var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
            var tr = Vector2.Transform(new Vector2(Bounds.X, 0), inverseViewMatrix);
            var bl = Vector2.Transform(new Vector2(0, Bounds.Y), inverseViewMatrix);
            var br = Vector2.Transform(new Vector2(Bounds.Width, Bounds.Height), inverseViewMatrix);

            var min = new Vector2(
                MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
            var max = new Vector2(
                MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
            VisibleArea = new Rectangle((int) min.X, (int) min.Y, (int) (max.X - min.X), (int) (max.Y - min.Y));
        }

        private void UpdateMatrix()
        {
            Transform = Matrix.CreateTranslation(new Vector3(-Position.X * Tile.TileSize, -Position.Y * Tile.TileSize, 0)) *
                        Matrix.CreateScale(Zoom) *
                        Matrix.CreateTranslation(new Vector3(Bounds.Width * 0.5f, Bounds.Height * 0.5f, 0));
            UpdateVisibleArea();
        }

        public void MoveCamera(Vector2 movePosition)
        {
            var newPosition = Position + movePosition;
            Position = newPosition;
        }

        public void AdjustZoom(float zoomAmount)
        {
            Zoom += zoomAmount;
            if (Zoom < .1f)
            {
                Zoom = .1f;
            }

            if (Zoom > 5f)
            {
                Zoom = 5f;
            }
        }

        public void UpdateCamera(Viewport bounds)
        {
            Bounds = bounds.Bounds;
            UpdateMatrix();

            var cameraMovement = Vector2.Zero;
            var moveSpeed = 10;

            _newKeyboardState = Keyboard.GetState();
            if (_newKeyboardState.IsKeyDown(Keys.W) && _oldKeyboardState.IsKeyUp(Keys.W) || _newKeyboardState.IsKeyDown(Keys.Up))
            {
                cameraMovement.Y = -moveSpeed;
            }

            if (_newKeyboardState.IsKeyDown(Keys.S) && _oldKeyboardState.IsKeyUp(Keys.S) || _newKeyboardState.IsKeyDown(Keys.Down))
            {
                cameraMovement.Y = moveSpeed;
            }

            if (_newKeyboardState.IsKeyDown(Keys.A) && _oldKeyboardState.IsKeyUp(Keys.A) || _newKeyboardState.IsKeyDown(Keys.Left))
            {
                cameraMovement.X = -moveSpeed;
            }

            if (_newKeyboardState.IsKeyDown(Keys.D) && _oldKeyboardState.IsKeyUp(Keys.D) || _newKeyboardState.IsKeyDown(Keys.Right))
            {
                cameraMovement.X = moveSpeed;
            }

            _oldKeyboardState = _newKeyboardState;
            _previousMouseWheelValue = _currentMouseWheelValue;
            _currentMouseWheelValue = Mouse.GetState().ScrollWheelValue;

            if (_currentMouseWheelValue > _previousMouseWheelValue)
            {
                AdjustZoom(.05f);
            }

            if (_currentMouseWheelValue < _previousMouseWheelValue)
            {
                AdjustZoom(-.05f);
            }

            _previousZoom = _zoom;
            _zoom = Zoom;

            MoveCamera(cameraMovement);
        }
    }
}