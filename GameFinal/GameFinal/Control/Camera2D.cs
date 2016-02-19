using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameFinal
{
    class Camera2D
    {
        #region Variables
        protected float _zoom; // Camera Zoom
        public Matrix _transform; // Matrix Transform
        public Vector2 _pos; // Camera Position
        protected float _rotation; // Camera Rotation
        private Vector2 ViewPortSize;
        Vector2 canvasSize;
        Vector2 lockedPos = new Vector2(0, 0);
        bool xLocked = false;
        bool yLocked = false;
        //MouseState prevMouseState;
        #endregion

        public Camera2D(Vector2 ViewPortSize, Vector2 canvasSize, float zoom)
        {
            _zoom = zoom;
            _rotation = 0.0f;
            _pos = Vector2.Zero;
            this.ViewPortSize = ViewPortSize;
            this.canvasSize = canvasSize;
            if (canvasSize.X < (ViewPortSize.X / _zoom))
            {
                xLocked = true;
                _pos.X = (ViewPortSize.X / 2) - ((ViewPortSize.X - canvasSize.X) / 2);
                lockedPos.X = _pos.X;
            }
            if (canvasSize.Y < (ViewPortSize.Y / _zoom))
            {
                yLocked = true;
                _pos.Y = (ViewPortSize.Y / 2) - ((ViewPortSize.Y - canvasSize.Y) / 2);
                lockedPos.Y = _pos.Y;
            }
            //prevMouseState = Mouse.GetState();
        }

        public Vector2 TLCorner
        {
            get { return new Vector2(_pos.X - (ViewPortSize.X / (2 * _zoom)), _pos.Y - (ViewPortSize.Y / (2 * _zoom))); }
            set { _pos = new Vector2(value.X + (ViewPortSize.X / (2 * _zoom)), value.Y + (ViewPortSize.Y / (2 * _zoom))); }

            //get { return new Vector2(_pos.X - (ViewPortSize.X / 2), _pos.Y - (ViewPortSize.Y / 2)); }
            //set { _pos = new Vector2(value.X + (ViewPortSize.X / 2), value.Y + (ViewPortSize.Y / 2)); }
        }

        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }
        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }
        // Auxiliary function to move the camera
        public void Move(Vector2 amount)
        {
            _pos += amount;
        }
        // Get set position
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public void Update()
        {
            //_zoom += (float)(Mouse.GetState().ScrollWheelValue - prevMouseState.ScrollWheelValue) / 1000f;
            //if (_zoom < 0.1f) 
            //    _zoom = 0.1f;


            if (!xLocked)
            {
                if (_pos.X - (ViewPortSize.X / (2 * _zoom)) <= 0)
                    _pos = new Vector2((ViewPortSize.X / (2 * _zoom)), _pos.Y);
                else if (_pos.X + (ViewPortSize.X / (2 * _zoom)) >= canvasSize.X)
                    _pos = new Vector2(canvasSize.X - (ViewPortSize.X / (2 * _zoom)), _pos.Y);
            }
            else
                _pos.X = lockedPos.X;

            if (!yLocked)
            {
                if (_pos.Y - (ViewPortSize.Y / (2 * _zoom)) <= 0)
                    _pos = new Vector2(_pos.X, (ViewPortSize.Y / (2 * _zoom)));
                else if (_pos.Y + (ViewPortSize.Y / (2 * _zoom)) >= canvasSize.Y)
                    _pos = new Vector2(_pos.X, canvasSize.Y - (ViewPortSize.Y / (2 * _zoom)));
            }
            else
                _pos.Y = lockedPos.Y;

            //prevMouseState = Mouse.GetState();
        }
        public Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            _transform =       // Thanks to o KB o for this solution
              Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(ViewPortSize.X * 0.5f, ViewPortSize.Y * 0.5f, 0));
            return _transform;
        }
    }
}
