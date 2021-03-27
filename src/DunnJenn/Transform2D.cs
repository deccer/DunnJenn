using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DunnJenn
{
    public class Transform2D
    {
        public Transform2D()
        {
            Position = Vector2.Zero;
            Rotation = 0;
            Scale = Vector2.One;
        }

        private Transform2D parent;

        private List<Transform2D> children = new List<Transform2D>();

        private Matrix absolute, invertAbsolute, local;

        private float localRotation, absoluteRotation;

        private Vector2 localScale, absoluteScale, localPosition, absolutePosition;

        private bool needsAbsoluteUpdate = true, needsLocalUpdate = true;

        /// <summary>
        /// Gets or sets the parent transform.
        /// </summary>
        /// <value>The parent.</value>
        public Transform2D Parent
        {
            get => parent;
            set
            {
                if(parent != value)
                {
                    if (parent != null)
                        parent.children.Remove(this);

                    parent = value;

                    if (parent != null)
                        parent.children.Add(this);

                    SetNeedsAbsoluteUpdate();
                }
            }
        }

        /// <summary>
        /// Gets all the children transform.
        /// </summary>
        /// <value>The children.</value>
        public IEnumerable<Transform2D> Children => children;

        /// <summary>
        /// Gets the absolute rotation.
        /// </summary>
        /// <value>The absolute rotation.</value>
        public float AbsoluteRotation => UpdateAbsoluteAndGet(ref absoluteRotation);

        /// <summary>
        /// Gets the absolute scale.
        /// </summary>
        /// <value>The absolute scale.</value>
        public Vector2 AbsoluteScale => UpdateAbsoluteAndGet(ref absoluteScale);

        /// <summary>
        /// Gets the absolute position.
        /// </summary>
        /// <value>The absolute position.</value>
        public Vector2 AbsolutePosition => UpdateAbsoluteAndGet(ref absolutePosition);

        /// <summary>
        /// Gets or sets the rotation (relative to the parent, absolute if no parent).
        /// </summary>
        /// <value>The rotation.</value>
        public float Rotation
        {
            get => localRotation;
            set
            {
                if (localRotation != value)
                {
                    localRotation = value;
                    SetNeedsLocalUpdate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the position (relative to the parent, absolute if no parent).
        /// </summary>
        /// <value>The position.</value>
        public Vector2 Position
        {
            get => localPosition;
            set
            {
                if(localPosition != value)
                {
                    localPosition = value;
                    SetNeedsLocalUpdate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the scale (relative to the parent, absolute if no parent).
        /// </summary>
        /// <value>The scale.</value>
        public Vector2 Scale
        {
            get => localScale;
            set
            {
                if (localScale != value)
                {
                    localScale = value;
                    SetNeedsLocalUpdate();
                }
            }
        }

        /// <summary>
        /// Gets the matrix representing the local transform.
        /// </summary>
        /// <value>The relative matrix.</value>
        public Matrix Local => UpdateLocalAndGet(ref absolute);

        /// <summary>
        /// Gets the matrix representing the absolute transform.
        /// </summary>
        /// <value>The absolute matrix.</value>
        public Matrix Absolute => UpdateAbsoluteAndGet(ref absolute);

        /// <summary>
        /// Gets the matrix representing the invert of the absolute transform.
        /// </summary>
        /// <value>The absolute matrix.</value>
        public Matrix InvertAbsolute => UpdateAbsoluteAndGet(ref invertAbsolute);

        public void ToLocalPosition(ref Vector2 absolute, out Vector2 local)
        {
            Vector2.Transform(ref absolute, ref invertAbsolute, out local);
        }

        public void ToAbsolutePosition(ref Vector2 local, out Vector2 absolute)
        {
            Vector2.Transform(ref local, ref this.absolute, out absolute);
        }

        public Vector2 ToLocalPosition(Vector2 absolute)
        {
            Vector2 result;
            ToLocalPosition(ref absolute, out result);
            return result;
        }

        public Vector2 ToAbsolutePosition(Vector2 local)
        {
            Vector2 result;
            ToAbsolutePosition(ref local, out result);
            return result;
        }

        private void SetNeedsLocalUpdate()
        {
            needsLocalUpdate = true;
            SetNeedsAbsoluteUpdate();
        }

        private void SetNeedsAbsoluteUpdate()
        {
            needsAbsoluteUpdate = true;

            foreach (var child in children)
            {
                child.SetNeedsAbsoluteUpdate();
            }
        }

        private void UpdateLocal()
        {
            var result = Matrix.CreateScale(Scale.X, Scale.Y, 1);
            result *= Matrix.CreateRotationZ(Rotation);
            result *= Matrix.CreateTranslation(Position.X, Position.Y, 0);
            local = result;

            needsLocalUpdate = false;
        }

        private void UpdateAbsolute()
        {
            if (Parent == null)
            {
                absolute = local;
                absoluteScale = localScale;
                absoluteRotation = localRotation;
                absolutePosition = localPosition;
            }
            else
            {
                var parentAbsolute = Parent.Absolute;
                Matrix.Multiply(ref local, ref parentAbsolute, out absolute);
                absoluteScale = Parent.AbsoluteScale * Scale;
                absoluteRotation = Parent.AbsoluteRotation + Rotation;
                absolutePosition = Vector2.Zero;
                ToAbsolutePosition(ref absolutePosition, out absolutePosition);
            }

            Matrix.Invert(ref absolute, out invertAbsolute);

            needsAbsoluteUpdate = false;
        }

        private T UpdateLocalAndGet<T>(ref T field)
        {
            if (needsLocalUpdate)
            {
                UpdateLocal();
            }

            return field;
        }

        private T UpdateAbsoluteAndGet<T>(ref T field)
        {
            if (needsLocalUpdate)
            {
                UpdateLocal();
            }

            if (needsAbsoluteUpdate)
            {
                UpdateAbsolute();
            }

            return field;
        }
    }
}