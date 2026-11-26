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

        public enum Colours
        {
            White = 1,
            Black = -1
        }

        public struct Square
        {
            public char _Letter;
            public int _Number;
            public Piece OccupiedBy;

            public Square(char letter, int number)
            {
                _Letter = letter;
                _Number = number;
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
                    if ((square._Letter == _OccupiedSquare._Letter && square._Number == _OccupiedSquare._Number + (int)_Colour && square.OccupiedBy == null)
                       ||
                       (square._Letter == _OccupiedSquare._Letter + 1 || square._Letter == _OccupiedSquare._Letter - 1) && square._Number == _OccupiedSquare._Number + (int)_Colour && square.OccupiedBy != null)
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
            for (int i = 0; i < 64; i++)
            {
                char letter = (char)(i / 8 + 65);
                int number = (i % 8 + 1);
                Square position = new Square(letter, number);
                Squares.Add(position);
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
