using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InlowLukeGOL
{
    public partial class Form1 : Form
    {
        // The universe array
        const int universeX = 30;
        const int universeY = 30;
        bool[,] universe = new bool[universeX, universeY];
        bool[,] scratchPad = new bool[universeX, universeY];
        const float PI = 3.14159265359f;

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        private void UpdateGenerationText()
        {
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
        }

        private void UpdateLivingText()
        {
            int living = 0;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if(universe[x,y])
                    {
                        living++;
                    }
                }
            }

            toolStripStatusLabelLiving.Text = "Living = " + living.ToString();
        }

        private int CheckNeighbor(int x, int y)
        {
            if((x >= 0 && y >= 0) && (x < universeX && y < universeY))
            {
                return Convert.ToInt32(universe[x, y] == true);
            }
            return 0;
        }

        private void CalculateNextGen()
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int neighbors = CheckNeighbor(x - 1, y - 1) + CheckNeighbor(x, y - 1) + CheckNeighbor(x + 1, y - 1) +
                                    CheckNeighbor(x - 1, y) + 0 + CheckNeighbor(x + 1, y) +
                                    CheckNeighbor(x - 1, y + 1) + CheckNeighbor(x, y + 1) + CheckNeighbor(x + 1, y + 1);

                    if(universe[x, y])
                    {
                        scratchPad[x, y] = (neighbors == 2 || neighbors == 3);
                    }
                    else
                    {
                        scratchPad[x, y] = (neighbors == 3);
                    }
                }
            }
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            generations++;
            UpdateGenerationText();

            CalculateNextGen();

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = scratchPad[x, y];
                }
            }

            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
            UpdateLivingText();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];
                UpdateLivingText();

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            generations = 0;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            UpdateGenerationText();
            UpdateLivingText();
            timer.Stop();
            graphicsPanel1.Invalidate();
        }

        private void runToolStripButton_Click(object sender, EventArgs e)
        {
            timer.Start();
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            NextGeneration();
            UpdateLivingText();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newToolStripButton_Click(sender, e);
        }

        private void randomSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Random random = new Random();

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = (random.NextDouble() >= 0.5);
                }
            }
            UpdateLivingText();

            graphicsPanel1.Invalidate();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
        }

        private void fillAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = true;
                }
            }
            UpdateLivingText();
            graphicsPanel1.Invalidate();
        }

        private void fillCircleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int r = 10;

            for (int i = 0; i < 360; i++)
            {
                int pos = r + (r / 2) - 1;
                int x1 = (int)(r * Math.Cos(i * PI / 180));
                int y1 = (int)(r * Math.Sin(i * PI / 180));
                universe[pos + x1, pos + y1] = true;
            }
            UpdateLivingText();
            graphicsPanel1.Invalidate();
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog color = new ColorDialog();
            color.ShowDialog();
            gridColor = color.Color;
            graphicsPanel1.Invalidate();
        }

        private void fGColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog color = new ColorDialog();
            color.ShowDialog();
            cellColor = color.Color;
            graphicsPanel1.Invalidate();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Conway's Game of Life\nMade by Luke Inlow 2021", "About", MessageBoxButtons.OK);
            if (res == DialogResult.OK)
                return;
        }
    }
}