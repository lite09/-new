using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace метки
{
    public partial class Form1 : Form
    {

        bool on_handl = true;
        List<ToolStripButton> btn_i = new List<ToolStripButton>();
        List<ToolStripSeparator> sp_i = new List<ToolStripSeparator>();
        List<IntPtr> hw_i = new List<IntPtr>();
        List<String> img_i = new List<String>();

        int type = 0;       //  0 - 5: легковые, грузовые, спецтех, мото, кросы, сто 
        int index_i = 0;


        int height = Convert.ToInt32(SystemInformation.VirtualScreen.Height.ToString());
        int width = Convert.ToInt32(SystemInformation.VirtualScreen.Width.ToString());

        int x_0, x_1, y_0, y_1;
        int img_index = 0, color_index = 0;

        const int link_h = 35;
        const int link_w = 277;


        const int head = 70;
        const int head_menu = 70;
        const int margin_top = 70;
        const int margin_left = 70;
        const int margin_bottom = 77;
        const int menu_bottom = 45;
        const int search_h = 70;
        const int search_w = 270;
        const int btn_h = 29;
        const int btn_w = 97;
        const int btn_lang = 27;
        const int color_lb_h = 17;
        const int color_size = 5;

        int row, col;

        Form mes        = new Form();
        Form form_color = new Form();
        Form f          = new Form();

        List<Ls> ls         = new List<Ls>();
        List<Ls> ls_truck   = new List<Ls>();
        List<Ls> ls_sts     = new List<Ls>();
        List<Ls> ls_moto    = new List<Ls>();
        List<Ls> ls_cross   = new List<Ls>();
        List<Ls> ls_special = new List<Ls>();

        LinkLabel[] l_a;
        String name_label;
        String cur_path = System.IO.Directory.GetCurrentDirectory();
        Color [] colors = new Color[color_size * 5];

        Button bt7 = new Button();
        Button[] bt = new Button[7];
        Button[] bt_menu = new Button[7];

        ToolStripContainer tlSC = new ToolStripContainer();
        ToolStrip tlS = new ToolStrip();
        Label label3 = new Label();
        Label [] color_lb = new Label[25];

        void iClick(object e, EventArgs ik, int hw)
        {
            //           System.Threading.Thread.Sleep(70);
            SetForegroundWindow((IntPtr)hw);
            //           System.Threading.Thread.Sleep(70);
            ShowWindow(hw, 3);
        }


        // ---------------------------------------------------------------------------------------------------------------- hook

        private const int WH_KEYBOARD_LL = 13;//Keyboard hook;

        [System.Runtime.InteropServices.StructLayout(
                System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
        }


        private LowLevelKeyboardProcDelegate l_m;
        private LowLevelKeyboardProcDelegate m_callback;
        private LowLevelKeyboardProcDelegate m_callback_7;

        // --- переменные для разблокировки клавиатуры
        private IntPtr m_l_hHook;
        private IntPtr m_hHook;
        private IntPtr m_hHook_7;

        public IntPtr l_mouse(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                if (objKeyInfo.key == Keys.LButton)
                    MessageBox.Show("clack");

            }
            return CallNextHookEx(m_hHook, nCode, wParam, lParam);
        }



        // --- Захват winkey
        public IntPtr LowLevelKeyboardHookProc_win(
                        int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                if (objKeyInfo.key == Keys.RWin || objKeyInfo.key == Keys.LWin)
                    return (IntPtr)1;

            }
            return CallNextHookEx(m_hHook, nCode, wParam, lParam);
        }



        // --- Блокировка control, alt, shift -----------------------------------------------------------------------------------------------
        public IntPtr LowLevelKeyboardHookProc_control_alt_del(int nCode,
                        IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                if (objKeyInfo.key == Keys.LControlKey ||
                    objKeyInfo.key == Keys.RControlKey ||
                    objKeyInfo.key == Keys.Control ||
                    objKeyInfo.key == Keys.ControlKey ||
                    objKeyInfo.key == Keys.Alt ||
                    objKeyInfo.key == Keys.LMenu ||
                    //objKeyInfo.key == Keys.Menu || 
                    objKeyInfo.key == Keys.RMenu ||
                    objKeyInfo.key == Keys.LShiftKey ||
                    objKeyInfo.key == Keys.RShiftKey)

                {
                    //   return (IntPtr)1;
                    lang();
                    return CallNextHookEx(m_hHook_7, nCode, wParam, lParam);

                }
            }
            return CallNextHookEx(m_hHook_7, nCode, wParam, lParam);
        }
        // --- Блокировка control, alt, shift -----------------------------------------------------------------------------------------------

        private delegate IntPtr LowLevelKeyboardProcDelegate(int nCode, IntPtr wParam, IntPtr lParam);


        // --- настройки хука ---------

        private void set_hook()
        {
            int hWnd = FindWindow("Shell_TrayWnd", "");
            IntPtr startWnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Button", "Пуск");

            ShowWindow(hWnd, 0);
            ShowWindow((int)startWnd, 0);


            l_m = l_mouse;
            m_callback = LowLevelKeyboardHookProc_win;
            m_callback_7 = LowLevelKeyboardHookProc_control_alt_del;


            m_l_hHook = SetWindowsHookEx(WH_KEYBOARD_LL, l_m, GetModuleHandle(IntPtr.Zero), 0);
            m_hHook = SetWindowsHookEx(WH_KEYBOARD_LL, m_callback, GetModuleHandle(IntPtr.Zero), 0);
            m_hHook_7 = SetWindowsHookEx(WH_KEYBOARD_LL, m_callback_7, GetModuleHandle(IntPtr.Zero), 0);
        }

        private void un_hook()
        {
            int hWnd = FindWindow("Shell_TrayWnd", "");
            IntPtr startWnd = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Button", "Пуск");

            ShowWindow(hWnd, 1);
            ShowWindow((int)startWnd, 1);

            UnhookWindowsHookEx(m_l_hHook);
            UnhookWindowsHookEx(m_hHook);
            UnhookWindowsHookEx(m_hHook_7);
        }


        // ---------------------------------------------------------------------------------------------------------------- hook



        public void init()
        {

            x_0 = margin_left;
            x_1 = width - margin_left;
            y_0 = head + head_menu + margin_top;
            y_1 = height - menu_bottom - margin_bottom;

            row = (y_1 - y_0) / link_h;
            col = (x_1 - x_0) / link_w;


            WindowState = FormWindowState.Maximized;

            colors = new Color[] { Color.PaleGreen, Color.Aqua, Color.Blue, Color.BlueViolet, Color.Cyan, Color.Green,
                Color.Yellow, Color.Silver, Color.Magenta, Color.Gray, Color.Gold, Color.Brown, Color.Lime, Color.SkyBlue,
                Color.Orange, Color.Pink, Color.Indigo, Color.DarkBlue, Color.Red, Color.Coral, Color.Aquamarine, Color.Black,
                Color.Violet, Color.White, Color.LightGoldenrodYellow };

            // --- Form task bar ------------------------------------------------------------------------------------------------------------------------
            f.FormBorderStyle = FormBorderStyle.None;
            f.ControlBox = false;
            f.Show();
            f.AutoSize = false;
            //   f.Size = new Size(width - label1.Size.Width - btn_lang, btn_h);
            f.Size = new Size(width, btn_h);
            f.Location = new Point(0, height - btn_h);
            f.TopMost = true;

            // ---- labels --------------------------------------------------------------------------------------------------------------------

            int colr = 97;
            int colr_i = 241;

            label1.Size = new Size(btn_w, (btn_h / 2) + 2);
            //   label1.Location = new Point(width - label1.Size.Width, height - btn_h);
            label1.Location = new Point(width - label1.Size.Width, 0);
            label1.ForeColor = Color.FromArgb(255, colr, colr, colr);
            label1.BackColor = Color.FromArgb(255, colr_i, colr_i, colr_i);
            f.Controls.Add(label1);


            /*   label1.AutoSize = false;
               label1.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular);
               label1.TextAlign = ContentAlignment.MiddleRight;*/


            label2.Size = new Size(btn_w, btn_h / 2);
            label2.Location = new Point(width - label1.Size.Width, btn_h / 2);
            label2.ForeColor = Color.FromArgb(255, colr, colr, colr);
            label2.BackColor = Color.FromArgb(255, colr_i, colr_i, colr_i);
            f.Controls.Add(label2);


            label3.Size = new Size(btn_lang, btn_h);
            label3.Location = new Point(width - label1.Size.Width - btn_lang, 0);
            label3.ForeColor = Color.FromArgb(255, 107, 107, 107);
            label3.BackColor = Color.FromArgb(255, colr_i, colr_i, colr_i);
            label3.AutoSize = false;
            label3.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular);
            label3.TextAlign = ContentAlignment.MiddleRight;
            label3.Text = "RU";
            label3.Click += (s, i) =>
            {
                if (label3.Text == "EN")
                {
                    //   CultureInfo.CurrentCulture = new CultureInfo("ru-RU", false);
                    InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new System.Globalization.CultureInfo("ru-RU"));
                    label3.Text = "RU";
                }
                else
                {
                    //  CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
                    InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new System.Globalization.CultureInfo("en-US"));
                    label3.Text = "EN";
                }
            };
            f.Controls.Add(label3);

            // ---- labels --------------------------------------------------------------------------------------------------------------------


            //   textBox1.Location   = new Point( margin_left - (search_w / 2) + (x_1 - x_0) / 2, margin_top + head / 2 );
            textBox1.Visible = true;
            textBox1.Location = new Point(27 + margin_left + btn_w * 7, margin_top + head / 2);
            textBox1.Size = new Size(search_w, search_h);
            textBox1.BackColor = Color.FromArgb(255, 227, 227, 227);
            textBox1.ForeColor = Color.FromArgb(255, 187, 187, 187);


            mes.BackColor = Color.Aqua;
            mes.FormBorderStyle = FormBorderStyle.Fixed3D;
            mes.ControlBox = false;
            mes.Show();
            mes.Visible = false;

            // --- цвет текста -----------------------------------------------------------------------------------------------------------------------

            form_color.Show();
            form_color.AutoSize = false;
            form_color.Location = new Point(width - 7 - btn_w / 2 - (color_lb_h * color_size ), height - (int)(btn_h * 3) - (color_lb_h * 5));
            form_color.FormBorderStyle = FormBorderStyle.None;
            form_color.Size     = new Size((color_lb_h * color_size), (color_lb_h * color_size));
            form_color.BackColor = Color.Aqua;
            form_color.ControlBox = false;
            form_color.Visible = false;

            int lb_color_index = 0, lb_h = 0, lb_w = 0;
            for (int i = 0; i < color_size; i++)
            {
                for (int ki = 0; ki < color_size; ki++)
                {

                    color_lb[lb_color_index] = new Label();
                    color_lb[lb_color_index].Location = new Point(lb_w, lb_h);
                    color_lb[lb_color_index].Size = new Size(color_lb_h, color_lb_h);
                    color_lb[lb_color_index].Text = "";
                    //   color_lb[lb_color_index].ForeColor = Color.FromArgb(107, 97, 97, 97);
                    color_lb[lb_color_index].BackColor = colors[lb_color_index];

                    color_lb[lb_color_index].MouseClick += (e, ai) =>
                    {
                        form_color.Visible = false;
                        Label time_label = (Label)e;
                        for (int kil = 0; kil < l_a.Count(); kil++) l_a[kil].LinkColor = time_label.BackColor;
                        
                    };

                    color_lb[lb_color_index].FlatStyle = FlatStyle.Popup;

                    form_color.Controls.Add(color_lb[lb_color_index]);
                    lb_w += color_lb_h;

                    lb_color_index++;
                }

                lb_w = 0;
                lb_h += color_lb_h;

            }

            // --- цвет текста -----------------------------------------------------------------------------------------------------------------------

            // --- Меню основное ---------------------------------------------------------------------------------------------------------------------
            int a = 0;
            foreach (Button i in bt) { bt[a] = new Button(); bt[a].Name = "bt" + a; a++; }

            bt[0].Text = "Все";
            bt[1].Text = "Европа";
            bt[2].Text = "США";
            bt[3].Text = "Япония";
            bt[4].Text = "Корея";
            bt[5].Text = "Китай";
            bt[6].Text = "Россия";

            for (int i = 0; i < bt.Count(); i++)
            {
                bt[i].Location = new Point(margin_left + btn_w * i, margin_top + head - Convert.ToInt32(head / 2.43));  // 2.5
                bt[i].Size = new Size(btn_w, btn_h);
                Controls.Add(bt[i]);
            }

            // --- Меню основное ---------------------------------------------------------------------------------------------------------------------
            // --- Меню дополнительное ---------------------------------------------------------------------------------------------------------------------

            a = 0;
            foreach (Button i in bt) { bt_menu[a] = new Button(); bt_menu[a].Name = "bt_menu" + a; a++; }

            bt_menu[0].Text = "Легковые";
            bt_menu[1].Text = "Грузовые";
            bt_menu[2].Text = "Спецтехника";
            bt_menu[3].Text = "Мото";
            bt_menu[4].Text = "Кроссы";
            bt_menu[5].Text = "СТО";

            for (int i = 0; i < (bt_menu.Count() - 1); i++)
            {
                bt_menu[i].Location = new Point(margin_left + btn_w / 2 + btn_w * i, margin_top + head + btn_h - Convert.ToInt32(head / 2.7));
                bt_menu[i].Size = new Size(btn_w, btn_h - 7);
                bt_menu[i].FlatStyle = FlatStyle.Popup;
                bt_menu[i].BackColor = Color.FromArgb(197, 207, 207, 207);
                bt_menu[i].FlatAppearance.BorderSize = 0;

                Controls.Add(bt_menu[i]);
            }

            bt_menu[0].Click += (e, l) =>
            {

                for (int i_it = 0; i_it < l_a.Count(); i_it++)
                    l_a[i_it].Visible = false;

                int i_link = 0;
                foreach (Ls i_l in ls) { l_a[i_link].Text = i_l.name; i_link++; }

                type = 0;
                link_prt(ls.Count());
            };

            bt_menu[1].Click += (e, l) =>
            {
                int i_link = 0;

                for (int i_it = 0; i_it < l_a.Count(); i_it++)
                    l_a[i_it].Visible = false;

                foreach (Ls i_l in ls_truck)
                { l_a[i_link].Text = i_l.name; i_link++; }

                type = 1;
                link_prt(ls_truck.Count());
            };

            bt_menu[2].Click += (e, l) =>
            {
                int i_link = 0;

                for (int i_it = 0; i_it < l_a.Count(); i_it++)
                    l_a[i_it].Visible = false;

                foreach (Ls i_l in ls_special)
                { l_a[i_link].Text = i_l.name; i_link++; }

                type = 2;
                link_prt(ls_special.Count());
            };

            bt_menu[3].Click += (e, l) =>
            {
                int i_link = 0;

                for (int i_it = 0; i_it < l_a.Count(); i_it++)
                    l_a[i_it].Visible = false;

                foreach (Ls i_l in ls_moto)
                { l_a[i_link].Text = i_l.name; i_link++; }

                type = 3;
                link_prt(ls_moto.Count());
            };

            bt_menu[4].Click += (e, l) =>
            {
                int i_link = 0;

                for (int i_it = 0; i_it < l_a.Count(); i_it++)
                    l_a[i_it].Visible = false;

                foreach (Ls i_l in ls_cross)
                { l_a[i_link].Text = i_l.name; i_link++; }

                type = 4;
                link_prt(ls_cross.Count());
            };

            bt_menu[5].Click += (e, l) =>
            {
                int i_link = 0;

                for (int i_it = 0; i_it < l_a.Count(); i_it++)
                    l_a[i_it].Visible = false;

                foreach (Ls i_l in ls_sts)
                { l_a[i_link].Text = i_l.name; i_link++; }

                type = 5;
                link_prt(ls_sts.Count());
            };

            // --- Меню дополнительное ---------------------------------------------------------------------------------------------------------------------


// --- фон -----------------------------------------------------------------------------------------------------------------------------------------
            Label lb7 = new Label();
            lb7.Location = new Point(width - 7 - btn_w / 2, height - btn_h * 5);
            lb7.Size = new Size(btn_w / 2, 27);
            lb7.Text = "фон";
            lb7.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular);
            lb7.TextAlign = ContentAlignment.MiddleCenter;
            lb7.ForeColor = Color.FromArgb(107, 97, 97, 97);
            //  bt7.BackColor = Color.Transparent;
            lb7.BackColor = Color.FromArgb(17, 255, 90, 255);

            lb7.MouseEnter += (e, ai) =>
            {
                lb7.ForeColor = Color.FromArgb(207, 47, 47, 47);
                lb7.BackColor = Color.FromArgb(107, 255, 90, 255);
            };

            lb7.MouseLeave += (e, ai) =>
            {
                lb7.ForeColor = Color.FromArgb(107, 97, 97, 97);
                lb7.BackColor = Color.FromArgb(17, 255, 90, 255);
            };

            lb7.MouseClick += (e, ai) =>
            {
                if (img_i.Count() <= img_index) img_index = 0;

                this.BackgroundImage = Image.FromFile(cur_path + "\\img\\" + img_i[img_index]);
                img_index++;
            };

            lb7.FlatStyle = FlatStyle.Popup;

            Controls.Add(lb7);
// --- фон -----------------------------------------------------------------------------------------------------------------------------------------

// --- цвет ----------------------------------------------------------------------------------------------------------------------------------------
            Label lb7_link_color = new Label();
            lb7_link_color.Location = new Point(width - 7 - btn_w / 2, height - btn_h * 4);
            lb7_link_color.Size = new Size(btn_w / 2, 27);
            lb7_link_color.Text = "цвет";
            lb7_link_color.Font = new Font("Microsoft Sans Serif", 9, FontStyle.Regular);
            lb7_link_color.TextAlign = ContentAlignment.MiddleCenter;
            lb7_link_color.ForeColor = Color.FromArgb(107, 97, 97, 97);
            //  bt7.BackColor = Color.Transparent;
            lb7_link_color.BackColor = Color.FromArgb(17, 255, 90, 255);

            lb7_link_color.MouseEnter += (e, ai) =>
            {
                lb7_link_color.ForeColor = Color.FromArgb(207, 47, 47, 47);
                lb7_link_color.BackColor = Color.FromArgb(107, 255, 90, 255);
            };

            lb7_link_color.MouseLeave += (e, ai) =>
            {
                lb7_link_color.ForeColor = Color.FromArgb(107, 97, 97, 97);
                lb7_link_color.BackColor = Color.FromArgb(17, 255, 90, 255);
            };

            lb7_link_color.MouseClick += (e, ai) => form_color.Visible = true;

            lb7_link_color.FlatStyle = FlatStyle.Popup;


            Controls.Add(lb7_link_color);
// --- цвет ----------------------------------------------------------------------------------------------------------------------------------------


            bt[0].Click += (e, ia) =>
            {

                for (int i_l = 0; i_l < l_a.Count(); i_l++) l_a[i_l].Visible = false;

                int i_link = 0;
                foreach (Ls i_l in ls) { l_a[i_link].Text = i_l.name; i_link++; }

                type = 0;
                link_prt(ls.Count());
            };

            bt[1].Click += (e, ia) =>
            {
                var select = from us in ls
                             where us.location == bt[1].Text.ToString()
                             select us;


                for (int i_l = 0; i_l < l_a.Count(); i_l++) l_a[i_l].Visible = false;

                int i_am = 0;
                foreach (Ls var in select)
                {
                    l_a[i_am].Text = var.name;
                    i_am++;
                }

                type = 0;
                link_prt(select.Count());
            };

            bt[2].Click += (e, ia) =>
            {
                var select = from us in ls
                             where us.location == bt[2].Text.ToString()
                             select us;


                for (int i_l = 0; i_l < l_a.Count(); i_l++) l_a[i_l].Visible = false;

                int i_am = 0;
                foreach (Ls var in select)
                {
                    l_a[i_am].Text = var.name;
                    i_am++;
                }

                type = 0;
                link_prt(select.Count());
            };

            bt[3].Click += (e, ia) =>
            {
                var select = from us in ls
                             where us.location == bt[3].Text.ToString()
                             select us;


                for (int i_l = 0; i_l < l_a.Count(); i_l++) l_a[i_l].Visible = false;

                int i_am = 0;
                foreach (Ls var in select)
                {
                    l_a[i_am].Text = var.name;
                    i_am++;
                }

                type = 0;
                link_prt(select.Count());
            };

            bt[4].Click += (e, ia) =>
            {
                var select = from us in ls
                             where us.location == bt[4].Text.ToString()
                             select us;


                for (int i_l = 0; i_l < l_a.Count(); i_l++) l_a[i_l].Visible = false;

                int i_am = 0;
                foreach (Ls var in select)
                {
                    l_a[i_am].Text = var.name;
                    i_am++;
                }

                type = 0;
                link_prt(select.Count());
            };

            bt[5].Click += (e, ia) =>
            {
                var select = from us in ls
                             where us.location == bt[5].Text.ToString()
                             select us;


                for (int i_l = 0; i_l < l_a.Count(); i_l++) l_a[i_l].Visible = false;

                int i_am = 0;
                foreach (Ls var in select)
                {
                    l_a[i_am].Text = var.name;
                    i_am++;
                }

                type = 0;
                link_prt(select.Count());
            };

            bt[6].Click += (e, ia) =>
            {
                var select = from us in ls
                             where us.location == bt[6].Text.ToString()
                             select us;


                for (int i_l = 0; i_l < l_a.Count(); i_l++) l_a[i_l].Visible = false;

                int i_am = 0;
                foreach (Ls var in select)
                {
                    l_a[i_am].Text = var.name;
                    i_am++;
                }

                type = 0;
                link_prt(select.Count());
            };

            //  bt7.Click += (e, ia) => Close();




            //      tlSC.Location = new Point(0, height - btn_h);
            tlSC.Location = new Point(0, 0);
            tlSC.Size = new Size(width - label1.Size.Width - btn_lang, btn_h);

            tlS.AutoSize = false;
            //      tlS.Location = new Point(0, height - btn_h);
            tlS.Location = new Point(0, 0);
            tlS.Size = new Size(width - label1.Size.Width - btn_lang, btn_h);



            ToolStripButton btn_menu = new ToolStripButton();

            btn_menu.Text = "Выход";
            btn_menu.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btn_menu.Size = new Size(btn_w, btn_h);
            btn_menu.AutoToolTip = false;
            btn_menu.AutoSize = false;
            btn_menu.Click += (e, i) => { un_hook(); /* ExitWindowsEx(0, 0); */ Close(); };
            btn_menu.MouseEnter += (e, i) => tlSC.Focus();
            btn_menu.BackColor = Color.FloralWhite;
            btn_menu.Font = new Font("Verdana", 10, FontStyle.Regular);
            tlS.Items.Add(btn_menu);

            ToolStripButton btn_time = new ToolStripButton();

            ToolStripSeparator sp_menu = new ToolStripSeparator();
            sp_menu.AutoSize = false;
            sp_menu.Size = new Size(1, btn_h);
            tlS.Items.Add(sp_menu);


            /*  bt7.Location = new Point(margin_left, height - btn_h);
              bt7.Size = new Size(btn_w, btn_h);
              bt7.Text = "Выход"; */
            tlSC.TopToolStripPanel.Controls.Add(tlS);
            f.Controls.Add(tlSC);
        }

        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 1000;
            timer1.Start();
            timer2.Interval = 1000;
            timer2.Start();
            timer3.Interval = 2700;
            timer3.Start();
        }

        class Sun_Ls
        {
            public Ls[] i { get; set; }
        }


        class Ls
        {
            public string name { get; set; }
            public string location { get; set; }
            public Type_i[] type { get; set; }
            public Vin [] vins { get; set; }

            public options[] lines { get; set; }

            public bool chek(String templ)
            {

                foreach (options i in lines) { if (i.name.ToLower().StartsWith(templ.ToLower())) return true; }

                return false;
            }
            public bool chek_vin(String templ)
            {

                foreach (Vin i in vins) { if (i.vin.ToLower().StartsWith(templ.ToLower())) return true; }

                return false;
            }

        };

        class options
        {
            public string name { get; set; }
            public string link { get; set; }
        }

        class Vin
        {
            public string vin { get; set; }
        }


        class Type_i
        {
            public string type { get; set; }
        }


        private void TextBox1_Enter(object sender, EventArgs e)
        {
            timer3.Stop();

            if (textBox1.Text == " Поиск")
            {
                on_handl = false;
                textBox1.Text = "";
                // textBox1.BackColor = Color.Aqua;
                textBox1.ForeColor = Color.FromArgb(207, 0, 0, 0);
            }
        }

        private void TextBox1_Leave(object sender, EventArgs e)
        {
            timer3.Start();

            //textBox1.TextChanged += (sender0, EventArgs) => {};
            on_handl = false;
            textBox1.Text = " Поиск";
            textBox1.ForeColor = Color.FromArgb(255, 187, 187, 187);

        }

        private void print_sel(IEnumerable<Ls> select_vin)
        {
          //  if (select_vin.Count() > 0)
          //  {

                for (int i_l = 0; i_l < l_a.Count(); i_l++) l_a[i_l].Visible = false;

                int i = 0;
                foreach (Ls var in select_vin) { l_a[i].Text = var.name; i++; }

                link_prt(select_vin.Count());

          //      return;
          //  }
        }


        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (!on_handl) { on_handl = true; return; }

            var select = from us in ls
                         where us.name.ToLower().StartsWith(textBox1.Text.ToString().ToLower()) || us.chek(textBox1.Text.ToString())
                         select us;

            for (int i_l = 0; i_l < l_a.Count(); i_l++) l_a[i_l].Visible = false;

            int i = 0;
            foreach (Ls var in select) { l_a[i].Text = var.name; i++; }

            link_prt(select.Count());


            if (select.Count() <= 0)
            {
                //   MessageBox.Show("scan to vin");

                var select_vin = from us in ls
                                 where us.chek_vin(textBox1.Text.ToString())
                                 select us;
                if (select_vin.Count() > 0)
                {

                    type = 0;
                    print_sel(select_vin);

                    return;
                }


                select_vin = from us in ls_truck
                                 where us.chek_vin(textBox1.Text.ToString())
                                 select us;

                if (select_vin.Count() > 0)
                {

                    type = 1;
                    print_sel(select_vin);

                    return;
                }


                select_vin = from us in ls_special
                             where us.chek_vin(textBox1.Text.ToString())
                             select us;

                if (select_vin.Count() > 0)
                {

                    type = 2;
                    print_sel(select_vin);

                    return;
                }


                select_vin = from us in ls_moto
                             where us.chek_vin(textBox1.Text.ToString())
                             select us;

                if (select_vin.Count() > 0)
                {

                    type = 3;
                    print_sel(select_vin);

                    return;
                }


                select_vin = from us in ls_cross
                             where us.chek_vin(textBox1.Text.ToString())
                             select us;

                if (select_vin.Count() > 0)
                {

                    type = 4;
                    print_sel(select_vin);

                    return;
                }


                select_vin = from us in ls_sts
                             where us.chek_vin(textBox1.Text.ToString())
                             select us;

                if (select_vin.Count() > 0)
                {

                    type = 5;
                    print_sel(select_vin);

                    return;
                }
            }
        }



        private void Form1_Activated(object sender, EventArgs e)
        {
            mes.Hide();
            form_color.Visible = false;
            SendToBack();
        }


        public void i()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint
                        | ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true);

            int[] sizes_of_labels = { ls.Count(), ls_truck.Count(), ls_special.Count(), ls_moto.Count(), ls_cross.Count(), ls_sts.Count() };
            int label_size = sizes_of_labels.Max(), index = 0;

            l_a = new LinkLabel[label_size];
          /*  for (int i = 0; i < 777; i++) l_a[i] = new LinkLabel();*/


            for (int i = 0; i < label_size; i++)
            {
                l_a[i] = new LinkLabel();
                l_a[i].Name = "l" + index;
                //l_a[i].LinkArea = new LinkArea(0, 77);
                if (i < ls.Count()) l_a[i].Text = ls[index].name;
                l_a[i].AutoSize = true;
                l_a[i].Font = new Font("Times New Roman", 14, FontStyle.Regular);
                l_a[i].LinkColor = Color.White;
                l_a[i].BackColor = Color.Transparent;

                l_a[i].Click += (sender, EventArgs) => {


                    int a = Cursor.Position.X, b = Cursor.Position.Y;
                    mes.Location = new Point(a, b);


                    string[] str = sender.ToString().Split(new[] { "Text:" }, StringSplitOptions.None);
                    name_label = str[1].Trim(' ');
                    // MessageBox.Show(name_label);

                    var select = from us in ls_truck
                             where us.name == ""
                             select us;

                    switch (type)
                    {

                        case 1:

                            select = from us in ls_truck
                                        where us.name == name_label
                                        select us;
                            break;

                        case 2:

                            select = from us in ls_special
                                         where us.name == name_label
                                         select us;
                            break;

                        case 3:

                            select = from us in ls_moto
                                         where us.name == name_label
                                         select us;
                            break;

                        case 4:

                            select = from us in ls_cross
                                         where us.name == name_label
                                         select us;
                            break;

                        case 5:

                            select = from us in ls_sts
                                         where us.name == name_label
                                         select us;
                            break;

                        default:
                            select = from us in ls
                                         where us.name == name_label
                                         select us;
                            break;
                    }

                    /*    foreach (Ls var in select)
                        {*/

                    Ls var = select.First();
                    int height = 27, width = 257, head = 27, bottom = 9;
                    //   height = height * var.lines.Count() + head;

                    mes.Size = new Size(width, height * var.lines.Count() + head + bottom);
                    mes.Controls.Clear();

                    Label lb = new Label();
                    lb.Name = "lb";
                    lb.TextAlign = ContentAlignment.MiddleCenter;
                    lb.BorderStyle = BorderStyle.FixedSingle;
                    lb.BackColor = Color.AliceBlue;
                    lb.Text = var.name.ToString();
                    lb.Font = new Font("Times New Roman", 15, FontStyle.Regular);
                    lb.Location = new Point(0, 0);
                    lb.Size = new Size(width, 27);
                    lb.ForeColor = Color.BlueViolet;
                    mes.Controls.Add(lb);


                    for (int i_local = 0; i_local < var.lines.Count(); i_local++)
                    {
                        //    MessageBox.Show(var.lines[i_local].name.ToString());

                        Button btn = new Button();
                        btn.Name = i_local.ToString();
                        btn.Text = var.lines[i_local].name.ToString();
                        btn.Font = new Font("Times New Roman", 14, FontStyle.Regular);
                        btn.Location = new Point(0, (i_local * height + head));
                        btn.AutoSize = true;
                        btn.Size = new Size(width - 5, height);
                        btn.ForeColor = Color.BlueViolet;


                        btn.Click += (sender_i, EventArgs_i) => {

                            //   timer3.Start();
                            //   timer3.Stop();
                            var selt = from us in ls_moto
                                   where us.name == ""
                                   select us;

                            switch (type)
                            {
                                case 0:
                                    selt = from us in ls
                                               where us.name == name_label
                                               select us;
                                    break;
                                case 1:
                                    selt = from us in ls_truck
                                               where us.name == name_label
                                               select us;
                                    break;

                                case 2:
                                    selt = from us in ls_special
                                               where us.name == name_label
                                               select us;
                                    break;

                                case 3:
                                    selt = from us in ls_moto
                                               where us.name == name_label
                                               select us;
                                    break;

                                case 4:
                                    selt = from us in ls_cross
                                               where us.name == name_label
                                               select us;
                                    break;

                                case 5:
                                    selt = from us in ls_sts
                                               where us.name == name_label
                                               select us;
                                    break;


                            }

                            Ls link = new Ls();

                            link = selt.FirstOrDefault();

                            System.Diagnostics.Process p = new System.Diagnostics.Process();
                            p.EnableRaisingEvents = true;
                            p.StartInfo.FileName = link.lines[Convert.ToInt32(btn.Name)].link;
                            p.Start();


                            System.Threading.Thread.Sleep(470);

                            hw_i.Add(p.Handle);

// --- возврат окна в полноэкранный режим
                            ShowWindow(p.MainWindowHandle.ToInt32(), 3);

                            p.Exited += (e, i0) =>
                            {
                                int handl = p.Handle.ToInt32();
                                for (int i_k = 0; i_k < hw_i.Count(); i_k++)
                                    if (hw_i[i_k] == (IntPtr)handl)
                                    {
                                        tlS.Invoke((MethodInvoker)(() => tlS.Items.Remove(btn_i[i_k])));
                                        tlS.Invoke((MethodInvoker)(() => tlS.Items.Remove(sp_i[i_k])));

                                        hw_i.RemoveAt(i_k);
                                        btn_i.RemoveAt(i_k);
                                        sp_i.RemoveAt(i_k);

                                        index_i--;
                                    }
                            };


                            btn_i.Add(new ToolStripButton());
                            btn_i[index_i].Text = link.lines[Convert.ToInt32(btn.Name)].name;
                            //   btn_i[index_i].Text = "h " + p.Handle.ToString() + " wh " + p.MainWindowHandle.ToString();
                            btn_i[index_i].DisplayStyle = ToolStripItemDisplayStyle.Text;
                            btn_i[index_i].Size = new Size(btn_w + 70, btn_h);
                            btn_i[index_i].AutoToolTip = false;
                            btn_i[index_i].AutoSize = false;
                            btn_i[index_i].BackColor = Color.FloralWhite;
                            btn_i[index_i].Font = new Font("Verdana", 9, FontStyle.Regular);
                            tlS.Items.Add(btn_i[index_i]);

                            int hw = p.MainWindowHandle.ToInt32();
                            IntPtr h = p.Handle;
                            btn_i[index_i].Click += (e, ki) => iClick(e, ki, hw);
                            btn_i[index_i].MouseEnter += (e, ki) => tlSC.Focus();


                            tlS.Items.Add(btn_i[index_i]);

                            sp_i.Add(new ToolStripSeparator());
                            sp_i[index_i].AutoSize = false;
                            sp_i[index_i].Size = new Size(1, btn_h);
                            tlS.Items.Add(sp_i[index_i]);


                            index_i++;

                        };

                        mes.Controls.Add(btn);
                    }

                    mes.Show();
                    mes.Activate();
                };
                index++;
            }



            AutoSize = true;

            link_prt(label_size);

        }

        private void link_prt(int label_size)
        {
            int time = 0, balance = 0, index = 0;

            if (label_size > row) { time = label_size / row; balance = label_size - (time * row); }
            else balance = label_size;


            index = 0;
            int label_x = x_0, label_y = y_0;

            for (int i = 0; i <= time; i++)
            {
                for (int j = 0; j < row; j++)
                {

                    if (label_size <= index) break;

                    l_a[index].Location = new Point(label_x, label_y);
                    Controls.Add(l_a[index]);

                    index++;

                    label_y += link_h;
                }

                label_x += link_w;
                label_y = y_0;

            }

            for (int i = 0; i < label_size; i++) l_a[i].Visible = true;
        }

        private int lang()
        {
            String[] s = InputLanguage.CurrentInputLanguage.Culture.Name.Split('-');
            //  MessageBox.Show(InputLanguage.CurrentInputLanguage.Culture.Name);
            if (label3.Text == s[0].ToUpper()) return 1;
            else label3.Text = s[0].ToUpper();

            return 0;
        }

        private void load_img_s(List<String> img_s, String path_file_s)
        {
            DirectoryInfo d = new DirectoryInfo(path_file_s);//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.*");

            foreach (FileInfo file in Files)
                img_s.Add(file.Name);

        }

        private void Form1_MouseHover(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void load_item()
        {
            String conf = File.ReadAllText("conf.txt", Encoding.GetEncoding(1251));

            String reg_type = @"^[\S\s]+type:[\s]*([\S]+)";
            Regex type = new Regex(reg_type);
            Match m_type = type.Match(conf);
            Group g_type = m_type.Groups[1];

            if (m_type.Groups[1].ToString() == "1")
            {


                String data = File.ReadAllText("data\\info.json", Encoding.GetEncoding(1251));
                Sun_Ls info = JsonConvert.DeserializeObject<Sun_Ls>(data);
                foreach (Ls i in info.i) ls.Add(i);

                data = File.ReadAllText("data\\sts.json", Encoding.GetEncoding(1251));
                info = JsonConvert.DeserializeObject<Sun_Ls>(data);
                foreach (Ls i in info.i) ls_sts.Add(i);

                data = File.ReadAllText("data\\moto.json", Encoding.GetEncoding(1251));
                info = JsonConvert.DeserializeObject<Sun_Ls>(data);
                foreach (Ls i in info.i) ls_moto.Add(i);

                data = File.ReadAllText("data\\cross.json", Encoding.GetEncoding(1251));
                info = JsonConvert.DeserializeObject<Sun_Ls>(data);
                foreach (Ls i in info.i) ls_cross.Add(i);

                data = File.ReadAllText("data\\special.json", Encoding.GetEncoding(1251));
                info = JsonConvert.DeserializeObject<Sun_Ls>(data);
                foreach (Ls i in info.i) ls_special.Add(i);

                data = File.ReadAllText("data\\truck.json", Encoding.GetEncoding(1251));
                info = JsonConvert.DeserializeObject<Sun_Ls>(data);
                foreach (Ls i in info.i) ls_truck.Add(i);

            }
            else
            {

                String reg_info = @"^[\S\s]+info:[\s]*([\S]+)";
                String reg_truck = @"^[\S\s]+truck:[\s]*([\S]+)";
                String reg_special = @"^[\S\s]+special:[\s]*([\S]+)";
                String reg_moto = @"^[\S\s]+moto:[\s]*([\S]+)";
                String reg_cross = @"^[\S\s]+cross:[\s]*([\S]+)";
                String reg_sts = @"^[\S\s]+sts:[\s]*([\S]+)";


                Regex i_info = new Regex(reg_info), truck = new Regex(reg_truck), special = new Regex(reg_special),
                        moto = new Regex(reg_moto), cross = new Regex(reg_cross), sts = new Regex(reg_sts);
                Match m_info = i_info.Match(conf), m_truck = truck.Match(conf), m_special = special.Match(conf),
                        m_moto = moto.Match(conf), m_cross = cross.Match(conf), m_sts = sts.Match(conf);
                Group g_info = m_info.Groups[1], g_truck = m_truck.Groups[1], g_specia = m_special.Groups[1],
                        g_moto = m_moto.Groups[1], g_cross = m_cross.Groups[1], g_sts = m_sts.Groups[1];

                WebClient wc = new WebClient();
                reg_info = wc.DownloadString(m_info.Groups[1].ToString());
                reg_truck = wc.DownloadString(m_truck.Groups[1].ToString());
                reg_special = wc.DownloadString(m_special.Groups[1].ToString());
                reg_moto = wc.DownloadString(m_moto.Groups[1].ToString());
                reg_cross = wc.DownloadString(m_cross.Groups[1].ToString());
                reg_sts = wc.DownloadString(m_sts.Groups[1].ToString());

                Sun_Ls info = JsonConvert.DeserializeObject<Sun_Ls>(reg_info);
                foreach (Ls i in info.i) ls.Add(i);

                info = JsonConvert.DeserializeObject<Sun_Ls>(reg_truck);
                foreach (Ls i in info.i) ls_truck.Add(i);

                info = JsonConvert.DeserializeObject<Sun_Ls>(reg_special);
                foreach (Ls i in info.i) ls_special.Add(i);

                info = JsonConvert.DeserializeObject<Sun_Ls>(reg_moto);
                foreach (Ls i in info.i) ls_moto.Add(i);

                info = JsonConvert.DeserializeObject<Sun_Ls>(reg_cross);
                foreach (Ls i in info.i) ls_cross.Add(i);

                info = JsonConvert.DeserializeObject<Sun_Ls>(reg_sts);
                foreach (Ls i in info.i) ls_sts.Add(i);

            }

            // ls.Sort((x, y) =>  x.name.CompareTo(y.name));
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            set_hook();
          //  un_hook();
            String str = cur_path + "\\img\\";
            load_img_s(img_i, str);

            init();
            load_item();
            i();
            lang();

            timer1.Enabled = false;
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            label1.Text = DateTime.Now.ToShortTimeString();
            label2.Text = DateTime.Now.ToLongDateString();
        }

        private void Timer3_Tick(object sender, EventArgs e)
        {
            f.TopMost = true;
        }

        private void Form1_Enter(object sender, EventArgs e)
        {
            SendToBack();
        }


        // ---------------------------------------------------------------------------------------------------------------- hook

        //Установка перехвата клавиатуры
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(
            int idHook, LowLevelKeyboardProcDelegate lpfn, IntPtr hMod, int dwThreadId);

        //Разблокировка клавиатуры
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        //Hook handle
        [System.Runtime.InteropServices.DllImport("Kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(IntPtr lpModuleName);

        //Вызов следующего хука
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallNextHookEx(
            IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            //  TextBox1_Leave(sender, e);
        }



        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);


        [DllImport("user32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);


        // ---------------------------------------------------------------------------------------------------------------- hook

        // ---------------------------------------------------------------------------------------------------------------- log out
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int ExitWindowsEx(ExitWindows uFlags, ShutdownReason dwReason);
        [Flags]
        public enum ExitWindows : uint
        {
            // ONE of the following five:
            LogOff = 0x00,
            ShutDown = 0x01,
            Reboot = 0x02,
            PowerOff = 0x08,
            RestartApps = 0x40,
            // plus AT MOST ONE of the following two:
            Force = 0x04,
            ForceIfHung = 0x10,
        }
        [Flags]
        enum ShutdownReason : uint
        {
            MajorApplication = 0x00040000,
            MajorHardware = 0x00010000,
            MajorLegacyApi = 0x00070000,
            MajorOperatingSystem = 0x00020000,
            MajorOther = 0x00000000,
            MajorPower = 0x00060000,
            MajorSoftware = 0x00030000,
            MajorSystem = 0x00050000,

            MinorBlueScreen = 0x0000000F,
            MinorCordUnplugged = 0x0000000b,
            MinorDisk = 0x00000007,
            MinorEnvironment = 0x0000000c,
            MinorHardwareDriver = 0x0000000d,
            MinorHotfix = 0x00000011,
            MinorHung = 0x00000005,
            MinorInstallation = 0x00000002,
            MinorMaintenance = 0x00000001,
            MinorMMC = 0x00000019,
            MinorNetworkConnectivity = 0x00000014,
            MinorNetworkCard = 0x00000009,
            MinorOther = 0x00000000,
            MinorOtherDriver = 0x0000000e,
            MinorPowerSupply = 0x0000000a,
            MinorProcessor = 0x00000008,
            MinorReconfig = 0x00000004,
            MinorSecurity = 0x00000013,
            MinorSecurityFix = 0x00000012,
            MinorSecurityFixUninstall = 0x00000018,
            MinorServicePack = 0x00000010,
            MinorServicePackUninstall = 0x00000016,
            MinorTermSrv = 0x00000020,
            MinorUnstable = 0x00000006,
            MinorUpgrade = 0x00000003,
            MinorWMI = 0x00000015,

            FlagUserDefined = 0x40000000,
            FlagPlanned = 0x80000000
        }
        // ---------------------------------------------------------------------------------------------------------------- log out

    }
}
