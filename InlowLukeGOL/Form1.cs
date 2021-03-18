using System;
using System.IO;
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
        int universeX = 30;
        int universeY = 30;
        bool[,] universe;
        bool[,] scratchPad;
        const float PI = 3.14159265359f;
        int timeInterval = 100;
        bool showGrid;
        bool showNeighborCount;
        bool loaded;
        string loadPath;

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

            this.universe = new bool[universeX, universeY];
            this.scratchPad = new bool[universeX, universeY];
            this.showGrid = true;
            this.showNeighborCount = true;
            this.loaded = false;

            // Setup the timer
            timer.Interval = timeInterval; // milliseconds
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

        public int GetNeighborCount(int x, int y)
        {
            return CheckNeighbor(x - 1, y - 1) + CheckNeighbor(x, y - 1) + CheckNeighbor(x + 1, y - 1) +
                                    CheckNeighbor(x - 1, y) + 0 + CheckNeighbor(x + 1, y) +
                                    CheckNeighbor(x - 1, y + 1) + CheckNeighbor(x, y + 1) + CheckNeighbor(x + 1, y + 1);
        }

        private void CalculateNextGen()
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int neighbors = GetNeighborCount(x, y);

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
            UpdateLivingText();

            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            Size cellSize = new Size(graphicsPanel1.ClientSize.Width / universeX, graphicsPanel1.ClientSize.Height / universeY);
            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1.0f);
            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);
            Brush labelBrush = new SolidBrush(Color.Red);
            Font labelFont = new Font("Arial", cellSize.Width / 3);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    bool cell = universe[x, y];
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellSize.Width;
                    cellRect.Y = y * cellSize.Height;
                    cellRect.Size = cellSize;

                    // Fill the cell with a brush if alive
                    if (cell == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    if(showGrid)
                    {
                        // Outline the cell with a pen
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    }

                    int n = GetNeighborCount((int)x, (int)y);
                    // Show neighbor count if it's above 0
                    if(showNeighborCount && (n > 0))
                    {
                        e.Graphics.DrawString(n.ToString(), labelFont, labelBrush, new PointF(cellRect.X + 5, cellRect.Y));
                    }
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

        private void ClearUniverse()
        {
            generations = 0;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                }
            }
            this.loaded = false;
            UpdateGenerationText();
            UpdateLivingText();
            timer.Stop();
            graphicsPanel1.Invalidate();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            ClearUniverse();
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
            ClearUniverse();
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

        StringBuilder CreateStringBuilder()
        {
            StringBuilder builder = new StringBuilder();

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    char c;
                    if (universe[x, y] == true)
                        c = 'O';
                    else
                        c = '.';
                    builder.Append(c);
                }
                builder.Append('\n');
            }
            return builder;
        }

        bool[,] LoadFromString(string src)
        {
            src = src.Trim();
            string[] result = src.Split(new[] { '\r', '\n' });
            int h = result.Length;

            int w = 0;
            int comments = 0;
            for(int i = 0; i < result.Length; i++)
            {
                string line = result[i];
                if (line[0] == '!')
                {
                    comments++;
                    continue;
                }
                w++;
            }
            h -= comments;
            this.universeX = w;
            this.universeY = h;
            this.universe = new bool[w, h];
            this.scratchPad = new bool[w, h];

            // Comments should be at the beginning of the file
            for(int y = comments; y < h; y++)
            {
                string line = result[y - comments];
                for(int x = 0; x < line.Length; x++)
                {
                    if (line[x] == '.') universe[x, y] = false;
                    else if (line[x] == 'O') universe[x, y] = true;
                }
            }

            return universe;
        }

        private void SaveUniverse(bool saveAs = false)
        {
            if(!loaded || saveAs)
            {
                SaveFileDialog save = new SaveFileDialog();
                save.DefaultExt = ".cells";
                save.Filter = "Cell file (.cells)|*.cells";
                save.RestoreDirectory = true;

                if (save.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(save.FileName, CreateStringBuilder().ToString());
                    this.loadPath = save.FileName;
                    this.loaded = true;
                }
            }
            else
            {
                File.WriteAllText(loadPath, CreateStringBuilder().ToString());
            }
        }

        private void OpenUniverse()
        {
            OpenFileDialog open = new OpenFileDialog();
            open.DefaultExt = ".cells";
            open.Filter = "Cell file (.cells)|*.cells";
            open.RestoreDirectory = true;

            if (open.ShowDialog() == DialogResult.OK)
            {
                this.universe = LoadFromString(File.ReadAllText(open.FileName));
                this.loadPath = open.FileName;
                this.loaded = true;
                UpdateLivingText();
                graphicsPanel1.Invalidate();
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveUniverse();
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

        // I added this just for fun
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
            if (DialogResult.OK == color.ShowDialog())
            {
                gridColor = color.Color;
                graphicsPanel1.Invalidate();
            }
        }

        private void fGColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog color = new ColorDialog();
            if(DialogResult.OK == color.ShowDialog())
            {
                cellColor = color.Color;
                graphicsPanel1.Invalidate();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Conway's Game of Life\nMade by Luke Inlow 2021", "About", MessageBoxButtons.OK);
            if (res == DialogResult.OK)
                return;
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int oldX = universeX;
            int oldY = universeY;

            PropertiesDialog props = new PropertiesDialog(oldX, oldY, timeInterval);

            if (DialogResult.OK == props.ShowDialog())
            {
                this.universeX = props.universeX;
                this.universeY = props.universeY;

                // Resize the arrays
                this.scratchPad = new bool[universeX, universeY];

                bool[,] new_universe = new bool[universeX, universeY];
                for(int i = 0; i < oldX; i++)
                {
                    for (int j = 0; j < oldY; j++)
                    {
                        new_universe[i, j] = universe[i, j];
                    }
                }
                this.universe = new_universe;

                this.timeInterval = props.timeInterval;
                timer.Interval = timeInterval;

                props.Dispose();
            }

            graphicsPanel1.Invalidate();
        }

        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RandomDialog randomDialog = new RandomDialog();

            if(randomDialog.ShowDialog() == DialogResult.OK)
            {
                Random random = new Random(randomDialog.seed);

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

            graphicsPanel1.Invalidate();
        }

        private void bGColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog color = new ColorDialog();
            if (DialogResult.OK == color.ShowDialog())
            {
                graphicsPanel1.BackColor = color.Color;
                graphicsPanel1.Invalidate();
            }
        }

        private void toggleGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showGrid = !showGrid;
            graphicsPanel1.Invalidate();
        }

        private void toggleNeighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showNeighborCount = !showNeighborCount;
            graphicsPanel1.Invalidate();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveUniverse();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveUniverse(true);
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenUniverse();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenUniverse();
        }
    }
}