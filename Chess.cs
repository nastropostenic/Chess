using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.Marshalling;
using System.Text.RegularExpressions;

namespace Chess
{
    public class Chess : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _background;
        private Texture2D _whitePawn;
        private Texture2D _blackPawn;

        const int BackgroundWidth = 640;
        const int BackgroundHeight = 640;

        /*
        Pawn - num+1 or if occupied letter+-1, num +1
        Bishop - letter & num +-
        Knight - 
        Rook - letter +- | num +-
        Queen - letter & num +- | letter+- | num +-
        King - 

        */

        public static List<Square> Squares = new List<Square>();

        public static List<Piece> OnBoardPieces = new List<Piece>();
        public static List<Piece> CapturedPieces = new List<Piece>();

        private MouseState _CurrentMouseState;
        private MouseState _PreviousMouseState;
        private Square _SelectedSquare;


        public enum Colours
        {
            White = 1,
            Black = -1
        }

        public class Square
        {
            public char _Letter;
            public int _Number;
            public Vector2 _Position;
            public Piece OccupiedBy;

            public Square(char letter, int number)
            {
                _Letter = letter;
                _Number = number;
                _Position = new Vector2(BackgroundWidth / 8 * ((int)_Letter - 65), BackgroundHeight - BackgroundHeight / 8 * _Number);
            }
        }

        public abstract class Piece
        {
            public Square _OccupiedSquare;
            public Colours _Colour;

            public List<Square> CapturableSquares = new List<Square>();

            public Piece(Square occupiedSquare, Colours colour)
            {
                OnBoardPieces.Add(this);
                _OccupiedSquare = occupiedSquare;
                _Colour = colour;
            }

            public void Capture(Square square)
            {
                bool canCapture = false;
                foreach (Square capturableSquare in CapturableSquares)
                    if (capturableSquare.Equals(square))
                        canCapture = true;

                if (!canCapture)
                    return;

                if (square.OccupiedBy != null)
                {
                    OnBoardPieces.Remove(square.OccupiedBy);
                    CapturedPieces.Add(square.OccupiedBy);
                }

                this._OccupiedSquare = square;
                square.OccupiedBy = this;
            }
        }

        public class Pawn : Piece
        {
            public Pawn(Square occupiedSquare, Colours colour) : base(occupiedSquare, colour) {}

            public void CheckCapturableSquares()
            {
                foreach (Square square in Squares)
                {
                    if ((square._Letter == _OccupiedSquare._Letter && square._Number == _OccupiedSquare._Number + (int)_Colour && square.OccupiedBy == null) // move forward
                       ||
                       Math.Abs(square._Letter - _OccupiedSquare._Letter) == 1 && square._Number == _OccupiedSquare._Number + (int)_Colour && square.OccupiedBy != null) // move diagonal
                    {
                        this.CapturableSquares.Add(square);
                    }
                }
            }
        }

        public Chess()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = BackgroundWidth;
            _graphics.PreferredBackBufferHeight = BackgroundHeight;
            _graphics.ApplyChanges();

            for (int i = 0; i < 64; i++)
            {
                char letter = (char)(i / 8 + 65);
                int number = (i % 8 + 1);
                Square notation = new Square(letter, number);
                Squares.Add(notation);
            }

            foreach (Square square in Squares)
            {
                if (square._Number == 2)
                {
                    square.OccupiedBy = new Pawn(square, Colours.White);
                }
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _background = Content.Load<Texture2D>("background");
            _whitePawn = Content.Load<Texture2D>("wpawn");
            _blackPawn = Content.Load<Texture2D>("bpawn");
        }

        protected override void Update(GameTime gameTime)
        {   
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _PreviousMouseState = _CurrentMouseState;
            _CurrentMouseState = Mouse.GetState();

            foreach (Square square in Squares)
            {
                if (!(new Rectangle(Convert.ToInt32(square._Position.X), Convert.ToInt32(square._Position.Y), BackgroundWidth / 8, BackgroundHeight / 8).Contains(_CurrentMouseState.Position)))
                    continue;
                if (!(_CurrentMouseState.LeftButton == ButtonState.Pressed && _PreviousMouseState.LeftButton == ButtonState.Released))
                    continue;

                if (!_SelectedSquare)
                {
                    _SelectedSquare = square;
                    continue;
                }

                _SelectedSquare.OccupiedBy.CheckCapturableSquares();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.Draw(
                _background,
                new Rectangle(0,0, BackgroundWidth, BackgroundHeight),
                Color.White
            );

            foreach (Square square in Squares)
            {
                if (square.OccupiedBy != null)
                {
                    _spriteBatch.Draw(
                        _whitePawn,
                        new Rectangle(Convert.ToInt32(square._Position.X), Convert.ToInt32(square._Position.Y), BackgroundWidth / 8, BackgroundHeight / 8),
                        Color.White
                    );
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
