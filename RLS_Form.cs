using System;
using System.Collections.Generic;
using System.Threading;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace RLS
{
    public partial class RLS_Form : Form
    { 


        public const int SCALE = 10;
        public void DrawAxes(Graphics graphics)
        {
            Pen pen = new Pen(Color.Black, 1);
            graphics.DrawLine(pen, new Point(-Width / 4, 0), new Point(Width / 4, 0));
            graphics.DrawLine(pen, new Point(Width / 4, 0), new Point(Width / 4 - 4 * SCALE, SCALE));
            graphics.DrawLine(pen, new Point(Width / 4, 0), new Point(Width / 4 - 4 * SCALE, -SCALE));
            graphics.DrawLine(pen, new Point(0, -Height / 3), new Point(0, Height / 3));
            graphics.DrawLine(pen, new Point(0, Height / 3), new Point(SCALE, Height / 3 - 3 * SCALE));
            graphics.DrawLine(pen, new Point(0, Height / 3), new Point(-SCALE, Height / 3 - 3 * SCALE));
        }
        public void DrawRLS(Graphics graphics, RLS rlc)
        {
            graphics.DrawEllipse(
                new Pen(Color.Green, 2), 
                new RectangleF(
                    Convert.ToSingle(rlc.Current_Position.X - rlc.Detection_Range) * SCALE, 
                    Convert.ToSingle(rlc.Current_Position.Y - rlc.Detection_Range) * SCALE, 
                    Convert.ToSingle(rlc.Detection_Range * 2 * SCALE), 
                    Convert.ToSingle(rlc.Detection_Range * 2 * SCALE)
                ));
        }
        public void DrawCommandPost(Graphics graphics, CommandPost cp)
        {
            graphics.DrawEllipse(
                new Pen(Color.YellowGreen, 2), 
                new RectangleF(
                    Convert.ToSingle(cp.Current_Position.X - cp.Safety_Distance) * SCALE,
                    Convert.ToSingle(cp.Current_Position.Y - cp.Safety_Distance) * SCALE, 
                    Convert.ToSingle(cp.Safety_Distance * 2 * SCALE), 
                    Convert.ToSingle(cp.Safety_Distance * 2 * SCALE)
                    ));
        }
        public void DrawTarget(Graphics graphics, Target target)
        {
            Bitmap pt = new Bitmap(1, 1);
            if (target.b)
            {
                if(target.GetType() == typeof(SAMMissile))
                {
                    pt.SetPixel(0, 0, Color.Yellow);
                }
                else
                    pt.SetPixel(0, 0, Color.Red);
            }
            else
            {
                pt.SetPixel(0, 0, Color.Gray);
            }
            // Draw bitmap on Graphics surface
            graphics.DrawImageUnscaled(pt, Convert.ToInt32(target.Current_Position.X * SCALE), Convert.ToInt32(target.Current_Position.Y * SCALE));

            Thread.Sleep(50);
        }
        public void Draw(object O)
        {
            DoubleBuffered = true;
            Simulator simulator = O as Simulator;
            if(simulator == null)
            {
                return;
            }
            Graphics graphics = pictureBox1.CreateGraphics();
            graphics.TranslateTransform(pictureBox1.Width / 2, pictureBox1.Height / 2);

            DrawAxes(graphics);

            DrawRLS(graphics, simulator.my_RLS);   
            
            DrawCommandPost(graphics, simulator.my_CP);
            List<Target> tmp = new List<Target>(simulator.SAMMissileList.Concat(simulator.targets));
            foreach(Target target in tmp)
            {
                DrawTarget(graphics, target);
            }
            Invalidate();
            Thread.Sleep(100);
        }

        public RLS_Form()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
        }

        private void RLS_Form_Load(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            List<string> columns = new List<string>() { "Date", "Time", "Comment" };
            List<string> values = new List<string>() { now.ToString("d"), now.ToString("T"), "None" };
            WorkingWithDB.Insert("Experiments", columns, values);
            WorkingWithDB.ShowTablesInListBox(listBox2);

            dataGridView1.RowCount = Simulator.NUMBER + CommandPost.NUMBER;

            radioButton2.Checked = true;
            radioButton1.ForeColor = radioButton2.ForeColor = radioButton3.ForeColor = radioButton4.ForeColor = Color.Gray;

            textBox1.Text = Data.target_x.ToString();
            textBox1.ForeColor = Color.Gray;

            textBox2.Text = Data.target_y.ToString();
            textBox2.ForeColor = Color.Gray;

            textBox3.Text = Data.target_v.ToString();
            textBox3.ForeColor = Color.Gray;

            textBox4.Text = Data.target_K.ToString();
            textBox4.ForeColor = Color.Gray;

            textBox5.Text = Data.target_fuel_mass.ToString();
            textBox5.ForeColor = Color.Gray;

            textBox6.Text = Data.target_critical_fuel_mass.ToString();
            textBox6.ForeColor = Color.Gray;

            textBox7.Text = Data.target_mass_flow_of_fuel.ToString();
            textBox7.ForeColor = Color.Gray;

            textBox8.Text = Data.target_Cx.ToString();
            textBox8.ForeColor = Color.Gray;

            textBox9.Text = Data.target_S.ToString();
            textBox9.ForeColor = Color.Gray;

            textBox10.Text = Data.cp_x.ToString();
            textBox10.ForeColor = Color.Gray;

            textBox11.Text = Data.cp_y.ToString();
            textBox11.ForeColor = Color.Gray;

            textBox12.Text = Data.cp_safety_distance.ToString();
            textBox12.ForeColor = Color.Gray;

            textBox13.Text = Data.rls_x.ToString();
            textBox13.ForeColor = Color.Gray;

            textBox14.Text = Data.rls_y.ToString();
            textBox14.ForeColor = Color.Gray;

            textBox15.Text = Data.rls_det_range.ToString();
            textBox15.ForeColor = Color.Gray;

            textBox16.Text = Data.simulator_T0.ToString();
            textBox16.ForeColor = Color.Gray;

            textBox17.Text = Data.simulator_Tk.ToString();
            textBox17.ForeColor = Color.Gray;

            textBox18.Text = Data.simulator_dT.ToString();
            textBox18.ForeColor = Color.Gray;

            textBox19.Text = Data.simulator_density.ToString();
            textBox19.ForeColor = Color.Gray;

            textBox20.Text = Data.simulator_path.ToString();
            textBox20.ForeColor = Color.Gray;

            textBox21.Text = Data.cp_TMax.ToString();
            textBox21.ForeColor = Color.Gray;

            textBox22.Text = Data.cp_DLose.ToString();
            textBox22.ForeColor = Color.Gray;

            textBox23.Text = Data.cp_PMax.ToString();
            textBox23.ForeColor = Color.Gray;

            textBox24.Text = Data.cp_PMin.ToString();
            textBox24.ForeColor = Color.Gray;

            textBox25.Text = Data.cp_PReq.ToString();
            textBox25.ForeColor = Color.Gray;
        }


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Data.target_type = Type.None;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            Data.target_type = Type.aircraft;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            Data.target_type = Type.missile;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            Data.target_type = Type.sammissile;
        }
        private void radioButton_Enter(object sender, EventArgs e)
        {
            radioButton1.ForeColor = radioButton2.ForeColor = radioButton3.ForeColor = radioButton4.ForeColor = Color.Black;
        }
        private void radioButton_Leave(object sender, EventArgs e)
        {
            radioButton1.ForeColor = radioButton2.ForeColor = radioButton3.ForeColor = radioButton4.ForeColor = Color.Gray;
        }


        private void textBox_Enter(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if(textBox != null)
            {
                textBox.Text = null;
                textBox.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            textBox1.ForeColor = Color.Gray;
            try
            {
                Data.target_x = Convert.ToDouble(textBox1.Text.Trim('-'));
                if (textBox1.Text[0] == '-') Data.target_x *= -1;
            }
            catch { textBox1.Text = Data.target_x.ToString(); }
        }
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox2.Focus();
        }


        private void textBox2_Leave(object sender, EventArgs e)
        {
            textBox2.ForeColor = Color.Gray;
            try
            {
                Data.target_y = Convert.ToDouble(textBox2.Text.Trim('-'));
                if (textBox2.Text[0] == '-') Data.target_y *= -1;
            }
            catch { textBox2.Text = Data.target_y.ToString(); }
        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox3.Focus();
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            textBox3.ForeColor = Color.Gray;
            try
            {
                Data.target_v = Convert.ToDouble(textBox3.Text.Trim('-'));
                if (textBox3.Text[0] == '-') Data.target_v *= -1;
            }
            catch { textBox3.Text = Data.target_v.ToString(); }
        }

        private void textBox3_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox4.Focus();
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            textBox4.ForeColor = Color.Gray;
            try
            {
                Data.target_K = Convert.ToDouble(textBox4.Text.Trim('-'));
                if (textBox4.Text[0] == '-') Data.target_K *= -1;
            }
            catch { textBox4.Text = Data.target_K.ToString(); }
        }

        private void textBox4_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox5.Focus();
        }

        private void textBox5_Leave(object sender, EventArgs e)
        {
            textBox5.ForeColor = Color.Gray;
            try
            {
                Data.target_fuel_mass = Convert.ToDouble(textBox5.Text.Trim('-'));
                if (textBox5.Text[0] == '-')
                {
                    Data.target_fuel_mass = 0;
                    textBox5.Text = "0";
                }
            }
            catch { textBox5.Text = Data.target_fuel_mass.ToString(); }
        }

        private void textBox5_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox6.Focus();
        }

        private void textBox6_Leave(object sender, EventArgs e)
        {
            textBox6.ForeColor = Color.Gray;
            try
            {
                Data.target_critical_fuel_mass = Convert.ToDouble(textBox6.Text.Trim('-'));
                if (textBox6.Text[0] == '-')
                {
                    Data.target_critical_fuel_mass = 0;
                    textBox6.Text = "0";
                }
            }
            catch { textBox6.Text = Data.target_critical_fuel_mass.ToString(); ; }
        }

        private void textBox6_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox7.Focus();
        }

        private void textBox7_Leave(object sender, EventArgs e)
        {
            textBox7.ForeColor = Color.Gray;
            try
            {
                Data.target_mass_flow_of_fuel = Convert.ToDouble(textBox7.Text);
                if (textBox7.Text[0] == '-')
                {
                    Data.target_mass_flow_of_fuel = 0;
                    textBox7.Text = "0";
                }
            }
            catch { textBox7.Text = Data.target_mass_flow_of_fuel.ToString(); ; }
        }

        private void textBox7_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox8.Focus();
        }

        private void textBox8_Leave(object sender, EventArgs e)
        {
            textBox8.ForeColor = Color.Gray;
            try
            {
                Data.target_Cx = Convert.ToDouble(textBox8.Text.Trim('-'));
                if (textBox8.Text[0] == '-') Data.target_Cx *= -1;
            }
            catch { textBox8.Text = Data.target_Cx.ToString(); }
        }

        private void textBox8_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox9.Focus();
        }

        private void textBox9_Leave(object sender, EventArgs e)
        {
            textBox9.ForeColor = Color.Gray;
            try
            {
                Data.target_S = Convert.ToDouble(textBox9.Text.Trim('-'));
                if (textBox9.Text[0] == '-') Data.target_S *= -1;
            }
            catch { textBox9.Text = Data.target_S.ToString(); }
        }

        private void textBox9_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) button1.Focus();
        }

        private void textBox10_Leave(object sender, EventArgs e)
        {
            textBox10.ForeColor = Color.Gray;
            try
            {
                Data.cp_x = Convert.ToDouble(textBox10.Text.Trim('-'));
                if (textBox10.Text[0] == '-') Data.cp_x *= -1;
            }
            catch { textBox10.Text = Data.cp_x.ToString(); }
        }

        private void textBox10_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox11.Focus();
        }

        private void textBox11_Leave(object sender, EventArgs e)
        {
            textBox11.ForeColor = Color.Gray;
            try
            {
                Data.cp_y = Convert.ToDouble(textBox11.Text.Trim('-'));
                if (textBox11.Text[0] == '-') Data.cp_y *= -1;
            }
            catch { textBox11.Text = Data.cp_y.ToString(); }
        }

        private void textBox11_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox12.Focus();
        }

        private void textBox12_Leave(object sender, EventArgs e)
        {
            textBox12.ForeColor = Color.Gray;
            try
            {
                Data.cp_safety_distance = Convert.ToDouble(textBox12.Text.Trim('-'));
            }
            catch { textBox12.Text = Data.cp_safety_distance.ToString(); }
        }

        private void textBox12_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox13.Focus();
        }

        private void textBox13_Leave(object sender, EventArgs e)
        {
            textBox13.ForeColor = Color.Gray;
            try
            {
                Data.rls_x = Convert.ToDouble(textBox13.Text.Trim('-'));
                if (textBox13.Text[0] == '-') Data.rls_x *= -1;
            }
            catch { textBox13.Text = Data.rls_x.ToString(); }
        }

        private void textBox13_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox14.Focus();
        }

        private void textBox14_Leave(object sender, EventArgs e)
        {
            textBox14.ForeColor = Color.Gray;
            try
            {
                Data.rls_y = Convert.ToDouble(textBox14.Text.Trim('-'));
                if (textBox14.Text[0] == '-') Data.rls_y *= -1;
            }
            catch { textBox14.Text = Data.rls_y.ToString(); }
        }

        private void textBox14_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox15.Focus();
        }

        private void textBox15_Leave(object sender, EventArgs e)
        {
            textBox15.ForeColor = Color.Gray;
            try
            {
                Data.rls_det_range = Convert.ToDouble(textBox15.Text.Trim('-'));
                if (textBox15.Text[0] == '-') Data.rls_det_range *= -1;
            }
            catch { textBox15.Text = Data.rls_det_range.ToString(); }
        }

        private void textBox15_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox16.Focus();
        }

        private void textBox16_Leave(object sender, EventArgs e)
        {
            textBox16.ForeColor = Color.Gray;
            try
            {
                Data.simulator_T0 = Convert.ToDouble(textBox7.Text);
                if (textBox16.Text[0] == '-')
                {
                    Data.simulator_T0 = 0;
                    textBox16.Text = "0";
                }
            }
            catch { textBox16.Text = Data.simulator_T0.ToString(); }
        }

        private void textBox16_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox17.Focus();
        }

        private void textBox17_Leave(object sender, EventArgs e)
        {
            TextBox tmp = sender as TextBox;
            tmp.ForeColor = Color.Gray;
            try
            {
                Data.simulator_Tk = Convert.ToDouble(tmp.Text);
                if (tmp.Text[0] == '-')
                {
                    Data.simulator_Tk = 0;
                    tmp.Text = "0";
                }
            }
            catch { tmp.Text = Data.simulator_Tk.ToString(); }
        }

        private void textBox17_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox18.Focus();
        }

        private void textBox18_Leave(object sender, EventArgs e)
        {
            TextBox tmp = sender as TextBox;
            tmp.ForeColor = Color.Gray;
            try
            {
                Data.simulator_dT = Convert.ToDouble(tmp.Text);
                if (tmp.Text[0] == '-')
                {
                    Data.simulator_dT = 0;
                    tmp.Text = "0";
                }
            }
            catch { tmp.Text = Data.simulator_dT.ToString(); }
        }

        private void textBox18_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox19.Focus();
        }

        private void textBox19_Leave(object sender, EventArgs e)
        {
            TextBox tmp = sender as TextBox;
            tmp.ForeColor = Color.Gray;
            try
            {
                Data.simulator_density = Convert.ToDouble(tmp.Text);
                if (tmp.Text[0] == '-')
                {
                    Data.simulator_density = 0;
                    tmp.Text = "0";
                }
            }
            catch { tmp.Text = Data.simulator_density.ToString(); }
        }

        private void textBox19_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox20.Focus();
        }

        private void textBox20_Leave(object sender, EventArgs e)
        {
            TextBox tmp = sender as TextBox;
            tmp.ForeColor = Color.Gray;
            try
            {
                if (tmp.Text != "")
                {
                    Data.simulator_path = tmp.Text;
                }
                else
                {
                    tmp.Text = Data.simulator_path;
                }
            }
            catch { tmp.Text = Data.simulator_path; }
        }

        private void textBox20_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox21.Focus();
        }

        private void textBox21_Leave(object sender, EventArgs e)
        {
            TextBox tmp = sender as TextBox;
            tmp.ForeColor = Color.Gray;
            try
            {
                Data.cp_TMax = Convert.ToDouble(tmp.Text);
                if (tmp.Text[0] == '-')
                {
                    Data.cp_TMax = 0;
                    tmp.Text = "0";
                }
            }
            catch { tmp.Text = Data.cp_TMax.ToString(); }
        }

        private void textBox21_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox22.Focus();
        }

        private void textBox22_Leave(object sender, EventArgs e)
        {
            TextBox tmp = sender as TextBox;
            tmp.ForeColor = Color.Gray;
            try
            {
                Data.cp_DLose = Convert.ToDouble(tmp.Text);
                if (tmp.Text[0] == '-')
                {
                    Data.cp_DLose = 0;
                    tmp.Text = "0";
                }
            }
            catch { tmp.Text = Data.cp_DLose.ToString(); }
        }

        private void textBox22_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox23.Focus();
        }

        private void textBox23_Leave(object sender, EventArgs e)
        {
            TextBox tmp = sender as TextBox;
            tmp.ForeColor = Color.Gray;
            try
            {
                Data.cp_PMin = Convert.ToDouble(tmp.Text);
                if (tmp.Text[0] == '-')
                {
                    Data.cp_PMin = 0;
                    tmp.Text = "0";
                }
            }
            catch { tmp.Text = Data.cp_PMin.ToString(); }
        }

        private void textBox23_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox24.Focus();
        }

        private void textBox24_Leave(object sender, EventArgs e)
        {
            TextBox tmp = sender as TextBox;
            tmp.ForeColor = Color.Gray;
            try
            {
                Data.cp_PMax = Convert.ToDouble(tmp.Text);
                if (tmp.Text[0] == '-')
                {
                    Data.cp_PMax = 0;
                    tmp.Text = "0";
                }
            }
            catch { tmp.Text = Data.cp_PMax.ToString(); }
        }

        private void textBox24_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBox25.Focus();
        }

        private void textBox25_Leave(object sender, EventArgs e)
        {
            TextBox tmp = sender as TextBox;
            tmp.ForeColor = Color.Gray;
            try
            {
                Data.cp_PReq = Convert.ToDouble(tmp.Text);
                if (tmp.Text[0] == '-')
                {
                    Data.cp_PReq = 0;
                    tmp.Text = "0";
                }
            }
            catch { tmp.Text = Data.cp_PReq.ToString(); }
        }

        private void textBox25_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) button2.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if ((Data.Count_Target == Simulator.NUMBER) && (Data.Count_CP_Missiles == CommandPost.NUMBER))
                {
                    MessageBox.Show("Все цели и ракеты уже созданы");
                    button2.Focus();
                    return;
                }
                switch (Data.target_type)
                {
                    case Type.aircraft:
                        Data.targets.Add(
                            new Aircraft(
                                Data.target_x, 
                                Data.target_y, 
                                Data.target_K, 
                                Data.target_v,
                                Data.target_Cx, 
                                Data.target_S,
                                Data.target_fuel_mass,
                                Data.target_critical_fuel_mass,
                                Data.target_mass_flow_of_fuel,
                                Data.simulator_T0
                                ));
                        dataGridView1.Rows[Data.Count_Target + Data.Count_CP_Missiles].Cells[0].Value = "Самолет";
                        Data.Count_Target++;
                        break;
                    case Type.missile:
                        Data.targets.Add(
                            new Missile(
                                Data.target_x,
                                Data.target_y, 
                                Data.target_K,
                                Data.target_v,
                                Data.target_Cx,
                                Data.target_S, 
                                Data.target_fuel_mass,
                                Data.target_critical_fuel_mass,
                                Data.target_mass_flow_of_fuel,
                                Data.simulator_T0
                                ));
                        dataGridView1.Rows[Data.Count_Target + Data.Count_CP_Missiles].Cells[0].Value = "Ракета";
                        Data.Count_Target++;
                        break;
                    case Type.sammissile:
                        Data.missiles.Add(
                            new SAMMissile(
                                Data.target_x,
                                Data.target_y, 
                                Data.target_K, 
                                Data.target_v, 
                                Data.target_Cx,
                                Data.target_S, 
                                Data.target_fuel_mass, 
                                Data.target_critical_fuel_mass, 
                                Data.target_mass_flow_of_fuel, 
                                Data.simulator_T0, 
                                Data.cp_TMax, 
                                Data.cp_DLose,
                                Data.cp_PMin,
                                Data.cp_PMax,
                                Data.cp_PReq
                                ));
                        dataGridView1.Rows[Data.Count_Target + Data.Count_CP_Missiles].Cells[0].Value = "SAMMissile";
                        Data.Count_CP_Missiles++;
                        break;
                    default:
                        MessageBox.Show("Цель не создана");
                        return;
                }
                dataGridView1.Rows[Data.Count_Target + Data.Count_CP_Missiles - 1].Cells[1].Value = Data.target_x;
                dataGridView1.Rows[Data.Count_Target + Data.Count_CP_Missiles - 1].Cells[2].Value = Data.target_y;
                dataGridView1.Rows[Data.Count_Target + Data.Count_CP_Missiles - 1].Cells[3].Value = Data.target_v;
                dataGridView1.Rows[Data.Count_Target + Data.Count_CP_Missiles - 1].Cells[4].Value = Convertion.RadDegrees(Data.target_K);
                dataGridView1.Rows[Data.Count_Target + Data.Count_CP_Missiles - 1].Cells[5].Value = Data.target_fuel_mass;
                dataGridView1.Rows[Data.Count_Target + Data.Count_CP_Missiles - 1].Cells[6].Value = Data.target_critical_fuel_mass;
                dataGridView1.Rows[Data.Count_Target + Data.Count_CP_Missiles - 1].Cells[7].Value = Data.target_mass_flow_of_fuel;
                dataGridView1.Rows[Data.Count_Target + Data.Count_CP_Missiles - 1].Cells[8].Value = Data.target_Cx;
                dataGridView1.Rows[Data.Count_Target + Data.Count_CP_Missiles - 1].Cells[9].Value = Data.target_S;

                if ((Data.Count_Target == Simulator.NUMBER) && (Data.Count_CP_Missiles == CommandPost.NUMBER)) button2.Focus();
            }
            catch
            {
                MessageBox.Show("Что-то не так...");
                return;
            }
        }

        Thread thread; //Объявление потока
        private void button2_Click(object sender, EventArgs e)
        {
            if ((Data.Count_Target != Simulator.NUMBER) || (Data.Count_CP_Missiles != CommandPost.NUMBER))
            {
                MessageBox.Show("Вы не создали все цели!");
                return;
            }
            RLS my_RLC = new RLS(Data.rls_x,
                                 Data.rls_y,
                                 Data.rls_det_range,
                                 Data.simulator_T0);

            CommandPost my_CP = new CommandPost(Data.cp_x,
                                                Data.cp_y,
                                                Data.cp_safety_distance);

            Simulator simulator = new Simulator(ref my_RLC,
                                                ref my_CP,
                                                Data.simulator_T0,
                                                Data.simulator_Tk,
                                                Data.simulator_dT,
                                                Data.simulator_density,
                                                Data.simulator_path,
                                                Draw);

            foreach (Target target in Data.targets)
            {
                simulator.AppendTarget(target);
            }
            foreach (SAMMissile missile in Data.missiles)
            {
                simulator.AppendTarget(missile);
            }

            thread = new Thread(() => simulator.Run());
            thread.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (thread != null) thread.Abort();
            Application.Exit();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            button6.Enabled = false;
            WorkingWithDB.selected_table = listBox2.SelectedItem.ToString();
            WorkingWithDB.ShowColumnsInListBox(WorkingWithDB.selected_table, listBox3);
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            WorkingWithDB.selected_column = listBox3.SelectedItem.ToString();
            button6.Enabled = true;
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkedListBox1.GetItemChecked(0))                                         // Уникальные строки
                WorkingWithDB.uniq_strings = "DISTINCT";
            else
                WorkingWithDB.uniq_strings = "";

            if (!checkedListBox1.GetItemChecked(1))                                         // Упорядочивание строк в выборке;
                WorkingWithDB.ordering_string = "";
            else
                radioButton5.Enabled = radioButton6.Enabled = true;

            if (!checkedListBox1.GetItemChecked(2))                                         // Сравнение
                WorkingWithDB.comparison = "";
            else
                textBox29.Enabled = true;

            if (!checkedListBox1.GetItemChecked(3))                                         // Попадание в диапазон
                WorkingWithDB.belonging_to_range = "";
            else
                textBox31.Enabled = true;

            if (!checkedListBox1.GetItemChecked(4))                                         // Соответствие шаблону
                WorkingWithDB.pattern_matching = "";
            else
                textBox30.Enabled = true;

            WorkingWithDB.where = (checkedListBox1.GetItemChecked(1)) || (checkedListBox1.GetItemChecked(2)) || (checkedListBox1.GetItemChecked(3)) || (checkedListBox1.GetItemChecked(4))
                ? "WHERE"
                : "";
        }

        private void textBox29_TextChanged(object sender, EventArgs e)
        {
            string temp = textBox29.Text;
            if (!(temp.Contains("=") || temp.Contains(">") || temp.Contains("<") || temp.Contains(">=") || temp.Contains("<=") || temp.Contains("<>")))
                return;
            WorkingWithDB.comparison = $"{WorkingWithDB.selected_column}{temp}";
        }

        private void textBox31_TextChanged(object sender, EventArgs e)
        {
            WorkingWithDB.begin = textBox31.Text;
            textBox32.Enabled = true;
        }

        private void textBox32_TextChanged(object sender, EventArgs e)
        {
            WorkingWithDB.end = textBox32.Text;
            WorkingWithDB.belonging_to_range = $"{WorkingWithDB.selected_column} BETWEEN '{WorkingWithDB.begin}' AND '{WorkingWithDB.end}'";
        }

        private void textBox30_TextChanged(object sender, EventArgs e)
        {
            string temp = textBox30.Text;
            if (!(temp.Contains("%") || temp.Contains("_") || temp.Contains("[]") || temp.Contains("[^]")))
                return;
            WorkingWithDB.pattern_matching = $"{WorkingWithDB.selected_column} LIKE '{temp}'";
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            WorkingWithDB.ordering_string = $"ORDER BY {WorkingWithDB.selected_column} ASC";
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            WorkingWithDB.ordering_string = $"ORDER BY {WorkingWithDB.selected_column} DESC";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            WorkingWithDB.SelectRequestToListBox(listBox1);
//            MessageBox.Show(WorkingWithDB.selected_table + "\n" + WorkingWithDB.selected_column + "\n" + WorkingWithDB.uniq_strings + "\n" + WorkingWithDB.where + "\n" + WorkingWithDB.comparison + "\n" + WorkingWithDB.pattern_matching + "\n" + WorkingWithDB.ordering_string);
            WorkingWithDB.ClearRequestsData();
            button6.Enabled = false;
        }
    }
}