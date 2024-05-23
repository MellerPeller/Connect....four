using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test_Connect_four
{
    public partial class Form1 : Form
    {
        

        private Label[,] labels = new Label[6, 7];
        private int[,] board = new int[6, 7]; 
        private int turn = 1;
        private Label lblTurn;
        public Form1()
        {
            InitializeComponent();
            InitializeBoard();
            InitializeTurnIndicator();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
      
        /*Skapar en grid av labels 6*7 (42). 
         *Ställer in egenskaperna på labelsen såsom storlek, färg och position
         *Sparar positionen av labelsens row och col i "tag" 
          Lägger till Click, MouseEnter och MouseLeave så att dem kan användas*/
            private void InitializeBoard()
            {
                int labelSize = 50;
                for (int row = 0; row < 6; row++)
                {
                    for (int col = 0; col < 7; col++)
                    {
                        labels[row, col] = new Label();
                        labels[row, col].Size = new Size(labelSize, labelSize);
                        labels[row, col].Location = new Point(col * labelSize, row * labelSize + 50); // Offset for turn indicator
                        labels[row, col].BackColor = Color.White;
                        labels[row, col].BorderStyle = BorderStyle.FixedSingle;
                        labels[row, col].TextAlign = ContentAlignment.MiddleCenter;
                        labels[row, col].Tag = new Point(row, col);
                        labels[row, col].Click += new EventHandler(Label_Click);
                        labels[row, col].MouseEnter += new EventHandler(Label_MouseEnter);
                        labels[row, col].MouseLeave += new EventHandler(Label_MouseLeave);
                        this.Controls.Add(labels[row, col]);
                        board[row, col] = 0;
                    }
                }
            }

        /* Skapar en label som visar vems tur det är, 
         * ger denna label dens egenskaper likt InitializeBoard metoden. 
           använder UpdateTurnIndicator metoden sedan läggs labeln till i formen */ 
            private void InitializeTurnIndicator()
            {
                lblTurn = new Label();
                lblTurn.Size = new Size(350, 50);
                lblTurn.Location = new Point(0, 0);
                lblTurn.TextAlign = ContentAlignment.MiddleCenter;
                lblTurn.Font = new Font(lblTurn.Font, FontStyle.Bold);
                UpdateTurnIndicator();
                this.Controls.Add(lblTurn);
            }

        /* Ändrar så att lblTurn (labeln som InitializeTurnIndicator metoden skapade)
         visar vems tur det är genom att ändra från "player 1" till "player 2" sedan 
         ändrar den färgen på labeln */
            private void UpdateTurnIndicator()
            {
                lblTurn.Text = $"Player {turn}'s turn";
                lblTurn.BackColor = (turn == 1) ? Color.LightCoral : Color.LightYellow;
            }

            private void Label_Click(object sender, EventArgs e)
            {
                // Hittar vilken label du klickade på
                Label clickedLabel = sender as Label;
                Point location = (Point)clickedLabel.Tag;

                // gör så att "blocket" åker längst ner i row:en
                int row = FindAvailableRow(location.Y);
                if (row != -1)
                {
                    board[row, location.Y] = turn;
                    labels[row, location.Y].BackColor = (turn == 1) ? Color.Red : Color.Yellow;
                    // Tittar ifall någon vinner 
                    if (CheckWin(row, location.Y))
                    {
                        MessageBox.Show($"Player {turn} wins!");
                        ResetGame();
                    }
                    else
                    {
                        // Byt vems tur det är
                        turn = (turn == 1) ? 2 : 1;
                        UpdateTurnIndicator();
                    }
                }
            }


        /* Visar var "Blocket" kommer hamna om du clickar. 
           Hittar positionen var den skulle hamna och ändrar backcolor på alla labels upp till den musen pekar på */
        private void Label_MouseEnter(object sender, EventArgs e)
        {
            Label hoveredLabel = sender as Label;
            Point location = (Point)hoveredLabel.Tag;
            int col = location.Y;

            for (int row = 0; row < 6; row++)
            {
                if (board[row, col] == 0)
                {
                    labels[row, col].BackColor = (turn == 1) ? Color.LightCoral : Color.LightYellow;
                }
            }
        }

        /* Ändrar tillbaka labelsen efter att muspekaren lämnat row:en*/
        private void Label_MouseLeave(object sender, EventArgs e)
        {
            Label hoveredLabel = sender as Label;
            Point location = (Point)hoveredLabel.Tag;
            int col = location.Y;

            for (int row = 0; row < 6; row++)
            {
                if (board[row, col] == 0)
                {
                    labels[row, col].BackColor = Color.White; // Återställ till vit endast för tomma celler
                }
            }
        }



        private int FindAvailableRow(int col)
       {// Loopen startar från den nedersta raden och går uppåt till den översta raden.
            for (int row = 5; row >= 0; row--)
                {
                // tittar ifall brädan är full eller inte
                    if (board[row, col] == 0)
                    {
                        return row;
                    }
                }
                return -1; // Columnen är full
            }

            private bool CheckWin(int row, int col)
            {
            // Checkar horisontellt, vertikalt och båda diagonalerna
                return CheckDirection(row, col, 1, 0) || // Horisontell
                       CheckDirection(row, col, 0, 1) || // Vertikal
                       CheckDirection(row, col, 1, 1) || // Diagonal /
                       CheckDirection(row, col, 1, -1);  // Diagonal \
            }

            // Controllerar ifall det finns fyra i rad någonstans på brädet
            private bool CheckDirection(int row, int col, int dRow, int dCol)
            {
                int count = 1;
                count += CountDiscs(row, col, dRow, dCol);
                count += CountDiscs(row, col, -dRow, -dCol);
                return count >= 4;
            }

        // räknar antal sammanhängande "block" / "diskar" i en given riktning
        private int CountDiscs(int row, int col, int dRow, int dCol)
            {
                int r = row + dRow;
                int c = col + dCol;
                int count = 0;

                while (r >= 0 && r < 6 && c >= 0 && c < 7 && board[r, c] == turn)
                {
                    count++;
                    r += dRow;
                    c += dCol;
                }

                return count;
            }

        // Återställer spelbrädet till startpositionen för kunna starta upp ett till spel
            private void ResetGame()
            {
                for (int row = 0; row < 6; row++)
                {
                    for (int col = 0; col < 7; col++)
                    {
                        board[row, col] = 0;
                        labels[row, col].BackColor = Color.White;
                    }
                }
                turn = 1;
                UpdateTurnIndicator();
            }
        }
    }



