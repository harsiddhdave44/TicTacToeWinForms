using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using MetroFramework.Forms;
using System.Drawing;
using System.Linq;

namespace TicTacToe
{
    public partial class Form1 : MetroForm
    {
        XmlDocument doc;
        XmlElement root;
        Communication c;
        string wintype = string.Empty;
        string xpos = string.Empty, opos = string.Empty;
        string path = @"D:\gamedata.xml";
        bool userturn;
        int userscore = 0, aiscore = 0, turncount = 0;
        int pattern=-1;
        List<int[]> verticalPattern = new List<int[]>();
        List<int[]> horizontalPattern = new List<int[]>();
        List<int[]> diagonalPattern = new List<int[]>();

        List<MetroFramework.Controls.MetroButton> targetBtns = new List<MetroFramework.Controls.MetroButton>();
        List<MetroFramework.Controls.MetroButton> buttonEmpty = new List<MetroFramework.Controls.MetroButton>();

        public Form1()
        {
            InitializeComponent();
            userturn = true;
        }
        void InitializeWinPatterns()
        {
            //pattern=0
            verticalPattern.Add(new int[] { 1, 2, 3 });
            verticalPattern.Add(new int[] { 4, 5, 6 });
            verticalPattern.Add(new int[] { 7, 8, 9 });
            //pattern=1
            horizontalPattern.Add(new int[] { 1, 4, 7 });
            horizontalPattern.Add(new int[] { 2, 5, 8 });
            horizontalPattern.Add(new int[] { 3, 6, 9 });
            //pattern=2
            diagonalPattern.Add(new int[] { 1, 5, 9 });
            diagonalPattern.Add(new int[] { 3, 5, 7 });
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //c = new Communication();
            //t = new Thread(new ThreadStart(c.listener));
            InitializeWinPatterns();
            //t.Start();
        }
        void writedata()
        {
            if (File.Exists(path))
            {
                doc = new XmlDocument();
                doc.Load(path);

                XmlElement game = doc.CreateElement("game");
                doc.DocumentElement.AppendChild(game);

                XmlElement x_pos = doc.CreateElement("x_pos");
                x_pos.InnerText = xpos;
                game.AppendChild(x_pos);

                if (opos != string.Empty)
                {
                    XmlElement o_pos = doc.CreateElement("o_pos");
                    o_pos.InnerText = opos;
                    game.AppendChild(o_pos);
                }
                else
                {
                    XmlElement o_pos = doc.CreateElement("o_pos");
                    o_pos.InnerText = null;
                    game.AppendChild(o_pos);
                }
                doc.Save(path);
            }
            else
            {
                doc = new XmlDocument();

                root = doc.CreateElement("root");
                doc.AppendChild(root);

                XmlElement game = doc.CreateElement("game");
                root.AppendChild(game);

                XmlElement x_pos = doc.CreateElement("x_pos");
                x_pos.InnerText = xpos;
                game.AppendChild(x_pos);

                if (opos != string.Empty)
                {
                    XmlElement o_pos = doc.CreateElement("o_pos");
                    o_pos.InnerText = opos;

                    game.AppendChild(o_pos);
                }
                else
                {
                    XmlElement o_pos = doc.CreateElement("o_pos");
                    o_pos.InnerText = null;

                    game.AppendChild(o_pos);
                }
                doc.Save(path);
            }
        }
        //Tuple<List<String>, List<String>> getpos()
        //{
        //    Tuple<List<String>, List<String>> positions;

        //    List<String> opos = new List<string>();
        //    List<String> xpos = new List<string>();
        //    int i = 0;
        //    foreach (Control c in Controls)
        //    {
        //        try
        //        {
        //            Button b = (Button)c;
        //            if (b.Text == "X")
        //                xpos.Add(b.Name);
        //            else if (b.Text == "O")
        //                opos.Add(b.Name);
        //        }
        //        catch
        //        { }
        //    }
        //    positions = new Tuple<List<string>, List<string>>(xpos, opos);
        //    return positions;
        //}
        void ai()
        {
            int availCount = 0;
            Dictionary<int, Button> available_spaces = new Dictionary<int, Button>();
            Exception gameover = new Exception();
            foreach (Control c in Controls)
            {
                if(c is Button)
                {
                    Button button_available = (Button)c;
                    
                    if (button_available.Text == string.Empty)
                    {
                        ++availCount;
                        available_spaces.Add(availCount, button_available);
                    }
                }
            }
            try
            {
                if (availCount == 0)
                {
                    throw gameover;
                }
                Random r = new Random();
                //MessageBox.Show(r.Next(1,);
                //Control control = available_spaces[r.Next(1, availCount)];
                Control control=null;
                   
                int moveNum = FindNextMove();
                //= ChooseMove();
                if (moveNum >= 0)
                {
                    control = buttonEmpty[moveNum];
                    //control = available_spaces[moveNum];
                }
                else
                    control = available_spaces[r.Next(1, availCount)];
                Button button = (Button)control;

                button.Text = "O";
                userturn = true;
                ++turncount;

                if (turncount >= 5)
                {
                    if (checkforwinner() == 0)
                    {
                        ++aiscore;
                        //writedata();
                        lbl_aiscore.Text = aiscore.ToString();
                        if (MessageBox.Show("Already knew that !!\nAI wins the game\nWanna take a chance again ?", "AI WON", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            restartgame();
                        else
                            disablebuttons();
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }
        public void restartgame()
        {
            //userscore = aiscore = turncount = 0;
            //userturn = true;
            //lbl_aiscore.Text = lbl_userscore.Text = "0";
            pattern = -1;
            foreach (Control c in Controls)
            {
                if(c is Button)
                {
                    Button b = (Button)c; ;
                    b.Text = "";
                }
            }
            xpos = opos = string.Empty;
            userturn = true;
            turncount = 0;
        }
        public int checkforwinner()
        {
            //-1 is default value which shows that no one has won till now
            //0 means the AI won the game 
            //1 means user won the game
            int win = -1;
            //MessageBox.Show(userturn.ToString());
            if (!userturn)
            {
                //Vertical check for winnning
                if (b1.Text == "X" && b2.Text == "X" && b3.Text == "X")
                {
                    win = 1;
                    xpos += b1.Name + "," + b2.Name + "," + b3.Name;
                    //wintype = "vertical";
                }
                else if (b4.Text == "X" && b5.Text == "X" && b6.Text == "X")
                {
                    win = 1;
                    xpos += b4.Name + "," + b5.Name + "," + b6.Name;
                }
                else if (b7.Text == "X" && b8.Text == "X" && b9.Text == "X")
                {
                    win = 1;
                    xpos += b7.Name + "," + b8.Name + "," + b9.Name;
                }
                //Horizontal check for winning
                if (b1.Text == "X" && b4.Text == "X" && b7.Text == "X")
                {
                    win = 1;
                    xpos += b1.Name + "," + b4.Name + "," + b7.Name;
                }
                else if (b2.Text == "X" && b5.Text == "X" && b8.Text == "X")
                {
                    win = 1;
                    xpos += b2.Name + "," + b5.Name + "," + b8.Name;
                }
                else if (b3.Text == "X" && b6.Text == "X" && b9.Text == "X")
                {
                    win = 1;
                    xpos += b3.Name + "," + b6.Name + "," + b9.Name;
                }
                //Diagonal Check for winning
                if (b1.Text == "X" && b5.Text == "X" && b9.Text == "X")
                {
                    win = 1;
                    xpos += b1.Name + "," + b5.Name + "," + b9.Name;
                }
                else if (b3.Text == "X" && b5.Text == "X" && b7.Text == "X")
                {
                    win = 1;
                    xpos += b3.Name + "," + b5.Name + "," + b7.Name;
                }
            }
            else
            {
                //Vertical check for winnning
                if (b1.Text == "O" && b2.Text == "O" && b3.Text == "O")
                {
                    win = 0;
                    opos += b1.Name + "," + b2.Name + "," + b3.Name;
                }
                else if (b4.Text == "O" && b5.Text == "O" && b6.Text == "O")
                {
                    win = 0;
                    xpos += b4.Name + "," + b5.Name + "," + b6.Name;
                }
                else if (b7.Text == "O" && b8.Text == "O" && b9.Text == "O")
                {
                    win = 0;
                    xpos += b7.Name + "," + b8.Name + "," + b9.Name;
                }
                //Horizontal check for winning
                if (b1.Text == "O" && b4.Text == "O" && b7.Text == "O")
                {
                    win = 0;
                    xpos += b1.Name + "," + b4.Name + "," + b7.Name;
                }
                else if (b2.Text == "O" && b5.Text == "O" && b8.Text == "O")
                {
                    win = 0;
                    xpos += b2.Name + "," + b5.Name + "," + b8.Name;
                }
                else if (b3.Text == "O" && b6.Text == "O" && b9.Text == "O")
                {
                    win = 0;
                    xpos += b3.Name + "," + b6.Name + "," + b9.Name;
                }
                //Diagonal Check for winning
                if (b1.Text == "O" && b5.Text == "O" && b9.Text == "O")
                {
                    win = 0;
                    xpos += b1.Name + "," + b5.Name + "," + b9.Name;
                }
                else if (b3.Text == "O" && b5.Text == "O" && b7.Text == "O")
                {
                    win = 0;
                    xpos += b3.Name + "," + b5.Name + "," + b7.Name;
                }
            }
            return win;
        }
        #region "checkforwinner" fucntion made using LINQ (WrongOne)
        //int  checkforwinner()
        //{
        //    //-1 is default value which shows that no one has won till now
        //    //0 means the AI won the game 
        //    //1 means user won the game
        //    int win = -1;
        //    int count = 0;
        //    Control[] con = new Control[Controls.Count];

        //    foreach (Control c in Controls)
        //    {
        //        con[count] = c;
        //        count++;
        //    }
        //    if (userturn)
        //    {
        //        var xbuttons = from c in con
        //                       where c.Text == "X"
        //                       select c;

        //        var cons = from x in xbuttons
        //                   where (x.Text.Contains("1") || x.Text.Contains("2") || x.Text.Contains("3")) 
        //                   || ((x.Text.Contains("4") || x.Text.Contains("5") || x.Text.Contains("6"))
        //                   || (x.Text.Contains("7") || x.Text.Contains("8") || x.Text.Contains("9"))

        //                   || (x.Text.Contains("1") || x.Text.Contains("4") || x.Text.Contains("7"))
        //                   || (x.Text.Contains("2") || x.Text.Contains("5") || x.Text.Contains("8"))
        //                   || (x.Text.Contains("3") || x.Text.Contains("6") || x.Text.Contains("9"))

        //                   || (x.Text.Contains("1") || x.Text.Contains("5") || x.Text.Contains("9"))
        //                   || (x.Text.Contains("3") || x.Text.Contains("5") || x.Text.Contains("7")))
        //                   select x;
        //        int countx = 0;

        //        foreach (Control control in cons)
        //            countx++;

        //        if (countx==3)
        //             win = 1;

        //    }
        //    else
        //    {
        //        var ybuttons = from c in con
        //                     where c.Text == "O"
        //                     select c;

        //        var cons = from x in ybuttons
        //                   where (x.Text.Contains("1") || x.Text.Contains("2") || x.Text.Contains("3"))
        //                   || (x.Text.Contains("4") || x.Text.Contains("5") || x.Text.Contains("6"))
        //                   || (x.Text.Contains("7") || x.Text.Contains("8") || x.Text.Contains("9"))

        //                   || (x.Text.Contains("1") || x.Text.Contains("4") || x.Text.Contains("7"))
        //                   || (x.Text.Contains("2") || x.Text.Contains("5") || x.Text.Contains("8"))
        //                   || (x.Text.Contains("3") || x.Text.Contains("6") || x.Text.Contains("9"))

        //                   || (x.Text.Contains("1") || x.Text.Contains("5") || x.Text.Contains("9"))
        //                   || (x.Text.Contains("3") || x.Text.Contains("5") || x.Text.Contains("7"))
        //                   select x;
        //        int countx = 0;

        //        foreach (Control control in cons)
        //            countx++;

        //        if (countx == 3)
        //            win = 0;

        //    }
        //    return win;
        //}
        #endregion
        bool checkdraw()
        {
            foreach(Control c in Controls)
            {
                if (c is Button)
                    if (c.Text == "")
                        return false;
            }
            return true;
        }
        void disablebuttons()
        {
            foreach (Control c in Controls)
            {
                if (c is Button)
                {
                    Button b = (Button)c;
                    b.Enabled = false;
                }
            }
        }
        int FindNextMove()
        {
            int index=0;
            int i2 = 0;
            pattern = -1;
            var btns = this.Controls.OfType<MetroFramework.Controls.MetroButton>();
            targetBtns.Clear();
            verticalPattern.ForEach(s =>
            {
                index++;
                pattern = 0;
                s.ToList().ForEach(num =>
                {
                    //targetBtns.AddRange(btns.Where(x => x.Name.Contains(num.ToString()) && x.Text == "X"));
                    btns.ToList().ForEach(b =>
                    {
                        if (b.Name.Contains(num.ToString()) && b.Text == "X")
                        {
                            targetBtns.Add(b);
                            i2 = index-1;
                        }
                    });
                });
                //button.AddRange(btns.Where(x => x.Name.Contains(num.ToString()) && x.Text == ""));
            });
             
            if(targetBtns.Count==0)
            {
                index = 0;
                horizontalPattern.ForEach(s =>
                {
                    index++;
                    pattern = 1;
                    s.ToList().ForEach(num =>
                    {   
                        //targetBtns.AddRange(btns.Where(x => x.Name.Contains(num.ToString()) && x.Text == "X"));
                        btns.ToList().ForEach(b =>
                        {
                            if (b.Name.Contains(num.ToString()) && b.Text == "X")
                            {
                                targetBtns.Add(b);
                                i2 = index - 1;
                            }
                        });
                    });
                });
                if(targetBtns.Count==0)
                {
                    index = 0;
                    pattern = 2;
                    diagonalPattern.ForEach(s =>
                    {
                        index++;
                        s.ToList().ForEach(num =>
                        {
                            //targetBtns.AddRange(btns.Where(x => x.Name.Contains(num.ToString()) && x.Text == "X"));
                            btns.ToList().ForEach(b =>
                            {
                                if (b.Name.Contains(num.ToString()) && b.Text == "X")
                                {
                                    targetBtns.Add(b);
                                    i2 = index - 1;
                                }
                            });
                        });
                    });
                }
            }
            if(targetBtns.Count>1)
            {
                if(pattern==0)
                {
                    foreach(var n in verticalPattern[i2])
                    {
                        buttonEmpty.AddRange(btns.Where(x => x.Name.Contains(n.ToString()) && x.Text == ""));
                    }
                }
                if (buttonEmpty.Count > 0)
                {
                    Random r = new Random();
                    int i = r.Next(0, buttonEmpty.Count-1);
                    return i;
                }
                //else
                //{
                //    buttonEmpty[0].Text = "O";
                //}
            }
            return -1;
            #region CommentCode
            //bool isAdd = false;

            //foreach(var i in verticalPattern)
            //{
            //    foreach(var num in i)
            //    {
            //        targetBtns.Add(btns.Where(x => x.Name.Contains(num.ToString()) && x.Text == "").First());
            //        isAdd = true;
            //        break;
            //    }
            //    break;
            //}
            //foreach (var i in horizontalPattern)
            //{
            //    foreach (var num in i)
            //    {
            //        targetBtns.Add(btns.Where(x => x.Name.Contains(num.ToString()) && x.Text == "").First());
            //        isAdd = true;
            //    }
            //}
            //foreach (var i in diagonalPattern)
            //{
            //    foreach (var num in i)
            //    {
            //        targetBtns.Add(btns.Where(x => x.Name.Contains(num.ToString()) && x.Text == "").First());
            //        isAdd = true;
            //    }
            //}
            #endregion
        }
        int ChooseMove()
        {
            int count = 0;
            int listIndex = 0;
            MetroFramework.Controls.MetroButton button=null;
            foreach (var btn in Controls)
            {
                #region Horizontal Check
                if (btn is MetroFramework.Controls.MetroButton)
                {
                    button = btn as MetroFramework.Controls.MetroButton;
                    //Horizontal pattern checking
                    foreach (var i in horizontalPattern)
                    {
                        foreach (var num in i)
                        {
                            if (button.Name.Contains(num.ToString()) && button.Text == "X")
                            {
                                count++;
                                targetBtns.Add(button);
                            }
                        }
                        listIndex++;
                        if (count >= 1)
                        {
                            pattern = 1;
                            break;
                        }
                    }
                    #endregion
                    #region Vertical Check
                    if (count >= 0)
                    {
                        // Vertical pattern checking
                        foreach (var i in verticalPattern)
                        {
                            listIndex = 0;
                            foreach (var num in i)
                            {
                                if (button.Name.Contains(num.ToString()) && button.Text == "O")
                                {
                                    count++;
                                    targetBtns.Add(button);
                                }
                            }

                            if (count >= 1)
                            {
                                pattern = 0;
                                listIndex++;
                                break;
                            }
                        }
                    }
                    #endregion
                    #region Diagonal Check
                    else if (count >= 0)
                    {
                        // Diagonal pattern checking
                        foreach (var i in diagonalPattern)
                        {
                            listIndex = 0;
                            foreach (var num in i)
                            {
                                if (button.Name.Contains(num.ToString()) && button.Text == "O")
                                {
                                    count++;
                                    targetBtns.Add(button);
                                }
                            }
                            listIndex++;
                            if (count >= 1)
                            {
                                pattern = 1;
                                break;
                            }
                        }
                        //if (count >= 1)
                        //    break;
                    }
                }
                #endregion
            }
            
            if (count >= 1)
            {
                return GetNextMovePoint(listIndex);
            }
            else
                return 0;
        }

        private int GetNextMovePoint(int listIndex)
        {
            //Horizontal Pattern
            if (pattern == 0)
            {
                try
                {
                    foreach (var b in Controls)
                    {
                        var btn = b as MetroFramework.Controls.MetroButton;
                        foreach (var i in horizontalPattern[listIndex])
                        {
                            if (btn.Name.Contains(i.ToString()) && btn.Text == string.Empty)
                            {
                                //targetBtns.Add(btn);
                                return i;
                            }
                        }
                    }
                    Random r = new Random();
                    return r.Next(targetBtns.Count - 1);
                }
                catch (Exception)
                {
                   
                }
            }//Vertical Pattern
            else if(pattern==1)
            {
                try
                {
                    foreach (var b in Controls)
                    {
                        var btn = b as MetroFramework.Controls.MetroButton;
                        foreach (var i in verticalPattern[listIndex])
                        {
                            if (btn.Name.Contains(i.ToString()) && btn.Text == string.Empty)
                            {
                                //targetBtns.Add(btn);
                                return i;
                            }
                        }
                    }
                    Random r = new Random();
                    return r.Next(targetBtns.Count - 1);
                }
                catch (Exception)
                {
                 
                }
            }//Diagonal Pattern
            else if(pattern==2)
            {
                try
                {
                    foreach (var b in Controls)
                    {
                        var btn = b as MetroFramework.Controls.MetroButton;
                        foreach (var i in diagonalPattern[listIndex])
                        {
                            if (btn.Name.Contains(i.ToString()) && btn.Text == string.Empty)
                            {
                                //targetBtns.Add(btn);
                                return i;
                            }
                        }
                    }
                    Random r = new Random();
                    return r.Next(targetBtns.Count - 1);
                }
                catch (Exception)
                {
                    
                }
            }
            return -1;
        }
        private void button9_Click(object sender, EventArgs e)
        {
            Exception used = new Exception();
            Button b = (Button)sender;
            //c.Connect("X pos=" + b.Name);
            
            try
            {
                if (b.Text.Contains("X") || b.Text.Contains("O")) 
                    throw used;
                if (userturn)
                {
                    b.Text = "X";
                    b.BackColor = Color.Black;
                    userturn = !userturn;
                    ++turncount;
                    if (turncount >= 5)
                        if (checkforwinner() == 1)
                        {
                            writedata();
                            ++userscore;
                            lbl_userscore.Text = userscore.ToString();
                            if (MessageBox.Show("You Won the Game\nWanna Play again ?", "Congo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                restartgame();
                            else
                                disablebuttons();
                        }
                        else if(checkdraw())
                        {
                            if (MessageBox.Show("It is a DRAW !!\nWant to play again ??", "Draw", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                restartgame();
                            }
                            else
                                disablebuttons();
                        }
                }
                if (!userturn)
                {
                    ai();
                }
            }
            catch (Exception ex)
            {
                
            }

        }
    }
}